using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.Collections;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Objects
{
	/// <summary>
	/// Script object created by `new object` or any function or class in the script.
	/// The design is based on JavaScript (ECMA-262)
	/// </summary>
	public class UserObject : Descriptor, ISelfDescribing
	{
		Descriptor ISelfDescribing.Descriptor => this;

		/// <summary>
		/// Create new user object inheriting from this one
		/// </summary>
		public override bool Call(ref Value result, object self, in Arguments args)
		{
			var it = new UserObject(this);
			result = new Value(it, it);
			return true;
		}

		public delegate void Read(Core core, UserObject self, out Value value);
		public delegate void Write(Core core, UserObject self, ref Value value);
		/// <summary>
		/// Single property of an object (with name and value)
		/// </summary>
		[DebuggerDisplay("{name} = {value}")]
		public struct Prop
		{
			public string name;
			public Value value;
			public Read read;
			public Write write;
			public override string ToString()
				=> string.Format(Value.Culture, "{0} = {1}", name, value.ToString());
		}
		/// <summary>
		/// All properties of the object
		/// (properties from parent are auto-added when accessed)
		/// </summary>
		protected internal ListCore<Prop> prop;
		/// <summary>
		/// Map of all properties (name-to-index into <see cref="prop"/>)
		/// </summary>
		protected internal readonly Dictionary<string, int> dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		/// <summary>
		/// Parent object (this one is derived from, null if no such)
		/// </summary>
		protected internal UserObject parent;
		/// <summary>
		/// Number of locked / read-only properties
		/// </summary>
		protected int readOnlyTop = 0;

		public UserObject()
			: base("user object", typeof(UserObject)) { }
		public UserObject(string name)
			: base(name) { }
		public UserObject(string name, Type type)
			: base(name, type) { }
		public UserObject(string name, Type type, UserObject parent)
			: base(name, type)
			=> this.parent = parent;

		public UserObject(Type type)
			: base(type.Name, type) { }
		public UserObject(Type type, UserObject parent)
			: base(type.Name, type)
			=> this.parent = parent;

		public UserObject(UserObject parent)
			: this() => this.parent = parent;
		public UserObject(string name, UserObject parent)
			: this(name, typeof(UserObject))
			=> this.parent = parent;

		internal UserObject(string name, Type type, ExCode primitive, TypeCode typeCode, UserObject parent)
			: base(name, type, primitive, typeCode)
			=> this.parent = parent;

		/// <summary>
		/// Make all current properties read-only
		/// </summary>
		public void Lock()
			=> readOnlyTop = prop.size;
		/// <summary>
		/// Remove all writable properties (those added after last <see cref="Lock()"/>)
		/// </summary>
		public virtual void Reset()
		{
			prop.Count = readOnlyTop;
			if (dict != null)
			{
				dict.Clear();
				for (int i = 0; i < prop.size; i++)
				{
					var name = prop.items[i].name;
					if (name != null)
						dict[name] = i;
				}
			}
		}

		public int Add(Type type)
			=> Add(type.Name, new Value(type));
		public int Add(string name, Type type)
			=> Add(name, new Value(type));
		public int Add(string name, UserObject it)
			=> Add(name, new Value(it));
		public int Add(string name, object it)
			=> Add(name, new Value(it));
		public int Add(string name, Value value)
			=> Add(name, ref value);
		public virtual int Add(string name, ref Value value)
		{
			var idx = prop.size;
			ref var it = ref prop.Add();
			it.name = name;
			it.value = value;
			if (name != null)
				dict[name] = idx;
			return idx;
		}
		public virtual int Add(string name, Read read, Write write = null)
		{
			var idx = prop.size;
			ref var it = ref prop.Add();
			it.name = name;
			it.read = read;
			it.write = write;
			if (name != null)
				dict[name] = idx;
			return idx;
		}
		public virtual int Find(string name)
		{
			if (dict.TryGetValue(name, out var at))
				return at;
			if (parent == null)
				return -1;
			at = parent.Find(name);
			if (at < 0)
				return at;
			return ImportFrom(parent, name, at);
		}
		protected int ImportFrom(UserObject space, string name, int at)
		{
			ref var p = ref space.prop.items[at];
			if (p.value.IsValid)
				return Add(name, p.value);
			return Add(name, p.read, p.write);
		}
		public Value this[string name]
		{
			get
			{
				int at = Find(name);
				if (at < 0) return Value.Void;
				ref var p = ref prop.items[at];
				if (p.value.IsValid)
					return p.value;
				var read = p.read;
				if (read == null)
					throw new InvalidOperation($"{name} is write-only");
				read(null, this, out var v);
				return v;
			}
			set
			{
				int at = Find(name);
				if (at < 0)
					Add(name, ref value);
				else if (at < readOnlyTop)
					throw new InvalidOperation($"{name} is read-only");
				else
				{
					ref var p = ref prop.items[at];
					if (p.value.IsValid)
						p.value = value;
					else
					{
						var write = p.write;
						if (write == null)
							throw new InvalidOperation($"{name} is read-only");
						write(null, this, ref value);
					}
				}
			}
		}
		public bool Has(string name)
			=> Find(name) >= 0;
		public override bool Has(Core core, ref Value self, string name)
			=> Find(name) >= 0;
		public override void Get(Core core, ref Value self)
		{
			int at;
			ValueBox box = null;
			if (self.idx is string name)
			{
				at = Find(name);
				goto getit2;
			}
			if (self.IsIntIndex)
			{
				at = self.num.Int;
				goto getit;
			}
			box = self.idx as ValueBox;
			if (box == null)
				goto fail;
			if (box.Value.IsStringOrChar)
			{
				at = Find(box.Value.ToStr());
				goto getit2;
			}
			if (box.Value.IsIntegral)
			{
				var num = box.Value.num.Long;
				if (num < 0 || num >= prop.size)
					goto fail;
				at = (int)num;
				goto getit3;
			}
		fail:
			GetError(ref self);
			return;
		getit:
			if (at >= prop.size)
				goto fail;
			getit2:
			if (at < 0)
				goto fail;
			getit3:
			ref var p = ref prop.items[at];
			if (p.value.IsValid)
			{
				self = p.value;
				if (box != null)
					ValueBox.Return(box);
				return;
			}
			var read = p.read;
			if (read == null)
				goto fail;
			read(core, this, out self);
			if (box != null)
				ValueBox.Return(box);
		}
		public override void Set(Core core, ref Value self, OpCode op, ref Value value)
		{
			int at = -1;
			ValueBox box = null;
			if (self.idx is string name)
			{
				at = Find(name);
				if (at >= 0)
					goto setit3;
				if (op != OpCode.Assign)
					goto fail;
				at = Add(name, Value.Void);
				goto setit3;
			}
			if (self.IsIntIndex)
			{
				at = self.num.Int;
				goto setit;
			}
			box = self.idx as ValueBox;
			if (box == null)
				goto fail;
			if (box.Value.IsStringOrChar)
			{
				name = box.Value.ToStr();
				at = Find(name);
				if (at >= 0)
					goto setit3;
				if (op != OpCode.Assign)
					goto fail;
				at = Add(name, Value.Void);
				goto setit3;
			}
			if (box.Value.IsIntegral)
			{
				var num = box.Value.num.Long;
				if (num < 0 || num >= prop.size)
					goto fail;
				at = (int)num;
				goto setit3;
			}
		fail:
			SetError(ref self, ref value);
		setit:
			if (at >= prop.size)
				goto fail;
		//setit2:
			if (at < readOnlyTop)
				goto fail;
		setit3:
			ref var p = ref prop.items[at];
			if (op == OpCode.Assign)
			{
				if (p.value.IsValid)
				{
					p.value = value;
					return;
				}
				var write = p.write;
				if (write == null)
					goto fail;
				write(core, this, ref value);
			}
			if (op.Kind() == OpKind.Assign)
			{
				if (p.value.IsValid)
				{
					if (!p.value.desc.Binary(ref p.value, op + 0x10, ref value)
					&& !value.desc.Binary(ref p.value, op + 0x10, ref value))
						goto fail;
					return;
				}
				var read = p.read;
				if (read == null)
					goto fail;
				var write = p.write;
				if (write == null)
					goto fail;
				read(core, this, out var it);
				if (!it.desc.Binary(ref it, op + 0x10, ref value)
				&& !value.desc.Binary(ref it, op + 0x10, ref value))
					goto fail;
				write(core, this, ref it);
				return;
			}
			if (op.Kind() == OpKind.PreOrPost)
			{
				if (p.value.IsValid)
				{
					if (op >= OpCode.Inc)
					{
						p.value.desc.Unary(ref p.value, op);
						return;
					}
					self = p.value;
					if (box != null) ValueBox.Return(box);
					p.value.desc.Unary(ref p.value, op + 0x08);
					return;
				}
				var read = p.read;
				if (read == null)
					goto fail;
				var write = p.write;
				if (write == null)
					goto fail;
				read(core, this, out var it);
				if (op >= OpCode.Inc)
					it.desc.Unary(ref it, op);
				else
				{
					self = it;
					if (box != null) ValueBox.Return(box);
					it.desc.Unary(ref it, op + 0x08);
				}
				write(core, this, ref it);
				return;
			}
			goto fail;
		}

		public override IEnumerable<string> EnumerateProperties(object self)
		{
			foreach (var p in prop)
			{
				var name = p.name;
				if (name != null)
					yield return name;
			}
			if (parent == null)
				yield break;
			foreach (var name in parent.EnumerateProperties(self))
				if (!dict.ContainsKey(name))
					yield return name;
		}
	}
}

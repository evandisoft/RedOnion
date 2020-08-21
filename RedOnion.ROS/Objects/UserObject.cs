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
		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			var it = new UserObject(this);
			result = new Value(it, it);
			return true;
		}

		/// <summary>
		/// Single property of an object (with name and value)
		/// </summary>
		[DebuggerDisplay("{name} = {value}")]
		protected internal struct Prop
		{
			public string name;
			public Value value;
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
		protected internal Dictionary<string, int> dict;
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
			: base(name, typeof(UserObject)) { }
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
			{
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[name] = idx;
			}
			return idx;
		}
		public virtual int Find(string name)
		{
			if (dict != null && dict.TryGetValue(name, out var idx))
				return idx;
			if (parent == null)
				return -1;
			idx = parent.Find(name);
			if (idx < 0)
				return idx;
			return Add(name, ref parent.prop.items[idx].value);
		}
		protected int ImportFrom(UserObject space, string name, int at)
			=> Add(name, ref space.prop.items[at].value);
		public Value this[string name]
		{
			get
			{
				int at = Find(name);
				return at < 0 ? Value.Void : prop.items[at].value;
			}
			set
			{
				int at = Find(name);
				if (at < 0)
					Add(name, ref value);
				else if (at >= readOnlyTop)
					prop.items[at].value = value;
			}
		}
		public bool Has(string name)
			=> Find(name) >= 0;
		public override bool Has(ref Value self, string name)
			=> Find(name) >= 0;
		public override void Get(ref Value self)
		{
			if (self.idx is string name)
			{
				int at = Find(name);
				if (at >= 0)
					self = prop.items[at].value;
				return;
			}
			if (self.IsIntIndex)
			{
				int at = self.num.Int;
				if (at >= 0 && at < prop.size)
					self = prop.items[at].value;
				return;
			}
			if (self.idx is ValueBox box)
			{
				if (box.Value.IsStringOrChar)
				{
					var at = Find(box.Value.ToStr());
					if (at >= 0)
					{
						self = prop.items[at].value;
						ValueBox.Return(box);
						return;
					}
				}
			}
			GetError(ref self);
		}
		public override void Set(ref Value self, OpCode op, ref Value value)
		{
			int at = -1;
			ValueBox box = null;
			if (self.idx is string name)
			{
				at = Find(name);
				if (at < 0 && op == OpCode.Assign)
					at = Add(name, Value.Void);
			}
			else if (self.IsIntIndex)
			{
				at = self.num.Int;
				if (at >= prop.size)
					at = -1;
			}
			else if (self.idx is ValueBox box2)
			{
				box = box2;
				if (box.Value.IsStringOrChar)
				{
					at = Find(name = box.Value.ToStr());
					if (at < 0 && op == OpCode.Assign)
						at = Add(name, Value.Void);
				}
			}
			if (at >= readOnlyTop)
			{
				if (op == OpCode.Assign)
				{
					prop.items[at].value = value;
					return;
				}
				ref var it = ref prop.items[at].value;
				if (op.Kind() == OpKind.Assign)
				{
					if (it.desc.Binary(ref it, op + 0x10, ref value))
						return;
				}
				else if (op.Kind() == OpKind.PreOrPost)
				{
					if (op >= OpCode.Inc)
					{
						it.desc.Unary(ref it, op);
						return;
					}
					self = it;
					if (box != null) ValueBox.Return(box);
					it.desc.Unary(ref it, op + 0x08);
					return;
				}
			}
			GetError(ref self);
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

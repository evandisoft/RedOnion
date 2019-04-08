using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Objects
{
	public class UserObject : Descriptor
	{
		[DebuggerDisplay("{name} = {value}")]
		protected struct Prop
		{
			public string name;
			public Value value;
			public override string ToString()
				=> string.Format(Value.Culture, "{0} = {1}", name, value.ToString());
		}
		protected ListCore<Prop> prop;
		protected Dictionary<string, int> dict;
		protected UserObject parent;
		protected int readOnlyTop = 0;

		public UserObject() : base("user object", typeof(UserObject)) { }
		public UserObject(UserObject parent)
			: this() => this.parent = parent;

		/// <summary>
		/// Create new user object inheriting from this one
		/// </summary>
		public override bool Call(ref Value result, ref Value self, Arguments args, bool create)
		{
			var it = new UserObject(this);
			result = new Value(it, it);
			return true;
		}

		/// <summary>
		/// Make all current properties read-only
		/// </summary>
		public void Lock()
			=> readOnlyTop = prop.size;
		/// <summary>
		/// Remove all writable properties (those added after last <see cref="Lock()"/>)
		/// </summary>
		public void Reset()
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

		public int Add(string name, Value value)
			=> Add(name, ref value);
		public int Add(string name, ref Value value)
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
		public int Find(string name)
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
				if (at < readOnlyTop)
					return;
				if (at >= prop.size)
					Add(name, ref value);
				else prop.items[at].value = value;
			}
		}
		public override int Find(object self, string name, bool add)
		{
			if (dict != null && dict.TryGetValue(name, out var idx))
				return idx;
			if (parent != null)
			{
				idx = parent.Find(name);
				if (idx >= 0)
					return Add(name, ref parent.prop.items[idx].value);
			}
			return add ? Add(name, Value.Void) : -1;
		}
		public override string NameOf(object self, int at)
			=> prop[at].name ?? "#" + at;
		public override bool Get(ref Value self, int at)
		{
			if (at < 0 || at >= prop.size)
				return false;
			self = prop.items[at].value;
			return true;
		}
		public override bool Set(ref Value self, int at, OpCode op, ref Value value)
		{
			if (at < readOnlyTop || at >= prop.size)
				return false;
			if (op == OpCode.Assign)
			{
				prop.items[at].value = value;
				return true;
			}
			ref var it = ref prop.items[at].value;
			if (op.Kind() == OpKind.Assign)
				return it.desc.Binary(ref it, op + 0x10, ref value);
			if (op.Kind() != OpKind.PreOrPost)
				return false;
			if (op >= OpCode.Inc)
				return it.desc.Unary(ref it, op);
			self = it;
			return it.desc.Unary(ref it, op + 0x08);
		}
		public override int IndexFind(object self, Arguments args)
		{
			if (args.Length == 0 || dict == null)
				return -1;
			var index = args[0];
			if (!index.IsNumber)
			{
				if (!index.desc.Convert(ref index, String))
					return -1;
				return Find(index.obj.ToString());
			}
			var at = index.ToInt();
			if (at < 0 || at >= prop.size)
				return -1;
			if (args.Length == 1)
				return at;
			ref var it = ref prop.items[at].value;
			return it.desc.IndexFind(it.obj, new Arguments(args, args.Length-1));
		}
	}
}

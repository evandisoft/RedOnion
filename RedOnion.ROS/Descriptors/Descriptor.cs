using System;
using System.Collections.Generic;
using System.Threading;

namespace RedOnion.ROS
{
	public abstract partial class Descriptor
	{
		public string Name { get; }
		public Type Type { get; }
		public ExCode Primitive { get; }
		public TypeCode TypeCode { get; }
		public virtual object Box(ref Value self) => self.obj;
		public virtual bool Equals(ref Value self, object obj)
			=> self.obj.Equals(obj);
		public virtual int GetHashCode(ref Value self)
			=> self.obj.GetHashCode();

		protected Descriptor(string name, Type type)
		{
			Name = name;
			Type = type;
			TypeCode = TypeCode.Object;
		}
		internal Descriptor(string name, Type type, ExCode primitive, TypeCode typeCode)
		{
			Name = name;
			Type = type;
			Primitive = primitive;
			TypeCode = typeCode;
		}

		public override string ToString() => Name;
		public virtual string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
			=> format != null && self.obj is IFormattable fmt
			? fmt.ToString(format, provider) : self.obj?.ToString() ?? Name;

		public virtual bool Convert(ref Value self, Descriptor to)
		{
			if (to.Primitive == ExCode.String)
			{
				self = ToString(ref self, null, Value.Culture, false);
				return true;
			}
			return false;
		}

		public virtual bool Unary(ref Value self, OpCode op) => false;
		public virtual bool Binary(ref Value lhs, OpCode op, ref Value rhs) => false;
		public virtual bool Call(ref Value result, ref Value self, Arguments args, bool create) => false;
		public virtual int Find(object self, string name, bool add) => -1;
		public virtual string NameOf(object self, int at) => null;
		public virtual bool Get(ref Value self, int at) => false;
		public virtual bool Set(ref Value self, int at, OpCode op, ref Value value) => false;
		public virtual IEnumerator<Value> Enumerate(ref Value self) => null;

		public virtual int IndexFind(ref Value self, Arguments args)
		{
			if (args.Length == 0)
				return -1;
			ref var index = ref args.GetRef(0);
			int at;
			if (index.IsNumber)
				return -1;
			if (!index.desc.Convert(ref index, String))
				return -1;
			var name = index.obj.ToString();
			at = Find(self.obj, name, true);
			if (at < 0 || args.Length == 1)
				return at;
			if (!Get(ref self, at))
				return -1;
			return self.desc.IndexFind(ref self, new Arguments(args, args.Length-1));
		}

		static internal InvalidOperationException InvalidOperation(string msg)
			=> new InvalidOperationException(msg);
		static internal InvalidOperationException InvalidOperation(string msg, params object[] args)
			=> new InvalidOperationException(string.Format(Value.Culture, msg, args));
	}
}

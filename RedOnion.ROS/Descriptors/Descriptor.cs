using System;
using System.Collections.Generic;

namespace RedOnion.ROS
{
	public abstract partial class Descriptor
	{
		public string Name { get; }
		public Type Type { get; }
		public ExCode Primitive { get; }
		public TypeCode TypeCode { get; }
		public virtual object Box(ref Value self) => self.obj;

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
			? fmt.ToString(format, provider) : self.obj.ToString();

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
		public virtual int IndexFind(object self, Arguments args) => -1;
		public virtual string NameOf(object self, int at) => null;
		public virtual bool Get(ref Value self, int at) => false;
		public virtual bool Set(ref Value self, int at, OpCode op, ref Value value) => false;
		public virtual IEnumerator<Value> Enumerate(ref Value self) => null;
	}
}

using RedOnion.ROS.Utilities;
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
		public bool IsNumber => Primitive.Kind() == OpKind.Number;
		public bool IsNumberOrChar { get; }
		public bool IsStringOrChar { get; }
		public bool IsFpNumber { get; }
		public bool IsIntegral { get; }

		public virtual object Box(ref Value self)
			=> self.obj;
		public virtual bool IsInstanceOf(ref Value it)
			=> Type.IsInstanceOfType(it.Box());
		public virtual bool Equals(ref Value self, object obj)
			=> obj is Value val ? val.desc == this && self.obj.Equals(val.obj) : self.obj.Equals(obj);
		public virtual int GetHashCode(ref Value self)
			=> self.obj.GetHashCode();

		protected Descriptor(Type type)
			: this(type.Name, type) { }
		protected Descriptor(string name, Type type)
		{
			Name = name ?? type.Name;
			Type = type;
			Primitive = type.IsEnum ? ExCode.Enum : type.IsValueType ? ExCode.Struct : ExCode.Class;
			TypeCode = TypeCode.Object;
		}
		protected Descriptor(string name)
		{
			var type = GetType();
			Name = name ?? type.Name;
			Type = type;
			Primitive = ExCode.Class;
			TypeCode = TypeCode.Object;
		}
		protected Descriptor()
		{
			var type = GetType();
			Name = type.Name;
			Type = type;
			Primitive = ExCode.Class;
			TypeCode = TypeCode.Object;
		}
		internal Descriptor(string name, Type type, ExCode primitive, TypeCode typeCode)
		{
			Name = name;
			Type = type;
			Primitive = primitive;
			TypeCode = typeCode;
			var code = (OpCode)primitive;
			IsNumberOrChar = code >= OpCode.Char && code < OpCode.Create;
			IsStringOrChar = code >= OpCode.String && code <= OpCode.LongChar;
			IsFpNumber = code >= OpCode.Float && code <= OpCode.LongDouble;
			IsIntegral = code.Kind() == OpKind.Number && !IsFpNumber;
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

		// TODO: ConvertFrom or CreateFrom and maybe CanCreateFrom - for argument matching

		/// <summary>
		/// Check existence of a property in value <paramref name="self"/> named <paramref name="name"/>.
		/// </summary>
		public virtual bool Has(Core core, ref Value self, string name) => false;
		/// <summary>
		/// Get value of property (if <paramref name="self"/>.idx is string) or indexed value.
		/// </summary>
		public virtual void Get(Core core, ref Value self) => GetError(ref self);
		/// <summary>
		/// Set/modify value of property (if <paramref name="self"/>.idx is string) or indexed value.
		/// </summary>
		public virtual void Set(Core core, ref Value self, OpCode op, ref Value value) => GetError(ref self);

		protected void GetError(ref Value self)
		{
			if (self.IsIntIndex)
				throw new InvalidOperationException(string.Format(Value.Culture,
					"`{0}` cannot be indexed by [int: {1}]", Name, self.num.Int));
			if (self.idx is string name)
				throw new InvalidOperationException(string.Format(Value.Culture,
					"`{0}` does not have property named `{1}`", Name, name));
			if (self.idx is ValueBox idx)
				throw new InvalidOperationException(string.Format(Value.Culture,
					"`{0}` cannot be indexed by '{1}'", Name, idx.ToString()));
			if (self.idx is ValueBox[] arr)
				throw new InvalidOperationException(string.Format(Value.Culture,
					"`{0}` cannot be indexed by '{1}', ...", Name, arr[0].ToString()));
			throw new InvalidOperationException("Unknown indexing");
		}
		protected void SetError(ref Value self, ref Value value)
			=> GetError(ref self);

		public virtual void Unary(ref Value self, OpCode op)
			=> UnaryError(op);
		protected void UnaryError(OpCode op)
			=> throw new InvalidOperation("Unary operator '{0}' not supported on operand '{1}'", op.Text(), Name);
		public virtual bool Binary(ref Value lhs, OpCode op, ref Value rhs) => false;
		public virtual bool Call(ref Value result, object self, Arguments args, bool create = false) => false;

		public virtual IEnumerable<Value> Enumerate(object self) => null;
		public virtual IEnumerable<string> EnumerateProperties(object self)
		{
			yield break;
		}
	}
}

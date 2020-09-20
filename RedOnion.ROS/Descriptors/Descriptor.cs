using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RedOnion.ROS
{
	public abstract partial class Descriptor
	{
		/// <summary>
		/// Name of the descriptor (usually matches Type.Name)
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Type it describes
		/// </summary>
		public Type Type { get; }
		/// <summary>
		/// Associated <see cref="ExCode"/> (<see cref="ExCode.Class"/>/<see cref="ExCode.Struct"/>/<see cref="ExCode.Enum"/> for most but built-in descriptors)
		/// </summary>
		public ExCode Primitive { get; }
		/// <summary>
		/// Associated <see cref="TypeCode"/> (<see cref="TypeCode.Object"/> for most but built-in descriptors)
		/// See also <see cref="IConvertible.GetTypeCode()"/> and <see cref="Type.GetTypeCode(Type)"/>.
		/// </summary>
		public TypeCode TypeCode { get; }
		/// <summary>
		/// Describes number (<see cref="Primitive.Kind()"/> == <see cref="OpKind.Number"/>)
		/// </summary>
		public bool IsNumber => Primitive.Kind() == OpKind.Number;
		/// <summary>
		/// Describes number or character (<see cref="Primitive"/> &gt;= <see cref="OpCode.Char"/> and &lt;= <see cref="OpCode.Decimal"/>)
		/// </summary>
		public bool IsNumberOrChar { get; }
		/// <summary>
		/// Describes string or character (<see cref="Primitive"/> &gt;= <see cref="OpCode.String"/> and &lt;= <see cref="OpCode.LongChar"/>)
		/// </summary>
		public bool IsStringOrChar { get; }
		/// <summary>
		/// Describes floating point number (<see cref="Primitive"/> &gt;= <see cref="OpCode.Float"/> and code &lt;= <see cref="OpCode.LongDouble"/>)
		/// </summary>
		public bool IsFpNumber { get; }
		/// <summary>
		/// Describes integral number (<see cref="IsNumber"/> and !<see cref="IsFpNumber"/>)
		/// </summary>
		public bool IsIntegral { get; }
		/// <summary>
		/// Box the value (usually returns <see cref="Value.obj"/> but builtin descriptors will box the value-type).
		/// Used whenever the value needs to be passed as <see cref="object"/>.
		/// </summary>
		public virtual object Box(ref Value self)
			=> self.obj;
		/// <summary>
		/// Implements the <c>is</c> operator. Defaults to <c>Type.IsInstanceOfType(it.Box())</c>
		/// </summary>
		public virtual bool IsInstanceOf(ref Value it)
			=> Type.IsInstanceOfType(it.Box());
		/// <summary>
		/// Implements equality operator and <see cref="Value.Equals(object)"/>. Defaults to <c>obj is Value val ? val.desc == this &amp;&amp; self.obj.Equals(val.obj) : self.obj.Equals(obj);</c>
		/// </summary>
		public virtual bool Equals(ref Value self, object obj)
			=> obj is Value val ? val.desc == this && self.obj.Equals(val.obj) : self.obj.Equals(obj);
		/// <summary>
		/// Implements <see cref="Value.GetHashCode()"/> which can be used for hashing (e.g. in <see cref="Objects.RosDictionary"/>)
		/// </summary>
		public virtual int GetHashCode(ref Value self)
			=> self.obj.GetHashCode();

		/// <summary>
		/// Create descriptor for given type
		/// </summary>
		protected Descriptor(Type type)
			: this(type.Name, type) { }
		/// <summary>
		/// Create descriptor with given name for given type
		/// </summary>
		protected Descriptor(string name, Type type)
		{
			Name = name ?? type.Name;
			Type = type;
			Primitive = type.IsEnum ? ExCode.Enum : type.IsValueType ? ExCode.Struct : ExCode.Class;
			TypeCode = TypeCode.Object;
		}
		/// <summary>
		/// Create self-descriptor with given name
		/// </summary>
		protected Descriptor(string name)
		{
			var type = GetType();
			Name = name ?? type.Name;
			Type = type;
			Primitive = ExCode.Class;
			TypeCode = TypeCode.Object;
		}
		/// <summary>
		/// Create self-descriptor
		/// </summary>
		protected Descriptor()
		{
			var type = GetType();
			Name = type.Name;
			Type = type;
			Primitive = ExCode.Class;
			TypeCode = TypeCode.Object;
		}
		/// <summary>
		/// Create builtin descriptor
		/// </summary>
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
		/// <summary>
		/// Convert the value to string (should never fail/throw if <paramref name="debug"/> is true).
		/// See <see cref="Value.ToStr()"/> (<paramref name="debug"/> = false, <paramref name="format"/> = null, <paramref name="provider"/> = <see cref="Value.Culture"/>)
		/// and <see cref="Value.ToString(string, IFormatProvider)"/> (<paramref name="debug"/> = true)
		/// </summary>
		/// <param name="self">The value</param>
		/// <param name="format">Format</param>
		/// <param name="provider">Format provider</param>
		/// <param name="debug">The conversion is for debug/printout purposes</param>
		public virtual string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
			=> format != null && self.obj is IFormattable fmt
			? fmt.ToString(format, provider) : self.obj?.ToString() ?? Name;

		/// <summary>
		/// Try to convert the value to new type (<paramref name="to"/> could be <c>this</c>).
		/// Should not throw exceptions (return false instead).
		/// </summary>
		/// <param name="self">The value to be converted</param>
		/// <param name="to">Target descriptor</param>
		/// <param name="flags">Conversion flags (currently only <see cref="CallFlags.Explicit"/> is relevant)</param>
		/// <returns>True if converted, false if not</returns>
		public virtual bool Convert(ref Value self, Descriptor to, CallFlags flags = CallFlags.Convert)
		{
			if (to == this)
				return true;
			if (to.Primitive == ExCode.String)
			{
				self = ToString(ref self, null, Value.Culture, false);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check existence of a property in value <paramref name="self"/> named <paramref name="name"/>.
		/// (See <see cref="Objects.UserObject.Has(Core, ref Value, string)"/> and <see cref="Reflected.Has(Core, ref Value, string)"/>)
		/// </summary>
		/// <param name="core">The core executing the query (can be null)</param>
		public virtual bool Has(Core core, ref Value self, string name) => false;
		/// <summary>
		/// Get value of property (if <paramref name="self"/>.idx is string) or indexed value.
		/// (See <see cref="GetError(ref Value)"/>, <see cref="Objects.UserObject.Get(Core, ref Value)"/> and <see cref="Reflected.Get(Core, ref Value)"/>)
		/// </summary>
		/// <param name="core">The core executing the query (can be null - <see cref="P:Objects.UserObject.Item(string)"/>)</param>
		/// <param name="self">Both input (<see cref="Value.IsReference"/>) and output</param>
		public virtual void Get(Core core, ref Value self) => GetError(ref self);
		/// <summary>
		/// Set/modify value of property (if <paramref name="self"/>.idx is string) or indexed value.
		/// (See <see cref="SetError(ref Value, ref Value)"/>, <see cref="Objects.UserObject.Set(Core, ref Value, OpCode, ref Value)"/> and <see cref="Reflected.Set(Core, ref Value, OpCode, ref Value)"/>)
		/// </summary>
		/// <param name="core">The core executing the operation (can be null - <see cref="P:Objects.UserObject.Item(string)"/>)</param>
		/// <param name="self">Both input (<see cref="Value.IsReference"/>) and output</param>
		/// <param name="op">The operation (<see cref="OpKind.Assign"/> or <see cref="OpKind.PreOrPost"/>)</param>
		/// <param name="value">The value to be set / used as second argument (input only, do not modify)</param>
		public virtual void Set(Core core, ref Value self, OpCode op, ref Value value) => GetError(ref self);

		/// <summary>
		/// Throw standard exception based on type of <paramref name="self"/>
		/// </summary>
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
		/// <summary>
		/// Throw standard exception based on type of <paramref name="self"/>.
		/// (Currently redirects to <see cref="GetError(ref Value)"/>)
		/// </summary>
		protected void SetError(ref Value self, ref Value value)
			=> GetError(ref self);

		/// <summary>
		/// Implements unary operators.
		/// Execution engine will directly only use <see cref="OpKind.Unary"/>
		/// but <see cref="Set(Core, ref Value, OpCode, ref Value)"/> may redirect
		/// <see cref="OpKind.PreOrPost"/> here (even from different descriptor).
		/// (See <see cref="UnaryError(OpCode)"/> and <see cref="Reflected.Unary(ref Value, OpCode)"/>)
		/// </summary>
		/// <param name="self">Both input (<see cref="Value.IsReference"/>) and output</param>
		/// <param name="op">The operation (<see cref="OpKind.Unary"/> or <see cref="OpKind.PreOrPost"/>)</param>
		public virtual void Unary(ref Value self, OpCode op)
			=> UnaryError(op);
		/// <summary>
		/// Throw standard exception based on type of <paramref name="self"/>.
		/// </summary>
		protected void UnaryError(OpCode op)
			=> throw new InvalidOperation("Unary operator '{0}' not supported on operand '{1}'", op.Text(), Name);

		/// <summary>
		/// Implements binary operators (<see cref="OpKind.Binary"/> and <see cref="OpKind.Logic"/>
		/// except <see cref="OpCode.As"/> and higher).
		/// Should not throw exceptions (return false instead,
		/// the descriptor of second argument may handle it).
		/// </summary>
		/// <param name="lhs">Both first argument and output</param>
		/// <param name="op"></param>
		/// <param name="rhs">Second argument (input only, do not modify)</param>
		public virtual bool Binary(ref Value lhs, OpCode op, ref Value rhs) => false;

		/// <summary>
		/// Try to call the object.
		/// Should not throw exceptions (return false instead,
		/// may be part of method group and different overload may be attempted).
		/// </summary>
		/// <param name="result">Both input (the callable) and output (the result)</param>
		/// <param name="self">Value of `this` (null for function/static call)</param>
		/// <param name="args">Arguments and <see cref="CallFlags"/></param>
		public virtual bool Call(ref Value result, object self, in Arguments args) => false;

		/// <summary>
		/// Impelemts enumeration (<c>forach</c>).
		/// Should not throw exceptions (return null instead, if the object is not enumerable).
		/// </summary>
		public virtual IEnumerable<Value> Enumerate(object self) => null;

		/// <summary>
		/// Enumerate properties - mostly for suggestions (<c>RedOnion.KSP.ROS.RosSuggest</c>)
		/// </summary>
		public virtual IEnumerable<string> EnumerateProperties(object self)
		{
			yield break;
		}
	}
}

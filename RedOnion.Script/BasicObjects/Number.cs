using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// Number function (used to create new number objects)
	/// </summary>
	public class NumberFun : BasicObject
	{
		/// <summary>
		/// Prototype of all number objects
		/// </summary>
		public NumberObj Prototype { get; }

		/// <summary>
		/// Target type (null for no conversion)
		/// </summary>
		public override Type Type => _type;
		private Type _type;
		/// <summary>
		/// Target value kind
		/// </summary>
		public ValueKind Kind { get; }

		public override ObjectFeatures Features
			=> Type == null ? ObjectFeatures.Function | ObjectFeatures.Constructor
			: ObjectFeatures.Function | ObjectFeatures.Constructor
			| ObjectFeatures.TypeReference; // maybe add converter

		public NumberFun(IEngine engine, IObject baseClass, NumberObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;
		public NumberFun(IEngine engine, IObject baseClass, NumberObj prototype,
			Type type, ValueKind kind)
			: this(engine, baseClass, prototype)
		{
			_type = type;
			Kind = kind;
		}

		public override Value Call(IObject self, Arguments args)
		{
			if (args.Length == 0)
				return new Value();
			var value = args[0].Number;
			if (Type == null)
				return value;
			switch (Kind)
			{
			case ValueKind.Byte:
				return value.Byte;
			case ValueKind.UShort:
				return value.UShort;
			case ValueKind.UInt:
				return value.UInt;
			case ValueKind.ULong:
				return value.ULong;
			case ValueKind.SByte:
				return value.SByte;
			case ValueKind.Short:
				return value.Short;
			case ValueKind.Int:
				return value.Int;
			case ValueKind.Long:
				return value.Long;
			case ValueKind.Float:
				return value.Float;
			case ValueKind.Double:
				return value.Double;
			case ValueKind.Bool:
				return value.Bool;
			case ValueKind.Char:
				return value.Char;
			}
			throw new NotImplementedException("Conversion to number from " + value.Name);
		}

		public override IObject Create(Arguments args)
			=> new NumberObj(Engine, Prototype, Call(null, args));
	}

	/// <summary>
	/// Number object (value box)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {Number}")]
	[Creator(typeof(NumberFun))]
	public class NumberObj : BasicObject
	{
		/// <summary>
		/// Boxed value
		/// </summary>
		public Value Number { get; protected set; }
		public override Value Value => Number;

		/// <summary>
		/// Create Number.prototype
		/// </summary>
		public NumberObj(IEngine engine, IObject baseClass)
			: base(engine, baseClass)
		{
		}

		/// <summary>
		/// Create new number object boxing the value
		/// </summary>
		public NumberObj(IEngine engine, NumberObj baseClass, Value value)
			: base(engine, baseClass, StdProps)
			=> Number = value;

		public static Properties StdProps { get; } = new Properties();
	}
}

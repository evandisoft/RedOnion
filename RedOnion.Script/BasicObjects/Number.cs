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

		public NumberFun(Engine engine, IObject baseClass, NumberObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
		{
			Prototype = prototype;
		}

		public override Value Call(IObject self, int argc)
			=> argc == 0 ? new Value() : Arg(argc).Number;

		public override IObject Create(int argc)
			=> new NumberObj(Engine, Prototype, argc == 0 ? new Value() : Arg(argc).Number);
	}

	/// <summary>
	/// Number object (value box)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {Number}")]
	public class NumberObj : BasicObject
	{
		/// <summary>
		/// Boxed value
		/// </summary>
		public Value Number { get; protected set; }
		public override Value Value => this.Number;

		/// <summary>
		/// Create Number.prototype
		/// </summary>
		public NumberObj(Engine engine, IObject baseClass)
			: base(engine, baseClass)
		{
		}

		/// <summary>
		/// Create new number object boxing the value
		/// </summary>
		public NumberObj(Engine engine, NumberObj baseClass, Value value)
			: base(engine, baseClass, StdProps)
			=> Number = value;

		public static Properties StdProps { get; } = new Properties();
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// String function (used to create new string objects)
	/// </summary>
	public class StringFun : BasicObject, IObjectAndConverter
	{
		/// <summary>
		/// Prototype of all string objects
		/// </summary>
		public StringObj Prototype { get; }

		public StringFun(Engine engine, IObject baseClass, StringObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;

		public override Value Call(IObject self, int argc)
			=> argc == 0 ? new Value("") : new Value(Arg(argc).String);

		public override IObject Create(int argc)
			=> new StringObj(Engine, Prototype, argc == 0 ? "" : Arg(argc).String);

		public IObject Convert(object value)
			=> new StringObj(Engine, Prototype, value.ToString());
	}

	/// <summary>
	/// String object (string box)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {String}")]
	public class StringObj : BasicObject
	{
		/// <summary>
		/// Boxed value
		/// </summary>
		public String String { get; protected set; }
		public override Value Value => new Value(String);

		/// <summary>
		/// Create String.prototype
		/// </summary>
		public StringObj(Engine engine, IObject baseClass)
			: base(engine, baseClass)
			=> String = "";

		/// <summary>
		/// Create new string object boxing the string
		/// </summary>
		public StringObj(Engine engine, StringObj baseClass, string value)
			: base(engine, baseClass, StdProps)
			=> String = value;

		public static Properties StdProps { get; } = new Properties();
		static StringObj()
		{
			StdProps.Set("length", new Length());
		}

		public class Length : IProperty
		{
			public Value Get(IObject obj)
				=> new Value(((StringObj)obj).String.Length);
			public bool Set(IObject obj, Value value)
				=> false;
		}
	}
}

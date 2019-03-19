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
	public class StringFun : BasicObject
	{
		/// <summary>
		/// Prototype of all string objects
		/// </summary>
		public StringObj Prototype { get; }

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor
			| ObjectFeatures.Converter | ObjectFeatures.TypeReference;
		public override Type Type => typeof(string);

		public StringFun(IEngine engine, IObject baseClass, StringObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;

		public override Value Call(IObject self, int argc)
			=> argc == 0 ? new Value("") : new Value(Engine.GetArgument(argc).String);

		public override IObject Create(int argc)
			=> new StringObj(Engine, Prototype, argc == 0 ? "" : Engine.GetArgument(argc).String);

		public override IObject Convert(object value)
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
		public StringObj(IEngine engine, IObject baseClass)
			: base(engine, baseClass)
			=> String = "";

		/// <summary>
		/// Create new string object boxing the string
		/// </summary>
		public StringObj(IEngine engine, StringObj baseClass, string value)
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

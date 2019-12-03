using System;
using System.Collections.Generic;
using RedOnion.ROS.Functions;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Objects
{
	public class Globals : Namespace
	{
		public IProcessor Processor { get; set; }
		public UserObject System => parent;

		public Globals() : base("Globals", typeof(Globals)) { }

		public UserObject Prototype { get; protected set; }
		public UserObject Object { get; protected set; }
		public Function.Creator Function { get; protected set; }

		public Print Print { get; protected set; }
		public Run Run { get; protected set; }

		/// <summary>
		/// Called from Processor.SetGlobals and Processor.Reset
		/// (the second calls Globals.Reset first).
		/// </summary>
		public virtual void Fill()
		{
			if (readOnlyTop > 0)
				return;
			Prototype = new UserObject("Object.prototype");
			Add("System", parent = new UserObject(Prototype));

			System.Add("object", new Value(Object = new UserObject(Prototype)));
			Object.Add("prototype", Prototype);
			Object.Lock();
			System.Add("function", new Value(Function = new Function.Creator(Prototype)));

			System.Add("global", this);
			System.Add("globals", this);
			System.Add("math", typeof(RosMath));
			System.Add("print", new Value(Print = new Print(Prototype)));
			System.Add("run", new Value(Run = new Run(Prototype)));
			System.Add("list", new Value(typeof(List<Value>)));
			System.Add("queue", new Value(typeof(Queue<Value>)));
			System.Add("stack", new Value(typeof(Stack<Value>)));

			if (Processor is Processor processor)
			{
				System.Add("update", new Value(processor.Update));
				System.Add("idle", new Value(processor.Idle));
				System.Add("once", new Value(processor.Once));
			}

			System.Add("null", Value.Null);
			System.Add("true", Value.True);
			System.Add("false", Value.False);
			System.Add("nan", Value.NaN);
			System.Add("inf", double.PositiveInfinity);

			System.Add("double", new Value(Descriptor.Double));
			System.Add("float", new Value(Descriptor.Float));
			System.Add("long", new Value(Descriptor.Long));
			System.Add("ulong", new Value(Descriptor.ULong));
			System.Add("int", new Value(Descriptor.Int));
			System.Add("uint", new Value(Descriptor.UInt));
			System.Add("short", new Value(Descriptor.Short));
			System.Add("ushort", new Value(Descriptor.UShort));
			System.Add("sbyte", new Value(Descriptor.SByte));
			System.Add("byte", new Value(Descriptor.Byte));
			System.Add("char", new Value(Descriptor.Char));
			System.Add("string", new Value(Descriptor.String, null));

			System.Lock();
			Lock();
		}
		/// <summary>
		/// Called from Processor.Reset before calling Fill.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			System.Reset();
			Print?.Reset();
			Run?.Reset();
			Object?.Reset();
		}
	}
}

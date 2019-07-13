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

		protected Print print;
		protected Run run;
		protected UserObject obj;

		public virtual void Fill()
		{
			if (readOnlyTop > 0)
				return;
			Add("System", parent = new UserObject());
			System.Add("global", this);
			System.Add("math", typeof(RosMath));
			System.Add("print", new Value(print = new Print()));
			System.Add("run", new Value(run = new Run()));
			System.Add("object", new Value(obj = new UserObject()));
			System.Add("list", new Value(typeof(List<Value>)));

			if (Processor is Processor processor)
			{
				System.Add("update", new Value(Event.EventDescriptor.Instance, processor.Update));
				System.Add("idle", new Value(Event.EventDescriptor.Instance, processor.Update));
			}

			System.Add("null", Value.Null);
			System.Add("true", Value.True);
			System.Add("false", Value.False);

			System.Lock();
			Lock();
		}
		public override void Reset()
		{
			base.Reset();
			System.Reset();
			print?.Reset();
			run?.Reset();
			obj?.Reset();
		}
	}
}

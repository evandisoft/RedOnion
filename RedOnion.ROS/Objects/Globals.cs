using System;
using System.Collections.Generic;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Objects
{
	public class Globals : Namespace
	{
		public IProcessor Processor { get; set; }
		public UserObject System => parent;
		public Globals() : base("Globals", typeof(Globals)) { }
		public virtual void Fill()
		{
			if (readOnlyTop > 0)
				return;
			Add("System", parent = new UserObject());
			System.Add("print", new Value(new Print(Processor)));
			System.Add("object", new Value(new UserObject()));
			System.Add("list", new Value(typeof(List<Value>)));
			System.Add("math", typeof(Math));

			if (Processor is Processor processor)
			{
				System.Add("update", new Value(Event.EventDescriptor.Instance, processor.Update));
				System.Add("idle", new Value(Event.EventDescriptor.Instance, processor.Update));
			}
			System.Lock();
			Lock();
		}
		public override void Reset()
		{
			base.Reset();
			System.Reset();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public class Print : UserObject
	{
		public IProcessor Processor { get; set; }
		public Print(IProcessor processor) => Processor = processor;

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			if (args.Length == 0)
			{
				Processor?.Print("");
				result = "";
				return true;
			}
			var msg = args[0].ToStr();
			if (args.Length == 1)
			{
				Processor?.Print(msg);
				result = msg;
				return true;
			}
			var call = new string[args.Length-1];
			for (int i = 0; i < args.Length; i++)
				call[i] = args[i+1].ToStr();
			msg = string.Format(Value.Culture, msg, call);
			Processor?.Print(msg);
			result = msg;
			return true;
		}
	}
}

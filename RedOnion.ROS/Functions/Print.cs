using RedOnion.ROS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Functions
{
	public class Print : UserObject
	{
		public Print(UserObject baseClass) : base("Print", baseClass) { }

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			if (args.Length == 0)
			{
				args.Processor?.Print?.Invoke("");
				result = "";
				return true;
			}
			var msg = args[0].ToStr();
			if (args.Length == 1)
			{
				args.Processor?.Print?.Invoke(msg);
				result = msg;
				return true;
			}
			var call = new object[args.Length-1];
			for (int i = 1; i < args.Length; i++)
				call[i-1] = args[i].Box();
			msg = string.Format(Value.Culture, msg, call);
			args.Processor?.Print?.Invoke(msg);
			result = msg;
			return true;
		}
	}
}

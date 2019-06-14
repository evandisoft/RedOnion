using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public class Print : UserObject
	{
		protected Print() { }
		public static Print Instance { get; } = new Print();
		public static event Action<string> Listen;

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			if (args.Length == 0)
			{
				Listen?.Invoke("");
				result = "";
				return true;
			}
			var msg = args[0].ToStr();
			if (args.Length == 1)
			{
				Listen?.Invoke(msg);
				result = msg;
				return true;
			}
			var call = new string[args.Length-1];
			for (int i = 0; i < args.Length; i++)
				call[i] = args[i+1].ToStr();
			msg = string.Format(Value.Culture, msg, call);
			Listen?.Invoke(msg);
			result = msg;
			return true;
		}
	}
}

using System;
using RedOnion.Script;

namespace RedOnion.Script.BasicObjects
{
	public class PrintFun : BasicObject
	{
		public PrintFun(IEngine engine) : base(engine) { }
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, Arguments args)
		{
			if (args.Length == 0)
			{
				Engine.Print("");
				return "";
			}
			var msg = args[0].String;
			if (args.Length == 1)
			{
				Engine.Print(msg);
				return msg;
			}
			var call = new string[args.Length-1];
			for (int i = 0; i < args.Length; i++)
				call[i] = args[i+1].String;
			msg = string.Format(Value.Culture, msg, call);
			Engine.Print(msg);
			return msg;
		}
	}
}

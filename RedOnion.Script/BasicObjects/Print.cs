using System;
using RedOnion.Script;

namespace RedOnion.Script.BasicObjects
{
	public class PrintFun : BasicObject
	{
		public PrintFun(IEngine engine) : base(engine) { }
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
		{
			if (argc == 0)
			{
				Engine.Print("");
				return "";
			}
			var msg = Engine.GetArgument(argc, 0).String;
			if (argc == 1)
			{
				Engine.Print(msg);
				return msg;
			}
			var args = new string[argc-1];
			for (int i = 0; i < args.Length; i++)
				args[i] = Engine.GetArgument(argc, i+1).String;
			msg = string.Format(Value.Culture, msg, args);
			Engine.Print(msg);
			return msg;
		}
	}
}

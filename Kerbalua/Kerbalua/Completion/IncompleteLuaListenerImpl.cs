using System;
using Antlr4.Runtime.Misc;

namespace Kerbalua.Completion {
	public class IncompleteLuaListenerImpl : IncompleteLuaBaseListener {
		public IncompleteLuaParser.IncompleteVarContext LastIncompleteVar;

		//public override void EnterIncompleteName([NotNull] IncompleteLuaParser.IncompleteNameContext context)
		//{
		//	IncompleteName=context.NAME().ToString();
		//}

		public override void EnterIncompleteVar([NotNull] IncompleteLuaParser.IncompleteVarContext context)
		{
			LastIncompleteVar = context;
		}
	}
}

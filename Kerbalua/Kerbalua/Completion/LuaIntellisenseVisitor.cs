using System;
using Antlr4.Runtime.Misc;

namespace Kerbalua.Completion {
	public class LuaIntellisenseVisitor : BackwardsLuaBaseVisitor<string> {


		public override string VisitBackwardsArrayAccess([NotNull] BackwardsLuaParser.BackwardsArrayAccessContext context)
		{
			return base.VisitBackwardsArrayAccess(context);
		}

		public override string VisitBackwardsCall([NotNull] BackwardsLuaParser.BackwardsCallContext context)
		{
			return base.VisitBackwardsCall(context);
		}

		public override string VisitBackwardsCompletionExpr([NotNull] BackwardsLuaParser.BackwardsCompletionExprContext context)
		{
			if (context.backwardsPartialCompletion() != null) {

			}
			return base.VisitBackwardsCompletionExpr(context);
		}

		public override string VisitBackwardsField([NotNull] BackwardsLuaParser.BackwardsFieldContext context)
		{
			return base.VisitBackwardsField(context);
		}

		public override string VisitBackwardsPartialCompletion([NotNull] BackwardsLuaParser.BackwardsPartialCompletionContext context)
		{
			return base.VisitBackwardsPartialCompletion(context);
		}

		public override string VisitBackwardsStartSymbol([NotNull] BackwardsLuaParser.BackwardsStartSymbolContext context)
		{
			return base.VisitBackwardsStartSymbol(context);
		}

		public override string VisitCompletionChain([NotNull] BackwardsLuaParser.CompletionChainContext context)
		{
			return base.VisitCompletionChain(context);
		}

		public override string VisitExpr([NotNull] BackwardsLuaParser.ExprContext context)
		{
			return base.VisitExpr(context);
		}
	}
}

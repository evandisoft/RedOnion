using System;
using Grammar.IncompleteLuaParsing;

namespace Kerbalua.Completion
{
	public abstract class CompletionOperation
	{
		public override string ToString()
		{
			return GetType().Name;
		}
	}

	public class GetMemberOperation : CompletionOperation
	{
		public string Name;

		public GetMemberOperation(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return "Get(" + Name + ")";
		}
	}

	public class ArrayAccessOperation : CompletionOperation
	{
		internal IncompleteLuaParser.ExpContext exp;

		public override string ToString()
		{
			return "Array";
		}
	}

	public class CallOperation : CompletionOperation
	{
		public override string ToString()
		{
			return "Call";
		}
	}
}

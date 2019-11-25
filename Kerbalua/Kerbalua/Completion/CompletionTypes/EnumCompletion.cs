using System;
using System.Collections.Generic;

namespace Kerbalua.Completion.CompletionTypes
{
	public class EnumCompletion:CompletionObject
	{
		Type type;

		public EnumCompletion(Type type)
		{
			this.type=type;
		}

		public override IList<string> GetPossibleCompletions()
		{
			return type.GetEnumNames();
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
		}
	}
}

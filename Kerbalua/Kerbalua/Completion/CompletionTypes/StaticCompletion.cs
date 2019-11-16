using System;
using System.Collections.Generic;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class StaticCompletion : CompletionObject
    {
        private Type type;

        public StaticCompletion(Type type)
        {
            this.type = type;
        }

		public override IList<string> GetPossibleCompletions()
		{
			throw new NotImplementedException();
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
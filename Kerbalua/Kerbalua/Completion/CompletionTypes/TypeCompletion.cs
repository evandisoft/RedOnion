using System;
using System.Collections.Generic;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class TypeCompletion : CompletionObject
    {
        private Type type;

        public TypeCompletion(Type type)
        {
            this.type = type;
			
        }

		public override IList<string> StaticGetPossibleCompletions()
		{
			throw new System.NotImplementedException();
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new System.NotImplementedException();
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new System.NotImplementedException();
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
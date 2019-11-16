using System.Collections.Generic;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class CompletableCompletion : CompletionObject
    {
        private ICompletable completable;

        public CompletableCompletion(ICompletable completable)
        {
            this.completable = completable;
        }

		public override IList<string> GetPossibleCompletions()
		{
			return completable.PossibleCompletions;
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
			if (operations.Current is GetMemberOperation getMemberOperation)
			{
				if (completable.TryGetCompletion(getMemberOperation.Name, out object obj))
				{
					operations.MoveNext();
					completionObject=GetCompletionObject(obj);
					return true;
				}
			}
			completionObject=null;
			return false;
		}
	}
}
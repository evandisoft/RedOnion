using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class InstanceCompletion : InstanceStaticCompletion
    {
        private object v;

        public InstanceCompletion(object v):base(v.GetType())
        {
            this.v = v;
        }

		public override IList<string> StaticGetPossibleCompletions()
		{
			return CompletionReflectionUtil.GetMemberNames(v.GetType(), CompletionReflectionUtil.AllPublic);
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			return base.TryArrayAccess(operations, out completionObject);
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			return base.TryCall(operations, out completionObject);
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
using System;
using System.Collections.Generic;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion
{
	public class OperationsProcessorICompletable : OperationsProcessor
	{
		static OperationsProcessorICompletable instance;
		static public OperationsProcessorICompletable Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new OperationsProcessorICompletable();
				}
				return instance;
			}
		}

		public override IList<string> GetPossibleCompletions(object obj)
		{
			return ((ICompletable)obj).PossibleCompletions;
		}

		public override bool TryProcessArrayAccess(object obj, CompletionOperations operation, out object outObj)
		{
			throw new LuaIntellisenseException(nameof(ICompletable)+" does not handle Array Access");
		}

		public override bool TryProcessCall(object obj, CompletionOperations operations, out object outObj)
		{
			throw new LuaIntellisenseException(nameof(ICompletable) + " does not handle Calls");
		}

		public override bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj)
		{
			var completable = (ICompletable)obj;
			if(operations.Current is GetMemberOperation getMemberOperation)
			{
				if (completable.TryGetCompletion(getMemberOperation.Name,out outObj))
				{
					operations.MoveNext();
					return true;
				}
			}
			outObj = null;
			return false;
		}
	}
}

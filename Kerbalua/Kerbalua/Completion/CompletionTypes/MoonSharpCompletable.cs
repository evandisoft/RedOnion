using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using RedOnion.KSP.MoonSharp.Interfaces;

namespace Kerbalua.Completion.CompletionTypes
{
	public class MoonSharpCompletable:CompletionObject
	{
		IMoonSharpCompletable moonSharpCompletable;

		public MoonSharpCompletable(IMoonSharpCompletable moonSharpCompletable)
		{
			this.moonSharpCompletable=moonSharpCompletable;
		}

		public override IList<string> GetPossibleCompletions()
		{
			return moonSharpCompletable.PossibleCompletions;
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("ArrayAccess not implemented for MoonSharpCompletable");
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("Call not implemented for MoonSharpCompletable");
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			if (operations.Current is GetMemberOperation getMemberOperation)
			{
				if (moonSharpCompletable.TryGetCompletion(getMemberOperation.Name, out object obj))
				{
					operations.MoveNext();
					if (obj is Type t)
					{
						obj = UserData.CreateStatic(t);
					}
					completionObject=GetCompletionObject(obj);
					return true;
				}
			}
			completionObject=null;
			return false;
		}
	}
}

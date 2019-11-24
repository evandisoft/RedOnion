using System.Collections.Generic;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class HasProxyCompletion : CompletionObject
    {
        private IHasCompletionProxy hasProxy;

		public override string ToString()
		{
			return base.ToString()+"("+hasProxy?.GetType().Name+")";
		}

		public HasProxyCompletion(IHasCompletionProxy hasProxy)
        {
            this.hasProxy = hasProxy;
        }

		static public CompletionObject GetProxy(IHasCompletionProxy hasProxy, int limit)
		{
			object obj=hasProxy.CompletionProxy;
			int i = 0;
			for (i = 0; i < limit; i++)
			{
				if (obj is IHasCompletionProxy proxy)
				{
					obj = proxy.CompletionProxy;
					continue;
				}
				if (obj is DynValue dyn)
				{
					if (dyn.Type == DataType.UserData)
					{
						if (dyn.UserData is IHasCompletionProxy proxy2)
						{
							obj = proxy2.CompletionProxy;
							continue;
						}
					}
				}
				break;
			}
			if (i >= limit)
			{
				throw new LuaIntellisenseException("Could not resolve proxies in " + nameof(GetProxy));
			}
			return GetCompletionObject(obj);
		}

		public override bool TryOperation(CompletionOperations operations, out CompletionObject completionObject)
		{
			completionObject=GetProxy(hasProxy, 100);
			return true;
		}

		public override IList<string> GetPossibleCompletions()
		{
			var completionObject=GetProxy(hasProxy, 100);
			return completionObject.GetPossibleCompletions();
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("HasProxyCompletion does not implement TryArrayAccess");
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("HasProxyCompletion does not implement TryCall");
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("HasProxyCompletion does not implement TryGetMember");
		}
	}
}
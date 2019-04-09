using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion
{
	public abstract class OperationsProcessor
	{
		//public readonly Operations Operations;
		public abstract IList<string> GetPossibleCompletions(object obj);

		public abstract bool TryProcessGetMember(object obj, CompletionOperations operations,out object outObj);
		public abstract bool TryProcessCall(object obj, CompletionOperations operations, out object outObj);
		public abstract bool TryProcessArrayAccess(object obj, CompletionOperations operations, out object outObj);

		static public IList<string> StaticGetPossibleCompletions(object obj)
		{
			var processor = GetCompletionProcessor(obj, out object convertedObj);

			return processor.GetPossibleCompletions(convertedObj);
		}

		static public bool TryProcessOperation(object obj, CompletionOperations operations, out object outObj)
		{
			if (operations.IsFinished)
			{
				outObj = null;
				return false;
			}
			var processor = GetCompletionProcessor(obj,out object convertedObj);
			var operation = operations.Current;
			switch (operation)
			{
				case GetMemberOperation getMemberOperation:
					return processor.TryProcessGetMember(convertedObj, operations, out outObj);
				case CallOperation callOperation:
					return processor.TryProcessCall(convertedObj, operations, out outObj);
				case ArrayAccessOperation arrayAccessOperation:
					return processor.TryProcessArrayAccess(convertedObj, operations, out outObj);
			}
			throw new LuaIntellisenseException("Operation must be call, getmember, or arrayaccess");
		}

		//protected OperationsProcessor(Operations operations)
		//{
		//	Operations = operations;
		//}

		//protected List<string> FilterAndSortCompletions(IList<string> possibleCompletions)
		//{
		//	List<string> completions = new List<string>();

		//	if(Operations.Current is GetMemberOperation getMemberOperation)
		//	{
		//		string lowercasePartial = Operations.Name.ToLower();

		//		foreach (var possibleCompletion in possibleCompletions)
		//		{
		//			if (possibleCompletion.ToLower().Contains(lowercasePartial))
		//			{
		//				completions.Add(possibleCompletion);
		//			}
		//		}

		//		completions.Sort();
		//		return completions;
		//	}


		//}
		static public object GetProxy(object obj)
		{
			int limit = 100;
			int i = 0;
			for (i = 0; i < limit; i++)
			{
				if (obj is IHasCompletionProxy proxy)
				{
					obj = proxy.CompletionProxy;
				}
				else
				{
					break;
				}
			}
			if (i >= limit)
			{
				throw new LuaIntellisenseException("Could not resolve proxies in " + nameof(OperationsProcessor));
			}
			return obj;
		}

		static public OperationsProcessor GetCompletionProcessor(object obj,out object convertedObj)
		{
			obj = GetProxy(obj);

			if (obj is Instance instance)
			{
				convertedObj = instance.Type;
				return OperationsProcessorInstance.Instance;
			}

			if (obj is ICompletable completable)
			{
				convertedObj = completable;
				return OperationsProcessorICompletable.Instance;
			}

			if(obj is Table table)
			{
				convertedObj = table;
				return OperationsProcessorTable.Instance;
			}

			if (obj is DynValue dynValue)
			{
				if (dynValue.Type == DataType.Table)
				{
					convertedObj = dynValue.Table;
					return OperationsProcessorTable.Instance;
				}

				if (dynValue.Type == DataType.UserData)
				{
					if (dynValue.UserData.Object == null)
					{
						convertedObj = dynValue.UserData.Descriptor.Type;
						return OperationsProcessorStatic.Instance;
					}

					if(dynValue.UserData.Object is Type)
					{
						convertedObj = dynValue.UserData.Object.GetType();
						return OperationsProcessorType.Instance;
					}

					if(dynValue.UserData.Object is ICompletable)
					{
						convertedObj = dynValue.UserData.Object as ICompletable;
						return OperationsProcessorICompletable.Instance;
					}

					convertedObj = dynValue.UserData.Descriptor.Type;
					return OperationsProcessorInstance.Instance;
				}
				else
				{
					convertedObj = dynValue.ToObject().GetType();
					return OperationsProcessorInstance.Instance;
				}
			}
			if(obj is Type)
			{
				convertedObj = obj.GetType();
				return OperationsProcessorType.Instance;
			}


			convertedObj = obj.GetType();
			return OperationsProcessorInstance.Instance;
		}
	}
}

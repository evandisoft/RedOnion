using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion.CompletionTypes
{
	public abstract class CompletionObject
	{
		public virtual bool TryOperation(CompletionOperations operations, out CompletionObject completionObject)
		{
			CompletionQueue.Log("<Completion object is> "+GetType());
			var operation = operations.Current;
			CompletionQueue.Log("<Completion type is> "+operation.GetType());

			switch (operation)
			{
			case GetMemberOperation getMemberOperation:
				return TryGetMember(operations, out completionObject);

			case CallOperation callOperation:
				return TryCall(operations, out completionObject);

			case ArrayAccessOperation arrayAccessOperation:
				return TryArrayAccess(operations, out completionObject);
			}
			throw new LuaIntellisenseException("Operation must be call, getmember, or arrayaccess");
		}

		public abstract bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject);
		public abstract bool TryCall(CompletionOperations operations, out CompletionObject completionObject);
		public abstract bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject);
		public abstract IList<string> GetPossibleCompletions();

		public static CompletionObject GetCompletionObject(DynValue dynValue)
		{
			CompletionQueue.Log("converting dynvalue "+dynValue);
			if (dynValue==null)
			{
				CompletionQueue.Log("dynvalue is null -> null");
				return null;
			}

			if (dynValue.UserData==null)
			{
				CompletionQueue.Log("userdata is null -> Instance");
				return new InstanceCompletion(dynValue.ToObject());
			}

			if (dynValue.UserData.Object==null)
			{
				CompletionQueue.Log("Object is null -> Static");
				return new StaticCompletion(dynValue.ToObject() as Type);
			}

			object obj=dynValue.ToObject();

			return GetCompletionObject(obj);
		}

		public static CompletionObject GetCompletionObject(object obj)
		{
			CompletionQueue.Log("converting object "+obj);

			if (obj.GetType().Name=="RuntimeType")
			{
				CompletionQueue.Log("obj is runtimetype -> Type");
				return new TypeCompletion(obj as Type);
			}

			if (obj is Static static1)
			{
				CompletionQueue.Log("obj is Static -> Static");
				return new StaticCompletion(static1.type);
			}

			if (obj is ICompletable completable)
			{
				CompletionQueue.Log("obj is ICompletable");
				return new CompletableCompletion(completable);
			}

			if (obj is IHasCompletionProxy hasProxy)
			{
				CompletionQueue.Log("obj is IHasCompletionProxy");
				return new HasProxyCompletion(hasProxy);
			}

			if (obj is Table table)
			{
				CompletionQueue.Log("obj is Table -> Table");
				return new TableCompletion(table);
			}

			CompletionQueue.Log("obj is Instance -> Instance");
			return new InstanceCompletion(obj);
		}
	}
}

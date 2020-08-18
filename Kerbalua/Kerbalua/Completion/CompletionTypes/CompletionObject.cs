using System;
using System.Collections.Generic;
using MunSharp.Interpreter;
using RedOnion.Common.Completion;
using static RedOnion.Debugging.QueueLogger;

namespace Kerbalua.Completion.CompletionTypes
{
	public abstract class CompletionObject
	{
		public override string ToString()
		{
			return GetType().Name;
		}

		/// <summary>
		/// </summary>
		/// <returns><c>true</c>, if operation was tryed, <c>false</c> otherwise.</returns>
		/// <param name="operations">Operations.</param>
		/// <param name="completionObject">Completion object.</param>
		public virtual bool TryOperation(CompletionOperations operations, out CompletionObject completionObject)
		{
			//CompletionQueue.Log("<Completion object is> "+GetType());
			var operation = operations.Current;
			//CompletionQueue.Log("<Completion type is> "+operation.GetType());

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

		/// <summary>
		/// Get's the appropriate completion object from a DynValue
		/// </summary>
		/// <returns>The completion object.</returns>
		/// <param name="dynValue">Dyn value.</param>
		public static CompletionObject GetCompletionObject(DynValue dynValue)
		{
			Complogger.DebugLog("Converting dynvalue "+dynValue);
			if (dynValue==null)
			{
				Complogger.DebugLog("dynvalue is null -> null");
				return null;
			}

			if (dynValue.UserData!=null && dynValue.UserData.Object==null)
			{
				Complogger.DebugLog("Object is null -> Static");
				return new StaticCompletion(dynValue.ToObject() as Type);
			}

			if (dynValue.Table==null && dynValue.UserData==null)
			{
				Complogger.DebugLog("non-table lua type -> Instance");
				return new InstanceCompletion(dynValue.ToObject());
			}

			object obj=dynValue.ToObject();

			return GetCompletionObject(obj);
		}

		/// <summary>
		/// Get's the appropriate CompletionObject from an object
		/// </summary>
		/// <returns>The completion object.</returns>
		/// <param name="obj">Object.</param>
		public static CompletionObject GetCompletionObject(object obj)
		{
			Complogger.DebugLog("Converting object "+obj);
			if (obj is DynValue dynValue)
			{
				Complogger.DebugLog("obj is dynvalue, running dynvalue GetCompletionObject");
				return GetCompletionObject(dynValue);
			}


			if (obj is Type type)
			{
				Complogger.DebugLog("obj's a type and it's name is " + type.Name);
				if (type.IsEnum)
				{
					Complogger.DebugLog("obj is an enum -> EnumCompletion");
					return new InstanceStaticCompletion(type);
				}
				Complogger.DebugLog("obj is runtimetype -> RuntimeType");
				return new RuntimeTypeCompletion(obj as Type);
			}


			if (obj is InstanceStatic instance1)
			{
				Complogger.DebugLog("obj is Instance -> Instance " + instance1.type.Name);
				return new InstanceStaticCompletion(instance1.type);
			}

			if (obj is Static static1)
			{
				Complogger.DebugLog("obj is Static -> Static " + static1.type.Name);
				return new StaticCompletion(static1.type);
			}

			Complogger.DebugLog("obj has a type of " + obj.GetType().Name);
			if (obj is IMoonSharpCompletable m)
			{
				Complogger.DebugLog("obj is IMoonSharpCompletable");
				return new MoonSharpCompletable(m);
			}

			if (obj is ICompletable completable)
			{
				Complogger.DebugLog("obj is ICompletable");
				return new CompletableCompletion(completable);
			}

			if (obj is IHasCompletionProxy hasProxy)
			{
				Complogger.DebugLog("obj is IHasCompletionProxy");
				return new HasProxyCompletion(hasProxy);
			}

			if (obj is Table table)
			{
				Complogger.DebugLog("obj is Table -> Table");
				return new TableCompletion(table);
			}

			Complogger.DebugLog("obj is Instance -> Instance");
			return new InstanceCompletion(obj);
		}
	}
}

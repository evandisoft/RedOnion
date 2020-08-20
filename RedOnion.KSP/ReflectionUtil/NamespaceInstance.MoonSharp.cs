using System;
using System.Collections.Generic;
using MunSharp.Interpreter;
using MunSharp.Interpreter.Interop;
using RedOnion.Common.Completion;
using UnityEngine;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class NamespaceInstance : IUserDataType, IMoonSharpCompletable
	{
		public DynValue Index(global::MunSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			//Debug.Log(index);

			if (index.Type != DataType.String)
			{
				// Exceptions thrown from in here will get intercepted by interpreter and not shown
				throw new Exception("Type of index must be string for NamespaceInstance");
			}

			if (TryGetSubNamespace(index.String, out NamespaceInstance namespaceInstance))
			{
				return DynValue.FromObject(script,namespaceInstance);
			}

			if (TryGetType(index.String, out Type type))
			{
				return UserData.CreateStatic(type);
			}

			// Exceptions thrown from in here will get intercepted by interpreter and not shown
			throw new Exception("No type or subnamespace named " + index + " found in namespace " + ToString());
		}

		DynValue IUserDataType.MetaIndex(global::MunSharp.Interpreter.Script script, string metaname)
		{
			//if (metaname == "__tostring")
			//{
			//	return DynValue.FromObject(script, new CallbackFunction(ToString));
			//}
			//Debug.Log(metaname);
			return null;
		}

		//DynValue ToString(ScriptExecutionContext arg1, CallbackArguments args)
		//{
		//	return DynValue.FromObject(arg1.OwnerScript,ToString());
		//}


		bool IUserDataType.SetIndex(global::MunSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			throw new NotSupportedException("Cannot modify fields of "+nameof(NamespaceInstance));
		}

		IList<string> IMoonSharpCompletable.PossibleCompletions 
		{ 
			get 
			{
				return PossibleCompletions;
			} 
		}

		bool IMoonSharpCompletable.TryGetCompletion(string completionName, out object completion)
		{
			if(TryGetCompletion(completionName,out completion))
			{
				if (completion is Type type)
				{
					completion=UserData.CreateStatic(type);
				}
				return true;
			}

			completion = null;
			return false;
		}
	}
}

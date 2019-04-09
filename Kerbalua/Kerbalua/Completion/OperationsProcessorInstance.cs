using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kerbalua.Completion
{
	public class OperationsProcessorInstance : OperationsProcessorNative
	{
		static OperationsProcessorInstance instance;
		static public OperationsProcessorInstance Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new OperationsProcessorInstance();
				}
				return instance;
			}
		}

		public override IList<string> GetPossibleCompletions(object obj)
		{
			return base.GetPossibleCompletions(obj, CompletionReflectionUtil.AllPublic);
		}

		public override bool TryProcessArrayAccess(object obj, CompletionOperations operations, out object outObj)
		{
			Type t = obj as Type;
			if (CompletionReflectionUtil.TryGetProperty(t,"Item",out PropertyInfo propertyInfo))
			{
				outObj = new Instance(propertyInfo.PropertyType);
				operations.MoveNext();
				return true;
			}
			outObj = null;
			return false;
		}

		public override bool TryProcessCall(object obj, CompletionOperations operations, out object outObj)
		{
			Type t = obj as Type;
			if (CompletionReflectionUtil.TryGetMethod(t, "Invoke", out MethodInfo methodInfo))
			{
				outObj = new Instance(methodInfo.ReturnType);
				operations.MoveNext();
				return true;
			}
			outObj = null;
			return false;
		}

		public override bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj)
		{
			return base.TryProcessGetMember(obj, operations, out outObj, CompletionReflectionUtil.AllPublic);
		}
	}
}

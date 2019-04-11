using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kerbalua.Completion
{
	public abstract class OperationsProcessorNative : OperationsProcessor
	{
		public virtual IList<string> GetPossibleCompletions(object obj, BindingFlags flags)
		{
			Type t = obj as Type;
			return CompletionReflectionUtil.GetMembers(t,flags);
		}

		public virtual bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj, BindingFlags flags)
		{
			Type t = obj as Type;
			var getMember = operations.Current as GetMemberOperation;
			if (CompletionReflectionUtil.TryGetMethod(t, getMember.Name, out MethodInfo methodInfo, flags))
			{
				Type newType = methodInfo.ReturnType;
				var nextOp = operations.Peek(1);
				if (nextOp is CallOperation)
				{
					outObj = new Instance(newType);
					operations.Move(2);
					return true;
				}
				outObj = null;
				return false;
			}

			if (CompletionReflectionUtil.TryGetProperty(t, getMember.Name, out PropertyInfo propertyInfo, flags))
			{
				Type newType = propertyInfo.PropertyType;
				outObj = new Instance(newType);
				operations.MoveNext();
				return true;
			}

			if (CompletionReflectionUtil.TryGetField(t, getMember.Name, out FieldInfo fieldInfo, flags))
			{
				Type newType = fieldInfo.FieldType;
				outObj = new Instance(newType);
				operations.MoveNext();
				return true;
			}

			outObj = null;
			return false;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kerbalua.Completion.CompletionTypes
{
	/// <summary>
	/// Represents completion on an instance for which the object is not available.
	/// </summary>
	public class InstanceStaticCompletion:CompletionObject
	{
		Type type;

		public InstanceStaticCompletion(Type type)
		{
			this.type=type;
		}

		public override IList<string> GetPossibleCompletions()
		{
			throw new NotImplementedException();
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			if (CompletionReflectionUtil.TryGetProperty(type, "Item", out PropertyInfo propertyInfo))
			{
				completionObject=new InstanceStaticCompletion(propertyInfo.PropertyType);
				operations.MoveNext();
				return true;
			}
			completionObject = null;
			return false;
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			if (CompletionReflectionUtil.TryGetMethod(type, "Invoke", out MethodInfo methodInfo))
			{
				completionObject=new InstanceStaticCompletion(methodInfo.ReturnType);
				operations.MoveNext();
				return true;
			}
			completionObject = null;
			return false;
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			return CompletionReflectionUtil.TryGetNativeMember(type, operations, out completionObject, CompletionReflectionUtil.AllPublic);
		}
	}
}

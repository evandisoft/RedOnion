using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kerbalua.Completion
{
	public class OperationsProcessorType : OperationsProcessorNative
	{
		static OperationsProcessorType instance;
		static public OperationsProcessorType Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new OperationsProcessorType();
				}
				return instance;
			}
		}

		public override IList<string> GetPossibleCompletions(object obj)
		{
			return base.GetPossibleCompletions(obj,CompletionReflectionUtil.AllPublic);
		}

		public override bool TryProcessArrayAccess(object obj, CompletionOperations operation, out object outObj)
		{
			throw new LuaIntellisenseException("For " + nameof(OperationsProcessorType) + " ArrayAccess operation is invalid");
		}

		public override bool TryProcessCall(object obj, CompletionOperations operations, out object outObj)
		{
			throw new LuaIntellisenseException("For " + nameof(OperationsProcessorType) + " Call operation is invalid");
		}

		public override bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj)
		{
			return base.TryProcessGetMember(obj, operations, out outObj, CompletionReflectionUtil.AllPublic);
		}
	}
}

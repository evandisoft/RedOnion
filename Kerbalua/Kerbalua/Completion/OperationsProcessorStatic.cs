using System;
using System.Collections.Generic;
using System.Reflection;
using MoonSharp.Interpreter;

namespace Kerbalua.Completion
{
	public class OperationsProcessorStatic : OperationsProcessorNative
	{
		static OperationsProcessorStatic instance;
		static public OperationsProcessorStatic Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new OperationsProcessorStatic();
				}
				return instance;
			}
		}

		public override IList<string> GetPossibleCompletions(object obj)
		{
			return base.GetPossibleCompletions(obj, CompletionReflectionUtil.StaticPublic);
		}

		public override bool TryProcessArrayAccess(object obj, CompletionOperations operation, out object outObj)
		{
			throw new LuaIntellisenseException("For " + nameof(OperationsProcessorStatic) + " ArrayAccess operation is invalid");
		}

		public override bool TryProcessCall(object obj, CompletionOperations operations, out object outObj)
		{
			throw new LuaIntellisenseException("For " + nameof(OperationsProcessorStatic) + " Call operation is invalid");
		}

		public override bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj)
		{
			return base.TryProcessGetMember(obj, operations, out outObj, CompletionReflectionUtil.StaticPublic);
		}
	}
}

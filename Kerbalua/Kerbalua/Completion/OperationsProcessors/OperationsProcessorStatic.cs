using System;
using System.Collections.Generic;
using System.Reflection;
using MoonSharp.Interpreter;
using RedOnion.ROS;

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

		public override bool TryProcessArrayAccess(object obj, CompletionOperations operations, out object outObj)
		{
			throw new LuaIntellisenseException("For " + nameof(OperationsProcessorStatic) + " ArrayAccess operation is invalid");
		}

		public override bool TryProcessCall(object obj, CompletionOperations operations, out object outObj)
		{
			throw new LuaIntellisenseException("For " + nameof(OperationsProcessorStatic) + " Call operation is invalid");
		}

		public override bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj)
		{
			Type t = obj as Type;
			var getMember = operations.Current as GetMemberOperation;
			CompletionQueue.Log("type is "+t+", member name is "+getMember.Name);
			if (CompletionReflectionUtil.TryGetField(t, getMember.Name, out FieldInfo fieldInfo, CompletionReflectionUtil.StaticPublic))
			{
				//Type newType = fieldInfo.FieldType;
				//Static field access can be completed as an object.
				outObj = fieldInfo.GetValue(null);
				CompletionQueue.Log("static field access");
				operations.MoveNext();
				return true;
			}

			if (t.GetCustomAttribute<NamespaceAttribute>()!=null)
			{
				if (CompletionReflectionUtil.TryGetProperty(t, getMember.Name, out PropertyInfo propertyInfo, CompletionReflectionUtil.StaticPublic))
				{
					outObj = propertyInfo.GetValue(null);
					CompletionQueue.Log("static property access");
					operations.MoveNext();
					return true;
				}
			}

			return base.TryProcessGetMember(obj, operations, out outObj, CompletionReflectionUtil.StaticPublic);

			//
		}
	}
}

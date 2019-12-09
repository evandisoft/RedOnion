using System;
using System.Collections.Generic;
using System.Reflection;
using MoonSharp.Interpreter;
using RedOnion.Attributes;
using RedOnion.ROS;
using static MunOS.Debugging.QueueLogger;

namespace Kerbalua.Completion.CompletionTypes
{
	/// <summary>
	/// Instance completion.
	/// </summary>
    internal class InstanceCompletion : InstanceStaticCompletion
    {
        protected object obj;

		public override string ToString()
		{
			return base.ToString()+"("+obj?.GetType().Name+")";
		}

		public InstanceCompletion(object obj):base(obj.GetType())
        {
            this.obj = obj;
        }

		public override IList<string> GetPossibleCompletions()
		{
			return CompletionReflectionUtil.GetMemberNames(obj.GetType(), CompletionReflectionUtil.AllPublic);
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			return base.TryArrayAccess(operations, out completionObject);
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			return base.TryCall(operations, out completionObject);
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			//EvanPotential: This will be replaced by dynamically accessing actual field values
			// and also dynamically accessing property values but only if the class indicates,
			// with an attribute, that the properties are safe and fast and have no side effects. 

			Type type=obj.GetType();
			var getMember = operations.Current as GetMemberOperation;
			Complogger.Log("type is "+type+", member name is "+getMember.Name);
			if (CompletionReflectionUtil.TryGetField(type, getMember.Name, out FieldInfo fieldInfo, CompletionReflectionUtil.AllPublic))
			{
				if (fieldInfo.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					completionObject=null;
					return false;
				}
				//Type newType = fieldInfo.FieldType;
				//Static field access can be completed as an object.
				var fieldObj = fieldInfo.GetValue(obj);
				if (fieldObj==null)
				{
					completionObject=new InstanceStaticCompletion(type);
					return true;
				}
				completionObject=GetCompletionObject(fieldObj);
				Complogger.Log("instance field access");
				operations.MoveNext();
				return true;
			}

			// This can be active when we have agreed on an attribute to use.
			if (type.GetCustomAttribute<SafeProps>()!=null)
			{
				if (CompletionReflectionUtil.TryGetProperty(type, getMember.Name, out PropertyInfo propertyInfo, CompletionReflectionUtil.AllPublic))
				{
					if (propertyInfo.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
					{
						completionObject=null;
						return false;
					}
					var propObj = propertyInfo.GetValue(obj);
					if (propObj==null)
					{
						completionObject=new InstanceStaticCompletion(type);
						return true;
					}
					completionObject=GetCompletionObject(propObj);
					Complogger.Log("instance safe property access");
					operations.MoveNext();
					return true;
				}
			}

			return CompletionReflectionUtil.TryGetNativeMember(obj.GetType(), operations, out completionObject, CompletionReflectionUtil.AllPublic);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Reflection;
using RedOnion.KSP.Attributes;
using RedOnion.ROS;
using static RedOnion.KSP.Debugging.QueueLogger;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class StaticCompletion : CompletionObject
    {
        private Type type;

		public override string ToString()
		{
			return base.ToString()+"("+type?.Name+")";
		}

		public StaticCompletion(Type type)
        {
            this.type = type;
        }

		public override IList<string> GetPossibleCompletions()
		{
			return CompletionReflectionUtil.GetMemberNames(type, CompletionReflectionUtil.StaticPublic);
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("StaticCompletion TryArrayAccess is not implemented");
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("StaticCompletion TryCall is not implemented");
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			//EvanPotential: Could allow completion for nested types.

			var getMember = operations.Current as GetMemberOperation;
			Complogger.Log("type is "+type+", member name is "+getMember.Name);
			if (CompletionReflectionUtil.TryGetField(type, getMember.Name, out FieldInfo fieldInfo, CompletionReflectionUtil.StaticPublic))
			{
				//Type newType = fieldInfo.FieldType;
				//Static field access can be completed as an object.
				var obj = fieldInfo.GetValue(null);
				if (obj==null)
				{
					completionObject=new InstanceStaticCompletion(type);
					return true;
				}
				completionObject=GetCompletionObject(obj);
				Complogger.Log("static field access");
				operations.MoveNext();
				return true;
			}

			// This can be active when we have agreed on an attribute to use for classes that have
			// safe properties
			if (type.GetCustomAttribute<SafeProps>()!=null)
			{
				if (CompletionReflectionUtil.TryGetProperty(type, getMember.Name, out PropertyInfo propertyInfo, CompletionReflectionUtil.StaticPublic))
				{
					var obj = propertyInfo.GetValue(null);
					if (obj==null)
					{
						completionObject=new InstanceStaticCompletion(type);
						return true;
					}
					completionObject=GetCompletionObject(obj);
					Complogger.Log("static property access");
					operations.MoveNext();
					return true;
				}
			}

			return CompletionReflectionUtil.TryGetNativeMember(type, operations, out completionObject, CompletionReflectionUtil.StaticPublic);
		}
	}
}
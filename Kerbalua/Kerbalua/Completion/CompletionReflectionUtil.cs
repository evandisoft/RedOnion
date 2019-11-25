using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Kerbalua.Completion.CompletionTypes;
using MoonSharp.Interpreter;

namespace Kerbalua.Completion
{
	public class CompletionReflectionUtil
	{
		static HashSet<string> HiddenMethodNames=new HashSet<string>{"Equals","GetHashCode","GetType","ToString"};
		static public IList<string> GetMemberNames(Type t,BindingFlags flags=BindingFlags.Default)
		{
			var strs = new HashSet<string>();

			foreach (var type in t.GetNestedTypes())
			{
				//Complogger.Log("getting type "+type);
				if (type.IsSpecialName)
					continue;
				strs.Add(type.Name);
			}

			foreach (var ev in t.GetEvents())
			{
				if (ev.IsSpecialName)
					continue;
				strs.Add(ev.Name);
			}

			foreach (var field in t.GetFields(flags))
			{
				if (field.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					continue;
				}
				//strs.Add("field "+field.Name+" "+field.IsSpecialName);
				strs.Add(field.Name);
			}
			foreach (var property in t.GetProperties(flags))
			{
				if (property.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					continue;
				}
				if (property.Name != "Item")
				{
					//strs.Add("property " + property.Name + " " + property.IsSpecialName);
					strs.Add(property.Name);
				}
			}
			foreach (var method in t.GetMethods(flags))
			{
				if (HiddenMethodNames.Contains(method.Name))
				{
					continue;
				}
				if (method.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					continue;
				}
				if (!method.IsSpecialName)
				{
					//strs.Add("method " + method.Name + " " + method.IsSpecialName);
					strs.Add(method.Name);
				}
			}
			//foreach (var member in t.GetMembers(flags))
			//{
			//	if (member.Name.Contains("_"))
			//	{
			//		strs.Add(member.Name.Split('_')[1]);

			//	}
			//	else
			//	{
			//		strs.Add(member.Name);
			//	}
			//}
			return strs.ToList();
		}

		static public bool TryGetNestedType(Type t, string name, out Type type, BindingFlags flags = AllPublic)
		{
			type = t.GetNestedType(name, flags);
			return type != null;
		}

		static public bool TryGetEvent(Type t, string name, out EventInfo eventInfo, BindingFlags flags = AllPublic)
		{
			eventInfo= t.GetEvent(name, flags);
			return eventInfo != null;
		}

		static public bool TryGetField(Type t, string name, out FieldInfo fieldInfo, BindingFlags flags = AllPublic)
		{
			fieldInfo = t.GetField(name,flags);
			return fieldInfo != null;
		}

		static public bool TryGetArrayAccess(Type t, out Type outType, BindingFlags flags = AllPublic)
		{
			foreach(var methodInfo in t.GetMethods())
			{
				if (methodInfo.Name=="get_Item")
				{
					outType=methodInfo.ReturnType;
					return true;
				}
			}

			outType=null;
			return false;
		}

		static public bool TryGetProperty(Type t, string name, out PropertyInfo propertyInfo, BindingFlags flags = AllPublic)
		{
			propertyInfo = t.GetProperty(name, flags);
			return propertyInfo != null;
		}

		static public bool TryGetMethod(Type t, string name, out MethodInfo methodInfo, BindingFlags flags = AllPublic)
		{
			methodInfo = null;
			try
			{
				methodInfo = t.GetMethod(name, flags);
			}
			catch (AmbiguousMatchException)
			{
				MethodInfo[] mis = t.GetMethods(flags);
				foreach (var mi in mis)
				{
					if (mi.Name == name)
					{
						methodInfo = mi;
						break;
					}
				}
			}

			return methodInfo != null;
		}

		public static bool TryGetNativeMember(Type type, CompletionOperations operations, out CompletionObject completionObject, BindingFlags flags)
		{
			var getMember = operations.Current as GetMemberOperation;
			if (TryGetNestedType(type, getMember.Name, out Type nestedType, flags))
			{
				operations.MoveNext();
				if (nestedType.IsEnum)
				{
					completionObject=new EnumCompletion(nestedType);
					return true;
				}
				completionObject=new StaticCompletion(nestedType);
				return true;
			}

			if (TryGetMethod(type, getMember.Name, out MethodInfo methodInfo, flags))
			{
				if (methodInfo.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					completionObject=null;
					return false;
				}
				Type newType = methodInfo.ReturnType;
				var nextOp = operations.Peek(1);
				if (nextOp is CallOperation)
				{
					completionObject = new InstanceStaticCompletion(newType);
					operations.Move(2);
					return true;
				}
			 	completionObject = null;
				return false;
			}

			if (TryGetProperty(type, getMember.Name, out PropertyInfo propertyInfo, flags))
			{
				if (propertyInfo.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					completionObject=null;
					return false;
				}
				Type newType = propertyInfo.PropertyType;
				completionObject = new InstanceStaticCompletion(newType);
				operations.MoveNext();
				return true;
			}

			if (TryGetField(type, getMember.Name, out FieldInfo fieldInfo, flags))
			{
				if (fieldInfo.GetCustomAttribute<MoonSharpHiddenAttribute>()!=null)
				{
					completionObject=null;
					return false;
				}
				Type newType = fieldInfo.FieldType;
				completionObject = new InstanceStaticCompletion(newType);
				operations.MoveNext();
				return true;
			}

			completionObject = null;
			return false;
		}

		public const BindingFlags StaticPublic = BindingFlags.Static | BindingFlags.Public;
		public const BindingFlags AllPublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
	}
}

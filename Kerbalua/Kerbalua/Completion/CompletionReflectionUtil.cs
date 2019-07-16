using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kerbalua.Completion
{
	public class CompletionReflectionUtil
	{
		static public IList<string> GetMemberNames(Type t,BindingFlags flags=BindingFlags.Default)
		{
			var strs = new HashSet<string>();
			foreach (var field in t.GetFields(flags))
			{
				//strs.Add("field "+field.Name+" "+field.IsSpecialName);
				strs.Add(field.Name);
			}
			foreach (var property in t.GetProperties(flags))
			{
				if (property.Name != "Item")
				{
					//strs.Add("property " + property.Name + " " + property.IsSpecialName);
					strs.Add(property.Name);
				}
			}
			foreach (var method in t.GetMethods(flags))
			{
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

		static public bool TryGetField(Type t, string name, out FieldInfo fieldInfo, BindingFlags flags = AllPublic)
		{
			fieldInfo = t.GetField(name,flags);
			return fieldInfo != null;
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

		public const BindingFlags StaticPublic = BindingFlags.Static | BindingFlags.Public;
		public const BindingFlags AllPublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
	}
}

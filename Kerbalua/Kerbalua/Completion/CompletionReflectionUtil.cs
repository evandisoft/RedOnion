using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kerbalua.Completion
{
	public class CompletionReflectionUtil
	{
		static public IList<string> GetMembers(Type t,BindingFlags flags=BindingFlags.Default)
		{
			var strs = new HashSet<string>();
			foreach (var member in t.GetMembers(flags))
			{
				if (member.Name.Contains("_"))
				{
					strs.Add(member.Name.Split('_')[1]);
				}
				else
				{
					strs.Add(member.Name);
				}
			}
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

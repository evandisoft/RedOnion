using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion
{
	public class TypeCompletable : ICompletable
	{
		Type type;
		Segment segment;

		public TypeCompletable(Type type, Segment segment)
		{
			this.type = type;
			this.segment = segment;
		}

		public IList<string> PossibleCompletions
		{
			get
			{
				var strs = new HashSet<string>();
				foreach (var member in type.GetMembers())
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
		}

		public bool TryGetCompletion(string completionName, out object completion)
		{
			IList<Part> parts = segment.Parts;
			if (parts.Count > 0 && parts[0] is CallPart)
			{
				MethodInfo mi = null;
				try
				{
					mi = type.GetMethod(completionName);

				}
				catch (AmbiguousMatchException)
				{
					MethodInfo[] mis = type.GetMethods();
					foreach (var methodInfo in mis)
					{
						if (methodInfo.Name == completionName)
						{
							mi = methodInfo;
							break;
						}
					}
				}
				if (mi != null)
				{
					return ProcessCurrentParts(mi.ReturnType,1,out completion);
				}
			}

			var p = type.GetProperty(completionName);
			if (p != null)
			{
				return ProcessCurrentParts(p.PropertyType,0,out completion);
			}
			var f = type.GetField(completionName);
			if (f == null)
			{
				completion = null;
				return false;
			}

			return ProcessCurrentParts(f.FieldType, 0, out completion);
		}

		bool ProcessCurrentParts(Type currentType,int startIndex,out object completion)
		{
			IList<Part> parts = segment.Parts;

			for (int i = 0; i < parts.Count; i++)
			{
				if (parts[i] is CallPart)
				{
					MethodInfo mi = currentType.GetMethod("Invoke");
					if (mi == null)
					{
						completion = null;
						return false;
					}
					currentType = currentType.GetMethod("Invoke").ReturnType;
				}
				else if (parts[i] is ArrayPart)
				{
					PropertyInfo pi = currentType.GetProperty("Item");
					if (pi == null)
					{
						completion = null;
						return false;
					}
					currentType = pi.PropertyType;
				}
			}

			completion = currentType;
			return true;
		}
	}
}

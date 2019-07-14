using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace RedOnion.Build
{
	static class Documentation
	{
		class Document
		{
			public Type type;
			public string name;
			public string path;
			public string desc;
			public ListCore<MemberInfo> members;
		}
		static Dictionary<string, Document> docs = new Dictionary<string, Document>();
		static Dictionary<Type, Document> types = new Dictionary<Type, Document>();
		static HashSet<Type> discovered = new HashSet<Type>();
		static Dictionary<Type, Type> obj2fn = new Dictionary<Type, Type>();
		static Dictionary<Type, Type> fn2obj = new Dictionary<Type, Type>();

		static IEnumerable<MemberInfo> GetMembers(Type type)
		{
			var members = type.GetMembers(BindingFlags.Public|BindingFlags.Instance);
			if (members.Length > 1)
				Array.Sort(members, Descriptor.Reflected.MemberComparer.Instance);
			foreach (var member in members)
				yield return member;
			members = type.GetMembers(BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy);
			if (members.Length > 1)
				Array.Sort(members, Descriptor.Reflected.MemberComparer.Instance);
			foreach (var member in members)
				yield return member;
		}
		static string GetName(ICustomAttributeProvider member)
			=> Descriptor.Reflected.GetName(member);

		static ListCore<Type> queue = new ListCore<Type>();
		static void RegisterType(Type type)
		{
			if (type.Assembly != typeof(Globals).Assembly)
				return;
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			if (discovered.Add(type))
				queue.Add(type);
		}
		internal static void Exec()
		{
			RegisterType(typeof(Globals));
			var members = new Dictionary<string, MemberInfo>();
			for (int i = 0; i < queue.size; i++)
			{
				var type = queue[i];
				var desc = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
				if (desc == null)
					continue;
				var name = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
				var full = type.FullName;
				if (full.StartsWith("RedOnion.KSP."))
					full = full.Substring("RedOnion.KSP.".Length);
				var creator = type.GetCustomAttribute<CreatorAttribute>();
				if (creator != null)
				{
					obj2fn[type] = creator.Creator;
					fn2obj[creator.Creator] = type;
				}

				var doc = new Document()
				{
					type = type,
					name = name,
					path = "RedOnion.KSP/" + string.Join("/", full.Split('.')),
					desc = desc
				};
				docs.Add(name, doc);
				types.Add(type, doc);
				foreach (var member in GetMembers(type))
				{
					if (member.GetCustomAttribute<DescriptionAttribute>() == null)
						continue;
					var mname = GetName(member);
					members.TryGetValue(mname, out var prev);
					if (prev != null && !(prev is MethodInfo && member is MethodInfo))
						continue;
					if (member is FieldInfo f)
						RegisterType(f.FieldType);
					else if (member is PropertyInfo p)
						RegisterType(p.PropertyType);
					else if (member is MethodInfo m)
					{
						RegisterType(m.ReturnType);
						foreach (var mp in m.GetParameters())
							RegisterType(mp.ParameterType);
					}
					else continue;
					doc.members.Add(member);
					if (prev == null)
						members.Add(mname, member);
				}
				members.Clear();
			}
			foreach (var doc in docs.Values)
			{
				if (fn2obj.ContainsKey(doc.type))
					continue;
				Document cdoc = null;
				if (obj2fn.TryGetValue(doc.type, out var ctype))
					cdoc = types[ctype];
				using (var file = new FileStream(doc.path + ".md", FileMode.Create))
				using (var wr = new StreamWriter(file))
				{
					if (cdoc != null)
					{
						wr.WriteLine("## " + doc.name);
						Print(wr, cdoc, doc.name);
						wr.WriteLine();
					}
					wr.WriteLine("## " + doc.name);
					Print(wr, doc, doc.name);
				}
			}
		}

		class MetaCmp : IComparer<MemberInfo>
		{
			public static readonly MetaCmp It = new MetaCmp();
			public int Compare(MemberInfo x, MemberInfo y)
				=> x.MetadataToken.CompareTo(y.MetadataToken);
		}
		static void Print(StreamWriter wr, Document doc, string name)
		{
			wr.WriteLine();
			wr.WriteLine(doc.desc);
			wr.WriteLine();
			doc.members.Sort(MetaCmp.It);
			foreach (var member in doc.members)
			{
				var mname = GetName(member);
				if (member is FieldInfo f)
				{
					PrintSimpleMember(wr, doc, mname, member, f.FieldType);
					continue;
				}
				if (member is PropertyInfo p)
				{
					PrintSimpleMember(wr, doc, mname, member, p.PropertyType);
					continue;
				}
				if (member is MethodInfo m)
				{
					// for now
					PrintSimpleMember(wr, doc, mname, member, m.ReturnType);
					continue;
				}
			}
		}

		static void PrintSimpleMember(StreamWriter wr, Document doc, string name, MemberInfo member, Type type)
		{
			string typePath = null;
			string typeName = type.Name;
			var desc = member.GetCustomAttribute<DescriptionAttribute>().Description;
			if (types.TryGetValue(type, out var tdoc))
			{
				typeName = tdoc.name;
				if (type != doc.type)
				{
					if (fn2obj.TryGetValue(tdoc.type, out var objtype))
					{
						tdoc = docs[objtype.Name];
						typeName = objtype.Name + " Function";
					}
					typePath = GetRelativePath(doc.path, tdoc.path) + ".md";
				}
			}
			if (member is MethodInfo || typeof(ICallable).IsAssignableFrom(type))
				name = name + "()";
			else if (member is PropertyInfo p && p.GetIndexParameters().Length > 0)
				name = "[index]";
			wr.WriteLine(typePath == null
				? "- `{0}`: {1} - {3}"
				: "- `{0}`: [{1}]({2}) - {3}",
				name, typeName, typePath, desc);
		}

		static string GetRelativePath(string fromPath, string toPath)
		{
			var fromUri = new Uri(Path.GetFullPath(fromPath));
			var toUri = new Uri(Path.GetFullPath(toPath));
			var relativeUri = fromUri.MakeRelativeUri(toUri);
			return Uri.UnescapeDataString(relativeUri.ToString());
		}
	}
}

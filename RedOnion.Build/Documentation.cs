using RedOnion.KSP.API;
using RedOnion.KSP.Attributes;
using RedOnion.KSP.Utilities;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace RedOnion.Build
{
	static class Documentation
	{
		static string unsafeMark = "(Unsafe) {0}";

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
		static Dictionary<Type, Type> redirects = new Dictionary<Type, Type>();
		static Dictionary<Type, string> typeNames = new Dictionary<Type, string>()
		{
			{ typeof(void), "void" },
			{ typeof(string), "string" },
			{ typeof(double), "double" },
			{ typeof(float), "float" },
			{ typeof(int), "int" },
			{ typeof(uint), "uint" },
			{ typeof(long), "long" },
			{ typeof(ulong), "ulong" },
			{ typeof(short), "short" },
			{ typeof(ushort), "ushort" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(byte), "byte" },
			{ typeof(bool), "bool" },
		};

		// Compare by RID (record ID)
		class MetaCmp : IComparer<MemberInfo>
		{
			public static readonly IComparer<MemberInfo> It = new MetaCmp();
			static bool ByType<T>(MemberInfo x, MemberInfo y, ref int cmp) where T : MemberInfo
			{
				cmp = x is T ? y is T
					? x.MetadataToken.CompareTo(y.MetadataToken)
					: -1 : y is T ? +1 : 0;
				return cmp == 0;
			}
			public int Compare(MemberInfo x, MemberInfo y)
			{
				int cmp = 0;
				return ByType<ConstructorInfo>(x, y, ref cmp)
				&& ByType<FieldInfo>(x, y, ref cmp)
				&& ByType<EventInfo>(x, y, ref cmp)
				&& ByType<PropertyInfo>(x, y, ref cmp)
				&& ByType<MethodInfo>(x, y, ref cmp)
				? x.MetadataToken.CompareTo(y.MetadataToken)
				: cmp;
			}
		}

		static IEnumerable<MemberInfo> GetMembers(Type type)
		{
			var members = type.GetMembers(BindingFlags.Public|BindingFlags.Instance);
			if (members.Length > 1)
				Array.Sort(members, MetaCmp.It);
			foreach (var member in members)
				yield return member;
			members = type.GetMembers(BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy);
			if (members.Length > 1)
				Array.Sort(members, MetaCmp.It);
			foreach (var member in members)
				yield return member;
		}
		static string GetName(ICustomAttributeProvider member)
			=> Descriptor.Reflected.GetName(member);

		static ListCore<Type> queue = new ListCore<Type>();
		static void RegisterType(Type type)
		{
			var docb = type.GetCustomAttribute<DocBuildAttribute>();
			if (docb != null && docb.AsType != null)
			{
				redirects[type] = docb.AsType;
				type = docb.AsType;
			}
			if (type.Assembly != typeof(Globals).Assembly
				&& type.Assembly != typeof(RedOnion.UI.Element).Assembly)
				return;
			var fullType = type;
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			if (type.IsGenericParameter || type.FullName == null)
				return;
			var desc = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
			if (desc != null && discovered.Add(type))
				queue.Add(type);
			if (fullType != type)
			{
				foreach (var gen in fullType.GetGenericArguments())
					RegisterType(gen);
			}
			if (docb != null && docb.RegisterTypes?.Length > 0)
			{
				foreach (var rtype in docb.RegisterTypes)
					RegisterType(rtype);
			}
		}
		internal static void Exec()
		{
			Descriptor.Reflected.LowerFirstLetter = false;
			RegisterType(typeof(Globals));
			RegisterType(typeof(KSP.MoonSharp.MoonSharpAPI.MoonSharpGlobals));
			var members = new Dictionary<string, MemberInfo>();
			for (int i = 0; i < queue.size; i++)
			{
				var type = queue[i];
				var desc = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
				var name = type.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName
					?? type.Name.Replace('`', '.');
				var path = "";
				var full = type.FullName;
				if (full.StartsWith("RedOnion.KSP."))
				{
					full = full.Substring("RedOnion.KSP.".Length);
					path = "RedOnion.KSP/";
				}
				else if (full.StartsWith("RedOnion.UI."))
				{
					full = full.Substring("RedOnion.UI.".Length);
					path = "RedOnion.UI/";
				}
				path += string.Join("/", full.Split('.')).Replace('`', '.');
				var docb = type.GetCustomAttribute<DocBuildAttribute>();
				if (docb != null && !string.IsNullOrEmpty(docb.Path))
					path = docb.Path;
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
					path = path,
					desc = desc
				};
				docs.Add(name, doc);
				types.Add(type, doc);
				foreach (var member in GetMembers(type))
				{
					FieldInfo f = member as FieldInfo;
					bool typeRef = f != null
						&& f.IsStatic && f.IsInitOnly	// static readonly
						&& f.FieldType == typeof(Type);	// Type name = ...
					if (member.GetCustomAttribute<DescriptionAttribute>() == null && (!typeRef ||
						((Type)f.GetValue(null)).GetCustomAttribute<DescriptionAttribute>() == null))
						continue;
					var mname = GetName(member);
					members.TryGetValue(mname, out var prev);
					if (prev != null && !(prev is MethodInfo && member is MethodInfo))
						continue;
					if (f != null)
						RegisterType(
							f.FieldType == typeof(Type)
							&& f.IsInitOnly && f.IsStatic
							? (Type)f.GetValue(null)
							: f.FieldType);
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
						wr.WriteLine("## {0} (Function)", doc.name);
						Print(wr, cdoc, doc.name);
						wr.WriteLine();
					}
					wr.WriteLine(cdoc == null ? "## {0}" : "## {0} (Instance)", doc.name);
					Print(wr, doc, doc.name);
				}
			}
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
					PrintSimpleMember(wr, doc, mname, member,
						f.FieldType == typeof(Type)
						&& f.IsStatic && f.IsInitOnly
						? (Type)f.GetValue(null)
						: f.FieldType);
					continue;
				}
				if (member is PropertyInfo p)
				{
					PrintSimpleMember(wr, doc, mname, member, p.PropertyType);
					continue;
				}
				if (member is MethodInfo m)
				{
					PrintMethod(wr, doc, mname, m);
					continue;
				}
			}
		}

		static string ResolveType(Document doc, Type type, out string name)
		{
			string path = null;
			var fullType = type;
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			if (redirects.TryGetValue(type, out var redir))
			{
				fullType = type = redir;
				if (type.IsGenericType)
					type = type.GetGenericTypeDefinition();
			}
			if (!typeNames.TryGetValue(type, out name))
				name = type.Name;
			if (types.TryGetValue(type, out var tdoc))
			{
				name = tdoc.name;
				if (type != doc.type)
				{
					if (fn2obj.TryGetValue(tdoc.type, out var objtype))
					{
						tdoc = docs[objtype.Name];
						name = objtype.Name;
					}
					path = GetRelativePath(doc.path, tdoc.path) + ".md";
				}
			}
			if (type == fullType)
				return path == null ? name : string.Format("[{0}]({1})", name, path);
			var sb = new StringBuilder();
			name = name.Substring(0, name.LastIndexOfAny(new char[] { '.', '`' }));
			if (path == null) sb.Append(name);
			else sb.AppendFormat("[{0}]({1})", name, path);
			sb.Append("\\[");
			var first = true;
			foreach (var gen in fullType.GetGenericArguments())
			{
				if (!first)
					sb.Append(", ");
				first = false;
				sb.Append(ResolveType(doc, gen, out _));
			}
			sb.Append("\\]");
			return sb.ToString();
		}

		static void PrintSimpleMember(StreamWriter wr, Document doc, string name, MemberInfo member, Type type)
		{
			var desc = member.GetCustomAttribute<DescriptionAttribute>()?.Description
				?? type.GetCustomAttribute<DescriptionAttribute>()?.Description;
			var typeMd = ResolveType(doc, type, out var typeName);
			if (member is MethodInfo || typeof(ICallable).IsAssignableFrom(type))
				name += "()";
			else if (member is PropertyInfo p)
			{
				var pars = p.GetIndexParameters();
				if (pars.Length > 0)
				{
					var sb = new StringBuilder();
					sb.Append("[");
					for (int i = 0; i < pars.Length; i++)
					{
						if (i > 0)
							sb.Append(", ");
						sb.Append(pars[i].Name);
						sb.Append(" ");
						var pmd = ResolveType(doc, pars[i].ParameterType, out _);
						sb.Append(pmd);
					}
					sb.Append("]");
					name = sb.ToString();
				}
			}
			wr.WriteLine("- `{0}`: {1} - {2}", name, typeMd,
				member.IsDefined(typeof(UnsafeAttribute)) ?
				string.Format(unsafeMark, desc) : desc);
		}

		static void PrintMethod(StreamWriter wr, Document doc, string name, MethodInfo method)
		{
			var desc = method.GetCustomAttribute<DescriptionAttribute>().Description;
			var type = method.ReturnType;
			var typeMd = ResolveType(doc, type, out _);
			var pars = method.GetParameters();
			wr.Write("- `{0}()`: {1}", name, typeMd);
			foreach (var par in pars)
			{
				type = par.ParameterType;
				typeMd = ResolveType(doc, type, out _);
				wr.Write(", {0} {1}", par.Name, typeMd);
			}
			if (pars.Length > 0)
				wr.WriteLine();
			wr.WriteLine(pars.Length == 0 ? " - {0}" : "  - {0}",
				method.IsDefined(typeof(UnsafeAttribute)) ?
				string.Format(unsafeMark, desc) : desc);
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

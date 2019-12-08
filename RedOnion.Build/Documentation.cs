using RedOnion.Attributes;
using RedOnion.ROS;
using RedOnion.Collections;
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
		static string unsafeMark = "\\[`Unsafe`\\] {0}";
		static string wipMark = "\\[`WIP`\\] {0}";
		static HashSet<Assembly> assemblies = new HashSet<Assembly>()
		{
			typeof(RedOnion.KSP.API.Globals).Assembly,
			typeof(RedOnion.UI.Element).Assembly
		};
		static Type[] rootTypes = new Type[]
		{
			typeof(RedOnion.KSP.API.Globals),
			typeof(KSP.MoonSharp.MoonSharpAPI.MoonSharpGlobals)
		};
		const BindingFlags iflags = BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly;
		const BindingFlags sflags = BindingFlags.Static|BindingFlags.Public|BindingFlags.DeclaredOnly;

		readonly struct Member<Info>
		{
			public readonly Info info;
			public readonly string desc;
			public Member(Info info, string desc)
			{
				this.info = info;
				this.desc = desc;
			}
		}

		class Document
		{
			public Type type;
			public string name;
			public string path;
			public string desc;

			public Document baseClass;
			public ListCore<Document> derived;
			public ListCore<Member<MemberInfo>> nested;
			public ListCore<Member<ConstructorInfo>> ctors;
			public ListCore<Member<FieldInfo>> ifields;
			public ListCore<Member<FieldInfo>> sfields;
			public ListCore<Member<PropertyInfo>> iprops;
			public ListCore<Member<PropertyInfo>> sprops;
			public ListCore<Member<EventInfo>> ievents;
			public ListCore<Member<EventInfo>> sevents;
			public ListCore<Member<MethodInfo>> imethods;
			public ListCore<Member<MethodInfo>> smethods;

			public void Fill<Info>(ref ListCore<Member<Info>> list, Info[] infos) where Info : MemberInfo
			{
				if (infos.Length > 1)
					Array.Sort(infos, MetaCmp.It);
				foreach (var info in infos)
				{
					var desc = info.GetCustomAttribute<DescriptionAttribute>()?.Description;
					if (info is FieldInfo f)
					{
						if (f.IsStatic && f.IsInitOnly  // static readonly
						&& f.FieldType == typeof(Type)) // Type name = ...
						{
							var nested = (Type)f.GetValue(null);
							if (desc == null)
								desc = nested.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
							if (desc == null)
								continue;
							this.nested.Add(new Member<MemberInfo>(f, desc));
							RegisterType(nested);
							continue;
						}
					}
					if (desc != null)
						list.Add(new Member<Info>(info, desc));
					if (info is PropertyInfo p)
						RegisterType(p.PropertyType);
					else if (info is MethodBase m)
					{
						if (m is MethodInfo mi)
							RegisterType(mi.ReturnType);
						foreach (var mp in m.GetParameters())
							RegisterType(mp.ParameterType);
					}
				}
			}
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
			public int Compare(MemberInfo x, MemberInfo y)
			{
				var idx1 = x.GetCustomAttribute<DocIndexAttribute>()?.Index ?? -1;
				var idx2 = y.GetCustomAttribute<DocIndexAttribute>()?.Index ?? -1;
				if (idx1 >= 0)
				{
					if (idx1 != idx2)
						return idx2 < 0 ? -1 : idx2 - idx1;
				}
				else if (idx2 >= 0) return +1;
				return x.MetadataToken.CompareTo(y.MetadataToken);
			}
		}

		static string GetName(ICustomAttributeProvider provider)
		{
			string name = provider is MemberInfo member ? member is ConstructorInfo ci
					? ci.DeclaringType.Name : member.Name
					: provider is Type type ? type.Name : null;
			var displayName = provider.GetCustomAttributes(typeof(DisplayNameAttribute), !(provider is Type));
			if (displayName.Length == 1)
				return ((DisplayNameAttribute)displayName[0]).DisplayName;
			return name;
		}

		static ListCore<Type> queue = new ListCore<Type>();
		static void RegisterType(Type type)
		{
			if (type == null)
				return;
			if (!assemblies.Contains(type.Assembly))
				return;
			var docb = type.GetCustomAttribute<DocBuildAttribute>();
			if (docb != null && docb.AsType != null)
			{
				redirects[type] = docb.AsType;
				type = docb.AsType;
			}
			var fullType = type;
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			if (type.IsGenericParameter || type.FullName == null)
				return;
			var desc = type.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
			if (desc == null || !discovered.Add(type))
				return;
			RegisterType(type.BaseType);
			if (fullType != type)
			{
				foreach (var gen in fullType.GetGenericArguments())
					RegisterType(gen);
			}
			foreach (var nested in type.GetNestedTypes())
				RegisterType(nested);
			queue.Add(type);
			if (docb != null && docb.RegisterTypes?.Length > 0)
			{
				foreach (var rtype in docb.RegisterTypes)
					RegisterType(rtype);
			}
		}
		internal static void Exec()
		{
			Descriptor.Reflected.LowerFirstLetter = false;
			foreach (var rootType in rootTypes)
				RegisterType(rootType);
			for (int i = 0; i < queue.size; i++)
			{
				var type = queue[i];
				var desc = type.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
				var name = type.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName
					?? (type.DeclaringType == null ? type.Name :
					type.FullName.Substring(type.Namespace.Length + 1).Replace('+', '.') // nested type
					).Replace('`', '.');
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
				path += string.Join("/", full.Split('.')).Replace('`', '.').Replace('+', '.');
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
				if (type.BaseType != null)
					(doc.baseClass = ResolveType(type.BaseType))?.derived.Add(doc);
				doc.Fill(ref doc.ctors, type.GetConstructors());
				doc.Fill(ref doc.ifields, type.GetFields(iflags));
				doc.Fill(ref doc.sfields, type.GetFields(sflags));
				doc.Fill(ref doc.iprops, type.GetProperties(iflags));
				doc.Fill(ref doc.sprops, type.GetProperties(sflags));
				doc.Fill(ref doc.ievents, type.GetEvents(iflags));
				doc.Fill(ref doc.sevents, type.GetEvents(sflags));
				doc.Fill(ref doc.imethods, type.GetMethods(iflags));
				doc.Fill(ref doc.smethods, type.GetMethods(sflags));
				foreach (var nested in type.GetNestedTypes())
				{
					var ndoc = ResolveType(nested);
					if (ndoc != null)
						doc.nested.Add(new Member<MemberInfo>(ndoc.type, null));
				}
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
						wr.WriteLine("## {0} (Function)", AddMarks(doc.name,
							notsafe: doc.type.IsDefined(typeof(UnsafeAttribute)),
							wip: doc.type.IsDefined(typeof(WorkInProgressAttribute))));
						Print(wr, cdoc, doc.name);
						wr.WriteLine();
					}
					wr.WriteLine(cdoc == null ? "## {0}" : "## {0} (Instance)", AddMarks(doc.name,
						notsafe: doc.type.IsDefined(typeof(UnsafeAttribute)),
						wip: doc.type.IsDefined(typeof(WorkInProgressAttribute))));
					Print(wr, doc, doc.name);
				}
			}
		}

		static void Print(StreamWriter wr, Document doc, string name)
		{
			if (doc.baseClass != null)
			{
				wr.WriteLine();
				wr.WriteLine("**Base Class:** " + ResolveType(doc, doc.type.BaseType, out _));
			}
			if (doc.derived.Count > 0)
			{
				wr.WriteLine();
				wr.Write("**Derived:** ");
				bool first = true;
				foreach (var derived in doc.derived)
				{
					if (!first)
						wr.Write(", ");
					wr.Write(ResolveType(doc, derived.type, out _));
					first = false;
				}
				wr.WriteLine();
			}

			wr.WriteLine();
			wr.WriteLine(doc.desc);
			wr.WriteLine();

			if (doc.nested.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Types:**");
				foreach (var nested in doc.nested)
					PrintSimple(wr, doc, nested, nested.info as Type ?? (Type)((FieldInfo)nested.info).GetValue(null));
			}
			if (doc.ctors.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Constructors:**");
				foreach (var m in doc.ctors)
					PrintMethod(wr, doc, m);
			}
			if (doc.ifields.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Instance Fields:**");
				foreach (var m in doc.ifields)
					PrintSimple(wr, doc, m, m.info.FieldType);
			}
			if (doc.sfields.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Static Fields:**");
				foreach (var m in doc.sfields)
					PrintSimple(wr, doc, m, m.info.FieldType);
			}
			if (doc.iprops.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Instance Properties:**");
				foreach (var m in doc.iprops)
					PrintSimple(wr, doc, m, m.info.PropertyType);
			}
			if (doc.sprops.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Static Properties:**");
				foreach (var m in doc.sprops)
					PrintSimple(wr, doc, m, m.info.PropertyType);
			}
			if (doc.ievents.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Instance Events:**");
				foreach (var m in doc.ievents)
					PrintSimple(wr, doc, m, m.info.EventHandlerType);
			}
			if (doc.sevents.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Static Events:**");
				foreach (var m in doc.sevents)
					PrintSimple(wr, doc, m, m.info.EventHandlerType);
			}
			if (doc.imethods.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Instance Methods:**");
				foreach (var m in doc.imethods)
					PrintMethod(wr, doc, m);
			}
			if (doc.smethods.Count > 0)
			{
				wr.WriteLine();
				wr.WriteLine("**Static Methods:**");
				foreach (var m in doc.smethods)
					PrintMethod(wr, doc, m);
			}
		}

		static Document ResolveType(Type type)
		{
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			if (redirects.TryGetValue(type, out var redir))
			{
				type = redir;
				if (type.IsGenericType)
					type = type.GetGenericTypeDefinition();
			}
			return types.TryGetValue(type, out var tdoc) ? tdoc : null;

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

		static void PrintSimple<Info>(StreamWriter wr, Document doc, Member<Info> member, Type type) where Info : MemberInfo
		{
			var desc = member.desc;
			var typeMd = ResolveType(doc, type, out var typeName);
			var name = GetName(member.info);
			if (member.info is MethodInfo || typeof(ICallable).IsAssignableFrom(type))
				name += "()";
			else if (member.info is PropertyInfo p)
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
			wr.WriteLine(desc == null ? "- `{0}`: {1}" : "- `{0}`: {1} - {2}",
				name, typeMd, AddMarks(desc,
				notsafe: member.info.IsDefined(typeof(UnsafeAttribute)),
				wip: member.info.IsDefined(typeof(WorkInProgressAttribute))));
		}
		static string AddMarks(string desc, bool notsafe, bool wip)
		{
			if (wip)
				desc = string.Format(wipMark, desc);
			if (notsafe)
				desc = string.Format(unsafeMark, desc);
			return desc;
		}

		static void PrintMethod<Info>(StreamWriter wr, Document doc, Member<Info> member) where Info : MethodBase
		{
			var desc = member.desc;
			var method = member.info as MethodInfo;
			var type = method?.ReturnType;
			var typeMd = type != null ? ResolveType(doc, type, out _) : null;
			var name = GetName(member.info);
			wr.Write(typeMd == null ? "- `{0}()`" : "- `{0}()`: {1}", name, typeMd);
			var pars = member.info.GetParameters();
			var first = true;
			foreach (var par in pars)
			{
				type = par.ParameterType;
				typeMd = ResolveType(doc, type, out _);
				wr.Write(first && method == null ? ": {0} {1}" : ", {0} {1}", par.Name, typeMd);
				first = false;
			}
			if (pars.Length > 0)
				wr.WriteLine();
			wr.WriteLine(pars.Length == 0 ? " - {0}" : "  - {0}",
				AddMarks(desc, notsafe: member.info.IsDefined(typeof(UnsafeAttribute)),
				wip: member.info.IsDefined(typeof(WorkInProgressAttribute))));
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

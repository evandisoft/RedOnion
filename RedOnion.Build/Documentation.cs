using RedOnion.KSP.API;
using RedOnion.Script;
using System;
using System.Collections.Generic;
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
			public MemberList members;

			public bool HasFeature(ObjectFeatures feature)
				=> (members.Features & feature) != 0;
		}
		static Dictionary<string, Document> docs = new Dictionary<string, Document>();
		static Dictionary<Type, Document> types = new Dictionary<Type, Document>();
		static HashSet<Type> proxied = new HashSet<Type>();
		static HashSet<Type> discovered = new HashSet<Type>();
		static Dictionary<Type, Type> obj2fn = new Dictionary<Type, Type>();
		static Dictionary<Type, Type> fn2obj = new Dictionary<Type, Type>();
		internal static void Exec()
		{
			foreach (var type in typeof(GlobalMembers).Assembly.GetTypes())
			{
				if (type.IsDefined(typeof(IgnoreForDocsAttribute)))
					continue;
				var proxy = type.GetCustomAttribute<ProxyDocsAttribute>();
				if (proxy != null)
				{
					proxied.Add(proxy.ForType);
					discovered.Add(type);
					continue;
				}
				bool found = false;
				foreach (var face in type.GetInterfaces())
				{
					if (face == typeof(IType))
					{
						found = true;
						break;
					}
				}
				if (found)
				{
					discovered.Add(type);
					var creator = type.GetCustomAttribute<CreatorAttribute>();
					if (creator != null)
					{
						obj2fn[type] = creator.Creator;
						fn2obj[creator.Creator] = type;
					}
				}
			}
			foreach (var type in proxied)
				discovered.Remove(type);
			foreach (var type in discovered)
			{
				var doctype = type;
				var proxy = type.GetCustomAttribute<ProxyDocsAttribute>();
				if (proxy != null) doctype = proxy.ForType;
				var name = doctype.Name;
				var full = doctype.FullName.Substring("RedOnion.KSP.".Length);
				MemberList members;

				var getMembers = type.GetProperty("MemberList",
					BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);

				if (getMembers != null)
					members = (MemberList)getMembers.GetValue(null);
				else
				{
					var instance = doctype.GetProperty("Instance",
					BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);
					if (instance == null || !instance.CanRead)
					{
						if (proxy == null)
							continue;
						instance = type.GetProperty("Instance",
						BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);
						if (instance == null || !instance.CanRead)
							continue;
					}
					members = ((IType)instance.GetValue(null)).Members;
				}
				var doc = new Document()
				{
					type = doctype,
					name = name,
					path = "RedOnion.KSP/" + string.Join("/", full.Split('.')),
					members = members
				};
				docs.Add(name, doc);
				types.Add(doctype, doc);
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
						wr.WriteLine(
							cdoc.HasFeature(ObjectFeatures.Function)
							? "## {0} Function" :
							cdoc.HasFeature(ObjectFeatures.Constructor)
							? "## {0} Constructor"
							: "## {0}", doc.name);
						Print(wr, cdoc, doc.name);
						wr.WriteLine();
					}
					wr.WriteLine("## " + doc.name);
					Print(wr, doc, doc.name);
				}
			}
			void Print(StreamWriter wr, Document doc, string name)
			{
				wr.WriteLine();
				wr.WriteLine(doc.members.Help);
				wr.WriteLine();
				foreach (var member in doc.members)
				{
					string typePath = null;
					string typeName = member.Type;
					if (member.Type != name
						&& docs.TryGetValue(member.Type, out var tdoc))
					{
						if (fn2obj.TryGetValue(tdoc.type, out var objtype))
						{
							tdoc = docs[objtype.Name];
							typeName = objtype.Name + " Function";
						}
						typePath = GetRelativePath(doc.path, tdoc.path) + ".md";
					}
					wr.WriteLine(typePath == null
						? "- `{0}`: {1} - {3}"
						: "- `{0}`: [{1}]({2}) - {3}",
						member is Method ? member.Name + "()" : member.Name,
						typeName, typePath, member.Help);
				}
			}
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

using RedOnion.KSP.API;
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
			public string name;
			public string path;
			public string help;
			public IDictionary<string, IMember> members;
		}
		static Dictionary<string, Document> types = new Dictionary<string, Document>();
		static HashSet<Type> proxied = new HashSet<Type>();
		static HashSet<Type> discovered = new HashSet<Type>();
		internal static void Exec()
		{
			foreach (var type in typeof(GlobalMembers).Assembly.GetTypes())
			{
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
				if (found) discovered.Add(type);
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
				string help = null;
				IDictionary<string, IMember> members = null;

				var getHelp = type.GetProperty("Help",
					BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);
				var getMembers = type.GetProperty("Members",
					BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);

				if (getHelp != null && getMembers != null)
				{
					help = (string)getHelp.GetValue(null);
					members = (IDictionary<string, IMember>)getMembers.GetValue(null);
				}
				else
				{
					var instance = type.GetProperty("Instance",
					BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);
					if (instance == null || !instance.CanRead) continue;
					var it = (IType)instance.GetValue(null);
					help = it.Help;
					members = it.Members;
				}
				types.Add(name, new Document()
				{
					name = name,
					path = "RedOnion.KSP/" + string.Join("/", full.Split('.')),
					help = help,
					members = members
				});
			}
			foreach (var doc in types.Values)
			{
				using (var file = new FileStream(doc.path + ".md", FileMode.Create))
				using (var wr = new StreamWriter(file))
				{
					wr.WriteLine("## " + doc.name);
					wr.WriteLine();
					wr.WriteLine(doc.help);
					wr.WriteLine();
					foreach (var member in doc.members)
					{
						var m = member.Value;
						string typePath = null;
						if (types.TryGetValue(m.Type, out var tdoc))
							typePath = GetRelativePath(doc.path, tdoc.path) + ".md";
						wr.WriteLine(typePath == null
							? "- `{0}`: {1} - {3}"
							: "- `{0}`: [{1}]({2}) - {3}",
							member.Key, m.Type, typePath, m.Help);
					}
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

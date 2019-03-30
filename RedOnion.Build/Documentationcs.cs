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
			public IType desc;
			public string path;
			public string name;
			public string help;
		}
		static Dictionary<string, Document> types = new Dictionary<string, Document>();
		internal static void Exec()
		{
			foreach (var type in typeof(Globals).Assembly.GetTypes())
			{
				bool found = false;
				foreach (var face in type.GetInterfaces())
				{
					if (face == typeof(IType))
					{
						found = true;
						break;
					}
				}
				if (!found) continue;
				var instance = type.GetProperty("Instance",
					BindingFlags.Public|BindingFlags.Static|BindingFlags.GetProperty);
				if (instance == null || !instance.CanRead) continue;
				var it = instance.GetValue(null) as IType;
				var name = it.GetType().Name;
				var full = it.GetType().FullName.Substring("RedOnion.KSP.".Length);
				types.Add(name, new Document()
				{
					desc = it,
					path = "RedOnion.KSP/" + string.Join("/", full.Split('.')),
					name = name,
					help = it.Help
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
					foreach (var member in doc.desc.Members)
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

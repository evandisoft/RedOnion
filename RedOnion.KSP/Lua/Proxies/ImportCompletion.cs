using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.Lua.Proxies
{
	public class ImportCompletion : IUserDataType
	{
		public readonly string CurrentNamespace;

		//Dictionary<string, List<Type>> CompletionDictionary;
		public ImportCompletion(string currentNamespace)
		{
			CurrentNamespace = currentNamespace;

		}

		public ImportCompletion NextImportCompletion(string nextNamespacePart)
		{
			return new ImportCompletion(CurrentNamespace + "." + nextNamespacePart);
		}

		public List<string> GetCompletions()
		{
			var completions=AppDomain.CurrentDomain.GetAssemblies()
				.Select((assembly) => assembly.GetName().Name).ToList();
			completions.Sort();
			return completions;
		}

		public DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			throw new NotImplementedException();
		}

		public bool SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			throw new NotSupportedException("Attempted modification of ImportTable");
		}

		public DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			throw new NotImplementedException();
		}
	}
}

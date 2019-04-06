using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.ReflectionUtil;

namespace RedOnion.KSP.API
{
	public partial class NamespaceAPI : NamespaceInstance, IUserDataType
	{
		public NamespaceAPI(string namespaceString, NamespaceMappings namespaceMappings) : base(namespaceString, namespaceMappings)
		{
		}

		public DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type != DataType.String)
			{
				throw new Exception("index type for NamespaceAPI " + NamespaceString + " must be string");
			}

			if(TryGetSubNamespace(index.String,out NamespaceInstance namespaceInstance))
			{
				return DynValue.FromObject(script,namespaceInstance);
			}

			if(TryGetType(index.String,out Type type))
			{
				return DynValue.FromObject(script, type);
			}

			throw new Exception(index.String + " was not found in namespace " + NamespaceString);
		}

		public DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			return null;
		}

		public bool SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			throw new NotSupportedException("Modifying index of "+nameof(NamespaceAPI)+" "+NamespaceString+" is not supported.");
		}
	}
}

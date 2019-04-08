using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class NamespaceInstance : IUserDataType
	{
		public DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type != DataType.String)
			{
				throw new Exception("Type of index must be string for NamespaceInstance");
			}

			if (TryGetSubNamespace(index.String, out NamespaceInstance namespaceInstance))
			{
				return DynValue.FromObject(script,namespaceInstance);
			}

			if (TryGetType(index.String, out Type type))
			{
				return DynValue.FromObject(script,type);
			}

			throw new Exception("No type or subnamespace named " + index + " found in namespace " + NamespaceString);
		}

		public DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			throw new NotImplementedException();
		}

		public bool SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.ReflectionUtil;

namespace RedOnion.KSP.Lua
{
	public class NamespaceInstanceLua : NamespaceInstance, IUserDataType
	{
		public NamespaceInstanceLua(string namespaceString, NamespaceMappings namespaceMappings) : base(namespaceString, namespaceMappings)
		{
		}

		public DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type != DataType.String)
			{
				throw new Exception("index for NameSpaceInstanceLua must be a string");
			}

			if(TryGetSubNamespace(index.String,out NamespaceInstance namespaceInstance))
			{

			}
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

using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class GetMappings : IUserDataType
	{
		public DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type != DataType.String)
			{
				throw new Exception("index must be a string ");
			}

			if(TryGetAssemblyNamespaceInstance(index.String,out NamespaceInstance namespaceInstance))
			{
				return DynValue.FromObject(script,namespaceInstance);
			}

			throw new Exception("index not found");
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

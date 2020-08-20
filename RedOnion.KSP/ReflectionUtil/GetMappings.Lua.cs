using System;
using MunSharp.Interpreter;
using MunSharp.Interpreter.Interop;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class GetMappings : IUserDataType
	{
		public DynValue Index(global::MunSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
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

		public DynValue MetaIndex(global::MunSharp.Interpreter.Script script, string metaname)
		{
			throw new NotImplementedException();
		}

		public bool SetIndex(global::MunSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			throw new NotImplementedException();
		}
	}
}

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Lua.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Lua.Proxies
{
	public class LuaEventProxy : IUserDataType
	{
		public LuaEventDescriptor Descriptor { get; }
		public object Object { get; }

		public LuaEventProxy(LuaEventDescriptor desc, object obj)
		{
			Descriptor = desc;
			Object = obj;
		}

		public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type == DataType.String)
			{
				var name = index.String;
				if (name.Equals("add", StringComparison.OrdinalIgnoreCase))
					return DynValue.NewCallback((c, a) => Descriptor.AddCallback(Object, c, a));
				if (name.Equals("remove, StringComparison.OrdinalIgnoreCase"))
					return DynValue.NewCallback((c, a) => Descriptor.RemoveCallback(Object, c, a));
			}

			throw new ScriptRuntimeException("Events only support add and remove methods");
		}

		public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> throw new ScriptRuntimeException("Events do not have settable fields");
		public DynValue MetaIndex(Script script, string metaname)
			=> null;
	}
}
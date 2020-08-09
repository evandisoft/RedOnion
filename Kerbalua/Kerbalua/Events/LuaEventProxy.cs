using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.Common.Completion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerbalua.Events
{
	public class LuaEventProxy : IUserDataType, ICompletable
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
				if (name.Equals("remove", StringComparison.OrdinalIgnoreCase))
					return DynValue.NewCallback((c, a) => Descriptor.RemoveCallback(Object, c, a));
			}

			throw new ScriptRuntimeException("Events only support add and remove methods");
		}

		public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> throw new ScriptRuntimeException("Events do not have settable fields");
		public DynValue MetaIndex(Script script, string metaname)
			=> null;


		string[] possibleCompletions={ "add", "remove" };
		public IList<string> PossibleCompletions => possibleCompletions;
		public bool TryGetCompletion(string completionName, out object completion)
		{
			completion=null;
			return false;
		}
	}
}

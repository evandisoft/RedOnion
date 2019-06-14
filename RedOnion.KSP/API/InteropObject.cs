using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public abstract class InteropObject : InteropDescriptor, IUserDataType, IType
	{
		public MemberList Members { get; }
		public bool ReadOnly { get; protected set; }

		public InteropObject(MemberList members)
			: base("interop object", typeof(InteropObject))
			=> Members = members;

		public virtual DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (Members.TryGetValue(index.String, out var member) && member.CanRead)
				return member.LuaGet(this);
			return null;
		}

		public virtual bool SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			if (ReadOnly)
				return false;
			if (Members.TryGetValue(index.String, out var member) && member.CanWrite)
			{
				member.LuaSet(this, value);
				return true;
			}
			return false;
		}

		public virtual DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			var result = Value.Void;
			var self = args.ToRos(out var ros);
			return Call(ref result, self, ros, false) ? result.ToLua() : DynValue.Void;
		}
		public virtual DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			//if (metaname == "__call")
			//	DynValue.FromObject(script, new CallbackFunction(Call));
			return null;
		}
	}
}

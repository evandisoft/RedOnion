using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public abstract class InteropObject : IObject, IUserDataType, IType
	{
		public MemberList Members { get; }
		public ObjectFeatures Features => Members.Features;
		public bool ReadOnly { get; protected set; }

		public InteropObject(MemberList members)
			=> Members = members;

		public virtual bool Has(string name)
			=> Members.Contains(name);
		public Value Get(string name)
		{
			if (!Get(name, out var value))
				throw new NotImplementedException(name + " does not exist");
			return value;
		}
		public virtual bool Get(string name, out Value value)
		{
			if (Members.TryGetValue(name, out var member) && member.CanRead)
			{
				value = member.RosGet(this);
				return true;
			}
			value = new Value();
			return false;
		}

		public virtual bool Set(string name, Value value)
		{
			if (ReadOnly)
				return false;
			if (Members.TryGetValue(name, out var member) && member.CanWrite)
			{
				member.RosSet(this, value);
				return true;
			}
			return false;
		}

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

		public virtual Value Call(IObject self, Arguments args)
			=> Value.Void;
		public virtual IObject Create(Arguments args)
			=> null;
		DynValue IUserDataType.MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			if (metaname != "__call") return null;
			return DynValue.FromObject(script, new CallbackFunction(Call));
		}
		public virtual DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			=> DynValue.Void;

		bool IObject.Modify(string name, OpCode op, Value value) => false;
		bool IProperties.Delete(string name) => false;
		void IProperties.Reset() { }

		Value IObject.Index(Arguments args)
			=> throw new NotImplementedException();
		Value IObject.IndexGet(Value index)
			=> throw new NotImplementedException();
		bool IObject.IndexSet(Value index, Value value)
			=> throw new NotImplementedException();
		bool IObject.IndexModify(Value index, OpCode op, Value value)
			=> throw new NotImplementedException();
		bool IObject.Operator(OpCode op, Value arg, bool selfRhs, out Value result)
			=> throw new NotImplementedException();

		string IObject.Name => GetType().Name;
		Value IObject.Value => new Value(ToString());
		IEngine IObject.Engine => null;
		IObject IObject.BaseClass => null;
		IProperties IObject.BaseProps => null;
		IProperties IObject.MoreProps => null;
		Type IObject.Type => null;
		object IObject.Target => null;
		IObject IObject.Convert(object value) => null;
	}
}

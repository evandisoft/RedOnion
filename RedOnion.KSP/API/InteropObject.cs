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
		public virtual DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			var self = args.ToRos(out var ros);
			return Call(self, ros).ToLua();
		}

		public virtual string Name => GetType().Name;
		public virtual Value Value => new Value(ToString());
		public virtual Type Type => null;
		public virtual object Target => null;
		public virtual IObject Convert(object value) => null;

		public virtual bool Operator(OpCode op, Value arg, bool selfRhs, out Value result)
		{
			result = new Value();
			return false;
		}
		public bool HasFeature(ObjectFeatures feature)
			=> (Features & feature) != 0;
		DynValue IUserDataType.MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			if (metaname == "__call")
				return HasFeature(ObjectFeatures.Function)
					? DynValue.FromObject(script, new CallbackFunction(Call))
					: null;
			return HasFeature(ObjectFeatures.Operators) ? LuaOperator(metaname) : null;
		}
		public virtual DynValue LuaOperator(string metaname)
			=> null;

		public virtual Value Index(Arguments args)
		{
			switch (args.Length)
			{
			case 0:
				return new Value();
			case 1:
				return Value.IndexRef(this, args[0]);
			default:
				throw new InvalidOperationException(Name + " cannot be multi-indexed");
			}
		}
		public virtual Value IndexGet(Value index) => Get(index.String);
		public virtual bool IndexSet(Value index, Value value) => Set(index.String, value);
		public virtual bool IndexModify(Value index, OpCode op, Value value) => Modify(index.String, op, value);
		public virtual bool Modify(string name, OpCode op, Value value) => false;

		bool IProperties.Delete(string name) => false;
		void IProperties.Reset() { }
		IEngine IObject.Engine => null;
		IObject IObject.BaseClass => null;
		IProperties IObject.BaseProps => null;
		IProperties IObject.MoreProps => null;
	}
}

using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public abstract class FunctionBase : IObject, IUserDataType
	{
		public ObjectFeatures Features => ObjectFeatures.Function;
		public abstract Value Call(Arguments args);
		public virtual DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			args.ToRos(out var ros);
			return Call(ros).ToLua();
		}

		Value ICallable.Call(IObject self, Arguments args)
			=> Call(args);
		IObject IObject.Create(Arguments args)
		   	=> throw new InvalidOperationException(GetType().Name + " is a function, not a constructor");
		DynValue IUserDataType.MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			if (metaname != "__call") return null;
			return DynValue.FromObject(script, new CallbackFunction(Call));
		}

		bool IProperties.Has(string name) => false;
		Value IProperties.Get(string name) => throw new NotImplementedException();
		bool IProperties.Get(string name, out Value value)
		{
			value = new Value();
			return false;
		}
		bool IProperties.Set(string name, Value value) => false;
		DynValue IUserDataType.Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
			=> null;
		bool IUserDataType.SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> false;

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
		IEngine IObject.Engine => null;
		IObject IObject.BaseClass => null;
		IProperties IObject.BaseProps => null;
		IProperties IObject.MoreProps => null;
		Value IObject.Value => new Value(GetType().FullName);
		Type IObject.Type => null;
		object IObject.Target => null;
		IObject IObject.Convert(object value) => null;
	}
}

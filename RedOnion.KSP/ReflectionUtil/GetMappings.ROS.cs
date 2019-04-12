using System;
using RedOnion.Script;
using RedOnion.Script.ReflectedObjects;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class GetMappings : IObject
	{
		public bool Has(string name)
			=> TryGetAssemblyNamespaceInstance(name, out var dummy);
		Value IProperties.Get(string name)
		{
			if (!Get(name, out var value))
				throw new NotImplementedException(name + " does not exist");
			return value;
		}
		public bool Get(string name, out Value value)
		{
			if (TryGetAssemblyNamespaceInstance(name, out var ns))
			{
				value = Value.FromObject(ns);
				return true;
			}
			value = Value.Undefined;
			return false;
		}
		Value IObject.Index(Arguments args)
			=> args.Length != 1 ? new Value() : Value.IndexRef(this, args[0]);
		Value IObject.IndexGet(Value index)
		{
			if (TryGetAssemblyNamespaceInstance(index.String, out var ns))
				return Value.FromObject(ns);
			return new Value();
		}

		bool IProperties.Set(string name, Value value) => false;
		string IObject.Name => "assembly";
		Value IObject.Value => new Value("assembly");
		IEngine IObject.Engine => null;
		ObjectFeatures IObject.Features => ObjectFeatures.None;
		IObject IObject.BaseClass => null;
		IProperties IObject.BaseProps => null;
		IProperties IObject.MoreProps => null;
		Type IObject.Type => null;
		object IObject.Target => null;

		Value ICallable.Call(IObject self, Arguments args) => throw new NotImplementedException();
		IObject IObject.Convert(object value) => throw new NotImplementedException();
		IObject IObject.Create(Arguments args) => throw new NotImplementedException();
		bool IProperties.Delete(string name) => false;
		bool IObject.IndexModify(Value index, OpCode op, Value value) => false;
		bool IObject.IndexSet(Value index, Value value) => false;
		bool IObject.Modify(string name, OpCode op, Value value) => false;
		bool IObject.Operator(OpCode op, Value arg, bool selfRhs, out Value result)
		{
			result = Value.Undefined;
			return false;
		}
		void IProperties.Reset() { }
	}
}

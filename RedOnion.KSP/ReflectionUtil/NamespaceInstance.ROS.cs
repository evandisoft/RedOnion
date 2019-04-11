using RedOnion.Script;
using RedOnion.Script.ReflectedObjects;
using System;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class NamespaceInstance : IObject
	{
		public bool Has(string name)
			=> TryGetSubNamespace(name, out var dummy1)
			|| TryGetType(name, out var dummy2);
		public Value Get(string name)
		{
			if (!Get(name, out var value))
				throw new NotImplementedException(name + " does not exist");
			return value;
		}
		public bool Get(string name, out Value value)
		{
			if (TryGetSubNamespace(name, out var ns))
			{
				value = Value.FromObject(ns);
				return true;
			}
			if (TryGetType(name, out var type))
			{
				value = new Value(new ReflectedType(null, type));
				return true;
			}
			value = Value.Undefined;
			return false;
		}

		bool IProperties.Set(string name, Value value) => false;
		string IObject.Name => NamespaceString;
		Value IObject.Value => new Value(NamespaceString);
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
		Value IObject.Index(Arguments args) => throw new NotImplementedException();
		Value IObject.IndexGet(Value index) => throw new NotImplementedException();
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

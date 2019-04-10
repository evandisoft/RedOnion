using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	// this is here to mitigate problems with MoonSharp when RedOnion.Builder was touching Globals
	[ProxyDocs(typeof(Globals))]
	public static class GlobalMembers
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.None,

		"Global variables, objects and functions.",

		new IMember[]
		{
			new Interop("reflect", "Reflect", "All the reflection stuff and namespaces.",
				() => Reflect.Instance),
			new Interop("native", "Reflect", "Alias to `reflect` because of the namespaces.",
				() => Reflect.Instance),
			new Interop("ship", "Ship", "Active vessel (in flight or editor).",
				() => Ship.Instance),
			new Interop("stage", "Stage", "Staging logic.",
				() => Stage.Instance),
			new Interop("Vector", "VectorCreator", "Function for creating 3D vector / coordinate.",
				() => VectorCreator.Instance),
			new Interop("V", "VectorCreator", "Alias to Vector Function for creating 3D vector / coordinate.",
				() => VectorCreator.Instance),
			new Function("vdot", "Vector", "Alias to `Vector.dot` (or `v.dot`).",
				() => VectorCreator.DotFunction.Instance),
			new Function("vcrs", "Vector", "Alias to `Vector.cross` (or `v.cross`).",
				() => VectorCreator.CrossFunction.Instance),
			new Function("vcross", "Vector", "Alias to `Vector.cross` (or `v.cross`).",
				() => VectorCreator.CrossFunction.Instance),
			new Function("vangle", "double", "alias to `Vector.angle` (or `v.angle`).",
				() => VectorCreator.AngleFunction.Instance),
			new Function("vang", "double", "alias to `Vector.angle` (or `v.angle`).",
				() => VectorCreator.AngleFunction.Instance),
		});
	}
	[IgnoreForDocs]
	public class Globals : Table, IObject, IType
	{
		public MemberList Members => GlobalMembers.MemberList;
		public static Globals Instance { get; } = new Globals();

		public Globals() : base(null)
		{
			this["__index"] = new Func<Table, DynValue, DynValue>(Get);
			this["__newindex"] = new Func<Table, DynValue, DynValue, DynValue>(Set);
		}

		public bool Has(string name)
			=> Members.Contains(name);
		Value IProperties.Get(string name)
		{
			if (!Get(name, out var value))
				throw new NotImplementedException(name + " does not exist");
			return value;
		}
		bool IProperties.Delete(string name) => false;
		void IProperties.Reset() { }

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
			if (Members.TryGetValue(name, out var member) && member.CanWrite)
			{
				member.RosSet(this, value);
				return true;
			}
			return false;
		}

		public virtual DynValue Get(Table table, DynValue index)
		{
			if (Members.TryGetValue(index.String, out var member) && member.CanRead)
				return member.LuaGet(this);
			return null;
		}
		public virtual DynValue Set(Table table, DynValue index, DynValue value)
		{
			if (Members.TryGetValue(index.String, out var member) && member.CanWrite)
			{
				member.LuaSet(this, value);
				return DynValue.NewBoolean(true);
			}
			table[index] = value;
			return DynValue.NewBoolean(false);
		}

		IEngine IObject.Engine => null;
		ObjectFeatures IObject.Features => Members.Features;
		string IObject.Name => "Globals";
		IObject IObject.BaseClass => null;
		IProperties IObject.BaseProps => null;
		IProperties IObject.MoreProps => null;
		Value IObject.Value => new Value("Globals");
		Type IObject.Type => null;
		object IObject.Target => null;
		bool IObject.Modify(string name, OpCode op, Value value) => false;
		IObject IObject.Create(Arguments args) => null;
		Value IObject.Index(Arguments args) => Value.Undefined;
		Value IObject.IndexGet(Value index) => Value.Undefined;
		bool IObject.IndexSet(Value index, Value value) => false;
		bool IObject.IndexModify(Value index, OpCode op, Value value) => false;
		IObject IObject.Convert(object value) => null;
		Value ICallable.Call(IObject self, Arguments args) => Value.Undefined;
		bool IObject.Operator(OpCode op, Value arg, bool selfRhs, out Value result)
		{
			result = Value.Undefined;
			return false;
		}
	}
}

using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public class Globals : Table, IProperties, IType
	{
		public static Globals Instance { get; } = new Globals();

		public Dictionary<string, IMember> Members { get; }
		public IMember[] MemberList { get; } = new IMember[]
		{
			new Native("ship", "Vessel", "Active vessel (in flight or editor)",
				() => HighLogic.LoadedSceneIsFlight ? (object)FlightGlobals.ActiveVessel
				: HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null),
			new Interop("stage", "Stage", "Staging logic",
				() => Stage.Instance)
		};

		public string Help => "Global object";
		public ObjectFeatures Features => ObjectFeatures.None;

		public Globals() : base(null)
		{
			this["__index"] = new Func<Table, DynValue, DynValue>(Get);
			this["__newindex"] = new Func<Table, DynValue, DynValue, DynValue>(Set);
			Members = new Dictionary<string, IMember>(MemberList.Length);
			foreach (var member in MemberList)
				Members.Add(member.Name, member);
		}

		public bool Has(string name)
			=> Members.ContainsKey(name);
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
			return DynValue.NewBoolean(false);
		}
	}
}

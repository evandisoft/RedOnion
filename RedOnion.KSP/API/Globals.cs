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
		public static MemberList Members { get; } = new MemberList(
		"Global variables, objects and functions.",
		new IMember[]
		{
			new Native("ship", "Vessel", "Active vessel (in flight or editor)",
				() => HighLogic.LoadedSceneIsFlight ? (object)FlightGlobals.ActiveVessel
				: HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null),
			new Interop("stage", "Stage", "Staging logic",
				() => Stage.Instance)
		});
	}
	[IgnoreForDocs]
	public class Globals : Table, IProperties, IType
	{
		public static Globals Instance { get; } = new Globals();

		ObjectFeatures IType.Features => ObjectFeatures.None;
		public MemberList Members => GlobalMembers.Members;

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
	}
}

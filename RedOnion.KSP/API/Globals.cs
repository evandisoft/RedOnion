using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;

namespace RedOnion.KSP.API
{
	// this is here to mitigate problems with MoonSharp when RedOnion.Builder was touching Globals
	[ProxyDocs(typeof(Globals))]
	public static class GlobalMembers
	{
		public static MemberList MemberList { get; } = new MemberList(
		"Global variables, objects and functions.",

		new IMember[]
		{
			new Interop("reflect", "Reflect", "All the reflection stuff and namespaces.",
				() => Reflect.Instance),
			new Interop("native", "Reflect", "Alias to `reflect` because of the namespaces.",
				() => Reflect.Instance),
			new Interop("ship", "Ship", "Active vessel (in flight only, null otherwise).",
				() => Ship.Active),
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
	public class Globals : Table, IType, IHasCompletionProxy
	{
		public MemberList Members => GlobalMembers.MemberList;
		public static Globals Instance { get; } = new Globals();

		public Globals() : base(null)
		{
			this["__index"] = new Func<Table, DynValue, DynValue>(Get);
			this["__newindex"] = new Func<Table, DynValue, DynValue, DynValue>(Set);
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

		public object CompletionProxy => Members;
	}
}

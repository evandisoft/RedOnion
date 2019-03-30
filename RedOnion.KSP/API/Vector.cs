using MoonSharp.Interpreter;
using RedOnion.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public class VectorCreator : InteropObject
	{
		public static Vector Instance { get; } = new Vector();
		public static MemberList MemberList { get; } = new MemberList(
		// note: the following string is actually never used,
		// but do not try to link it to Vector.MemberList.Help
		// as that could crash (did try the opposite and Builder crashed
		// because VectorCreator.MemberList was null when accessed from Vector).
		"3D vector / coordinate",
		new IMember[]
		{

		});
		public VectorCreator() : base(
			ObjectFeatures.Function|ObjectFeatures.Constructor,
			MemberList)
		{ }
		public override Value Call(IObject self, Arguments args)
			=> new Value(Create(args));
		public override IObject Create(Arguments args)
		{
			switch (args.Length)
			{
			case 0:
				return new Vector();
			case 1:
				return new Vector(args[0]);
			case 2:
				return new Vector(args[0], args[1]);
			default:
				return new Vector(args[0], args[1], args[2]);
			}
		}
		public override DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			if (metaname != "__call") return null;
			return DynValue.FromObject(script, new Func<DynValue[], Vector>(CreateVector));
		}
		Vector CreateVector(DynValue[] args)
		{
			switch (args.Length)
			{
			case 0:
				return new Vector();
			case 1:
				return new Vector(args[0].Number);
			case 2:
				return new Vector(args[0].Number, args[1].Number);
			default:
				return new Vector(args[0].Number, args[1].Number, args[2].Number);
			}
		}
	}
	[Creator(typeof(VectorCreator))]
	public class Vector : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(
		"3D vector / coordinate",
		new IMember[]
		{
			new Native<Vector>("native", "Vector3d",
				"Native Vector3d (convertible to UnityEngine.Vector3)",
				v => v.Native, (v, value) => v.Native = (Vector3d)value)
		});
		public Vector() : base(ObjectFeatures.None, MemberList) { }
		public Vector(Vector3d native) : this() => Native = native;
		public Vector(double x, double y, double z) : this() => Native = new Vector3d(x, y, z);
		public Vector(double x, double y) : this() => Native = new Vector3d(x, y);
		public Vector(double all) : this() => Native = new Vector3d(all, all, all);
		public Vector3d Native { get; set; }
	}
}

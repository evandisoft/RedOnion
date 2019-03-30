using MoonSharp.Interpreter;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using System;
using UnityEngine;

//NOTE: Never move Instance above MemberList ;)

namespace RedOnion.KSP.API
{
	public partial class VectorCreator : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.Function|ObjectFeatures.Constructor,

		"Function to create 3D vector / coordinate, also aliased as simple `V`.",

		new IMember[]
		{
			new Interop("zero",	"Vector", "Vector(0, 0, 0).", () => Zero),
			new Interop("one",	"Vector", "Vector(1, 1, 1).", () => One),
			new Interop("forward","Vector","Vector(0, 0, 1).", () => Forward),
			new Interop("fwd",	"Vector", "Alias to forward - Vector(0, 0, 1).", () => Forward),
			new Interop("back",	"Vector", "Vector(0, 0, -1).", () => Back),
			new Interop("up",	"Vector", "Vector(0, 1, 0).", () => Up),
			new Interop("down",	"Vector", "Vector(0, -1, 0).", () => Down),
			new Interop("left",	"Vector", "Vector(-1, 0, 0).", () => Left),
			new Interop("right","Vector", "Vector(1, 0, 0).", () => Right),

			new Function("cross", "Vector", "Cross product.", () => CrossFunction.Instance),
		});

		public static VectorCreator Instance { get; } = new VectorCreator();
		public VectorCreator() : base(MemberList) { }

		public static Vector Zero = new Vector(Vector3d.zero, true);
		public static Vector One = new Vector(Vector3d.one, true);
		public static Vector Forward = new Vector(Vector3d.forward, true);
		public static Vector Back = new Vector(Vector3d.back, true);
		public static Vector Up = new Vector(Vector3d.up, true);
		public static Vector Down = new Vector(Vector3d.down, true);
		public static Vector Left = new Vector(Vector3d.left, true);
		public static Vector Right = new Vector(Vector3d.right, true);

		public override Value Call(IObject self, Arguments args)
			=> new Value(Create(args));
		public override IObject Create(Arguments args)
		{
			switch (args.Length)
			{
			case 0:
				return new Vector();
			case 1:
				var arg = args[0];
				var obj = arg.Object;
				if (obj != null)
				{
					if (obj is ArrayObj arr)
					{
						switch (arr.Length)
						{
						case 0:
							return new Vector();
						case 1:
							return new Vector(arr[0]);
						case 2:
							return new Vector(arr[1]);
						default:
							return new Vector(arr[2]);
						}
					}
					if (obj is ListObj list)
					{
						switch (list.Length)
						{
						case 0:
							return new Vector();
						case 1:
							return new Vector(list[0]);
						case 2:
							return new Vector(list[1]);
						default:
							return new Vector(list[2]);
						}
					}
				}
				return new Vector(arg);
			case 2:
				return new Vector(args[0], args[1]);
			default:
				return new Vector(args[0], args[1], args[2]);
			}
		}
		public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			switch (args.Count)
			{
			case 1:
				return UserData.Create(new Vector());
			case 2:
				return UserData.Create(new Vector(args[1].Number));
			case 3:
				return UserData.Create(new Vector(args[1].Number, args[2].Number));
			default:
				return UserData.Create(new Vector(args[1].Number, args[2].Number, args[3].Number));
			}
		}
	}
	[Creator(typeof(VectorCreator))]
	public partial class Vector : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.None,

		"3D vector / coordinate",

		new IMember[]
		{
			new Native<Vector, Vector3d>(
				"native",
				"Native Vector3d (convertible to UnityEngine.Vector3).",
				v => v.Native, (v, value) => v.Native = value),
			new Native<Vector, Vector3>(
				"unity",
				"Native UnityEngine.Vector3 (float).",
				v => v.Native, (v, value) => v.Native = value),
			new Native<Vector, Vector2>(
				"vector2",
				"Native UnityEngine.Vector2 (float x,y).",
				v => (Vector3)v.Native, (v, value) => v.Native = (Vector3)value),
			new Method<Vector>(
				"scale", "void",
				"Scale the vector by a factor (all axes at once if number is provided, per-axis if Vector).",
				() => ScaleMethod.Instance),
		});

		public Vector() : base(MemberList) { }
		public Vector(Vector3d native) : this() => Native = native;
		public Vector(Vector3d native, bool readOnly) : this(native) => ReadOnly = readOnly;
		public Vector(Vector3 native) : this() => Native = native;
		public Vector(double x, double y, double z) : this() => Native = new Vector3d(x, y, z);
		public Vector(double x, double y) : this() => Native = new Vector3d(x, y);
		public Vector(double all) : this() => Native = new Vector3d(all, all, all);

		public Vector3d Native { get; set; }
		public double X
		{
			get => Native.x;
			set => Native = new Vector3d(value, Y, Z);
		}
		public double Y
		{
			get => Native.y;
			set => Native = new Vector3d(X, value, Z);
		}
		public double Z
		{
			get => Native.z;
			set => Native = new Vector3d(X, Y, value);
		}

		public override string ToString()
			=> string.Format(Value.Culture, "[{0}, {1}, {2}]", X, Y, Z);
	}
}

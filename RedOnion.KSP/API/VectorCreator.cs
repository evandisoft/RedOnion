using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Never move Instance above MemberList ;)

namespace RedOnion.KSP.API
{
	public partial class VectorCreator : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(

@"Function to create 3D vector / coordinate, also aliased as simple `V`.
Receives either three arguments (x,y,z), two (x,y - z=0), or one (x=y=z).
Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).",

		new IMember[]
		{
			new Interop("zero", "Vector", "Vector(0, 0, 0).", () => Zero),
			new Interop("one",  "Vector", "Vector(1, 1, 1).", () => One),
			new Interop("forward","Vector","Vector(0, 0, 1).", () => Forward),
			new Interop("fwd",  "Vector", "Alias to forward - Vector(0, 0, 1).", () => Forward),
			new Interop("back", "Vector", "Vector(0, 0, -1).", () => Back),
			new Interop("up",   "Vector", "Vector(0, 1, 0).", () => Up),
			new Interop("down", "Vector", "Vector(0, -1, 0).", () => Down),
			new Interop("left", "Vector", "Vector(-1, 0, 0).", () => Left),
			new Interop("right","Vector", "Vector(1, 0, 0).", () => Right),

			new Function("cross", "Vector", "Cross product.", () => CrossFunction.Instance),
			new Function("crs", "Vector", "Cross product. (Alias to cross.)", () => CrossFunction.Instance),
			new Function("dot", "Vector", "Dot product.", () => DotFunction.Instance),
			new Function("abs", "Vector", "Vector with coordinates changed to non-negative.", () => AbsFunction.Instance),
			new Function("angle", "Vector", "Angle between vectors (0..180).", () => AngleFunction.Instance),
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

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			switch (args.Length)
			{
			case 0:
				result = new Value(new Vector());
				return true;
			case 1:
				var arg = args[0];
				var vec = ToVector(arg);
				if (vec == null)
					throw new InvalidOperationException(string.Format(Value.Culture,
						"Could not convert {0} to vector", arg.Name));
				result = new Value(vec);
				return true;
			case 2:
				result = new Value(new Vector(args[0].ToDouble(), args[1].ToDouble()));
				return true;
			default:
				result = new Value(new Vector(args[0].ToDouble(), args[1].ToDouble(), args[2].ToDouble()));
				return true;
			}
		}
		public static Vector ToVector(object value)
			=> ToVector3d(value, out var v) ? new Vector(v) : null;
		public static Vector ToVector(Value value)
			=> ToVector3d(value, out var v) ? new Vector(v) : null;
		public static bool ToVector3d(object value, out Vector3d result)
		{
			if (value != null)
			{
				if (value is Vector v)
				{
					result = v.native;
					return true;
				}
				if (value is Value val)
					return ToVector3d(val, out result);
				if (value is Vector3d v3d)
				{
					result = v3d;
					return true;
				}
				if (value is Vector3 v3)
				{
					result = v3;
					return true;
				}
				if (value is Vector2d v2d)
				{
					result = v2d;
					return true;
				}
				if (value is Vector2 v2)
				{
					result = new Vector3d(v2.x, v2.y);
					return true;
				}
				if (value is IConvertible cv)
				{
					var d = cv.ToDouble(Value.Culture);
					result = new Vector3d(d, d, d);
					return true;
				}
				if (value is IList<Value> list)
				{
					switch (list.Count)
					{
					case 0:
						result = new Vector3d();
						return true;
					case 1:
						var all = list[0].ToDouble();
						result = new Vector3d(all, all, all);
						return true;
					case 2:
						result = new Vector3d(list[0].ToDouble(), list[1].ToDouble());
						return true;
					default:
						result = new Vector3d(list[0].ToDouble(), list[1].ToDouble(), list[2].ToDouble());
						return true;
					}
				}
			}
			result = new Vector3d();
			return false;
		}
		public static bool ToVector3d(Value value, out Vector3d result)
		{
			if (value.IsNumerOrChar)
			{
				var d = value.ToDouble();
				result = new Vector3d(d, d, d);
				return true;
			}
			return ToVector3d(value.obj, out result);
		}
	}
}

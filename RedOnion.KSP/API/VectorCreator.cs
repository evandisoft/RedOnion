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
		ObjectFeatures.Function|ObjectFeatures.Constructor
		|ObjectFeatures.Converter|ObjectFeatures.TypeReference,

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

		public override Type Type => typeof(Vector);

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
				var vec = ToVector(arg);
				if (vec == null)
					throw new InvalidOperationException(string.Format(Value.Culture,
						"Could not convert {0} to vector", arg.Name));
				return vec;
			case 2:
				return new Vector(args[0], args[1]);
			default:
				return new Vector(args[0], args[1], args[2]);
			}
		}
		public override IObject Convert(object value)
			=> ToVector(value);
		public static Vector ToVector(object value)
			=> ToVector3d(value, out var v) ? new Vector(v) : null;
		public static Vector ToVector(Value value)
			=> ToVector3d(value, out var v) ? new Vector(v) : null;
		public static Vector ToVector(IObject obj)
			=> obj is Vector v ? v : ToVector3d(obj, out var v3d) ? new Vector(v3d) : null;
		public static bool ToVector3d(object value, out Vector3d result)
		{
			if (value != null)
			{
				if (value is IObject obj)
					return ToVector3d(obj, out result);
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
				var pval = Value.FromPrimitive(value);
				if (pval.IsNumber)
				{
					var d = pval.Double;
					result = new Vector3d(d, d, d);
					return true;
				}
			}
			result = new Vector3d();
			return false;
		}
		public static bool ToVector3d(Value value, out Vector3d result)
		{
			if (value.IsNumber)
			{
				var d = value.Double;
				result = new Vector3d(d, d, d);
				return true;
			}
			return ToVector3d(value.Object, out result);
		}
		public static bool ToVector3d(IObject obj, out Vector3d result)
		{
			result = new Vector3d();
			if (obj == null)
				return false;
			if (obj is Vector v)
			{
				result = v.Native;
				return true;
			}
			if (obj is IListObject list)
			{
				switch (list.Count)
				{
				case 0:
					result = new Vector3d();
					return true;
				case 1:
					var all = list[0].Double;
					result = new Vector3d(all, all, all);
					return true;
				case 2:
					result = new Vector3d(list[0].Double, list[1].Double);
					return true;
				default:
					result = new Vector3d(list[0].Double, list[1].Double, list[2].Double);
					return true;
				}
			}
			if (obj.HasFeature(ObjectFeatures.Proxy))
				return ToVector3d(obj.Target, out result);
			return false;
		}
	}
}

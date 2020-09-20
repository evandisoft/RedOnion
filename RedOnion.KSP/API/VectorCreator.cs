using MunSharp.Interpreter;
using RedOnion.KSP.Utilities;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Description("Function to create 3D vector / coordinate."
		+ " Receives either three arguments (x,y,z), two (x,y; z=0), or one (x=y=z)."
		+ " Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).")]
	public partial class VectorCreator : ICallable
	{
		[Browsable(false), MoonSharpHidden]
		public static VectorCreator Instance { get; } = new VectorCreator();
		protected VectorCreator() { }

		[Description("Vector(0, 0, 0).")]
		public static readonly Vector zero = new Vector(Vector3d.zero);
		[Description("Vector(1, 1, 1).")]
		public static readonly Vector one = new Vector(Vector3d.one);
		[Description("Vector(0, 0, 1).")]
		public static readonly Vector forward = new Vector(Vector3d.forward);
		[Description("Alias to forward - Vector(0, 0, 1).")]
		public static readonly Vector fwd = forward;
		[Description("Vector(0, 0, -1).")]
		public static readonly Vector back = new Vector(Vector3d.back);
		[Description("Vector(0, 1, 0).")]
		public static readonly Vector up = new Vector(Vector3d.up);
		[Description("Vector(0, -1, 0).")]
		public static readonly Vector down = new Vector(Vector3d.down);
		[Description("Vector(-1, 0, 0).")]
		public static readonly Vector left = new Vector(Vector3d.left);
		[Description("Vector(1, 0, 0).")]
		public static readonly Vector right = new Vector(Vector3d.right);
		[Description("Vector(nan, nan, nan).")]
		public static readonly Vector none = new Vector(new Vector3d(double.NaN, double.NaN, double.NaN));

		[Description("Cross product.")]
		public static Vector cross(Vector a, Vector b)
			=> new Vector(Vector3d.Cross(a.native, b.native));
		[Description("Cross product. (Alias to cross)")]
		public static Vector crs(Vector a, Vector b)
			=> new Vector(Vector3d.Cross(a.native, b.native));
		[Description("Scale vector by vector. Per axis. Multiplication does the same.")]
		public static Vector scale(Vector a, Vector b)
			=> a * b;
		[Description("Shrink vector by vector. Per axis. Division does the same.")]
		public static Vector shrink(Vector a, Vector b)
			=> a / b;
		[Description("Vector with coordinates changed to non-negative.")]
		public static Vector abs(Vector v)
			=> new Vector(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z));
		[Description("Dot product.")]
		public static double dot(Vector a, Vector b)
			=> Vector3d.Dot(a.native, b.native);
		[Description("Angle between vectors (0..180).")]
		public static double angle(Vector a, Vector b)
			=> Vector3d.Angle(a.native, b.native);

		public static Vector3d scale(Vector3d a, Vector3d b)
			=> new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vector3d shrink(Vector3d a, Vector3d b)
			=> new Vector3d(a.x / b.x, a.y / b.y, a.z / b.z);
		public static Vector3 scale(Vector3 a, Vector3 b)
			=> new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vector3 shrink(Vector3 a, Vector3 b)
			=> new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

		bool ICallable.Call(ref Value result, object self, in Arguments args)
			=> Call(ref result, self, args);
		static bool Call(ref Value result, object self, in Arguments args)
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
		[MoonSharpUserDataMetamethod("__call"), Browsable(false)]
		public static DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			var result = Value.Void;
			var self = args.ToRos(out var ros);
			return Call(ref result, self, ros) ? result.ToLua() : DynValue.Void;
		}

		public static Vector? ToVector(object value)
			=> ToVector3d(value, out var v) ? new Vector(v) : (Vector?)null;
		public static Vector? ToVector(Value value)
			=> ToVector3d(value, out var v) ? new Vector(v) : (Vector?)null;
		[Browsable(false), MoonSharpHidden]
		public static Vector? ToConstVector(object value)
			=> ToVector3d(value, out var v) ? new Vector(v) : (Vector?)null;
		[Browsable(false), MoonSharpHidden]
		public static Vector? ToConstVector(Value value)
			=> ToVector3d(value, out var v) ? new Vector(v) : (Vector?)null;
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
			if (value.IsNumberOrChar)
			{
				var d = value.ToDouble();
				result = new Vector3d(d, d, d);
				return true;
			}
			return ToVector3d(value.obj, out result);
		}
	}
}

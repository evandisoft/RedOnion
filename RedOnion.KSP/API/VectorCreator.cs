using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Description("Function to create 3D vector / coordinate, also aliased as simple `V`."
		+ " Receives either three arguments (x,y,z), two (x,y - z=0), or one (x=y=z)."
		+ " Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).")]
	public class VectorCreator : ICallable, IType
	{
		[Browsable(false), MoonSharpHidden]
		public static VectorCreator Instance { get; } = new VectorCreator();
		protected VectorCreator() { }

		Type IType.Type => typeof(Vector);
		bool IType.IsType(object type)
			=> type is string str ? str.Equals("vector", StringComparison.OrdinalIgnoreCase)
			: type is Type t ? t.IsAssignableFrom(typeof(Vector)) : false;

		[Description("Vector(0, 0, 0).")]
		public static readonly ConstVector Zero = new ConstVector(Vector3d.zero);
		[Description("Vector(1, 1, 1).")]
		public static readonly ConstVector One = new ConstVector(Vector3d.one);
		[Description("Vector(0, 0, 1).")]
		public static readonly ConstVector Forward = new ConstVector(Vector3d.forward);
		[Description("Alias to forward - Vector(0, 0, 1).")]
		public static readonly ConstVector Fwd = Forward;
		[Description("Vector(0, 0, -1).")]
		public static readonly ConstVector Back = new ConstVector(Vector3d.back);
		[Description("Vector(0, 1, 0).")]
		public static readonly ConstVector Up = new ConstVector(Vector3d.up);
		[Description("Vector(0, -1, 0).")]
		public static readonly ConstVector Down = new ConstVector(Vector3d.down);
		[Description("Vector(-1, 0, 0).")]
		public static readonly ConstVector Left = new ConstVector(Vector3d.left);
		[Description("Vector(1, 0, 0).")]
		public static readonly ConstVector Right = new ConstVector(Vector3d.right);

		[Description("Cross product.")]
		public static Vector Cross(ConstVector a, ConstVector b)
			=> new Vector(Vector3d.Cross(a.Native, b.Native));
		[Description("Cross product. (Alias to cross)")]
		public static Vector Crs(ConstVector a, ConstVector b)
			=> new Vector(Vector3d.Cross(a.Native, b.Native));
		[Description("Dot product.")]
		public static Vector Dot(ConstVector a, ConstVector b)
			=> new Vector(Vector3d.Dot(a.Native, b.Native));
		[Description("Vector with coordinates changed to non-negative.")]
		public static Vector Abs(ConstVector v)
			=> new Vector(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
		[Description("Angle between vectors(0..180).")]
		public static double Angle(ConstVector a, ConstVector b)
			=> Vector3d.Angle(a.Native, b.Native);

		bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
			=> Call(ref result, self, args, create);
		static bool Call(ref Value result, object self, Arguments args, bool create)
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
		static DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			var result = Value.Void;
			var self = args.ToRos(out var ros);
			return Call(ref result, self, ros, false) ? result.ToLua() : DynValue.Void;
		}

		public static Vector ToVector(object value)
			=> ToVector3d(value, out var v) ? new Vector(v) : null;
		public static Vector ToVector(Value value)
			=> ToVector3d(value, out var v) ? new Vector(v) : null;
		[Browsable(false), MoonSharpHidden]
		public static ConstVector ToConstVector(object value)
			=> ToVector3d(value, out var v) ? new ConstVector(v) : null;
		[Browsable(false), MoonSharpHidden]
		public static ConstVector ToConstVector(Value value)
			=> ToVector3d(value, out var v) ? new ConstVector(v) : null;
		public static bool ToVector3d(object value, out Vector3d result)
		{
			if (value != null)
			{
				if (value is ConstVector v)
				{
					result = v.Native;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using MoonSharp.Interpreter;
using RedOnion.ROS;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Description("Read-only 3D vector / coordinate, base class for `Vector`,"
		+ " used for Vector.zero and other constants. Can also be used for properties.")]
	public class ConstVector : IEquatable<ConstVector>, IEquatable<Vector>, IOperators, IConvert
	{
		protected Vector3d native;
		[Description("Native Vector3d(`double x, y, z`).")]
		public Vector3d Native => native;

		public override string ToString()
			=> string.Format(Value.Culture, "[{0}, {1}, {2}]", X, Y, Z);

		public ConstVector() { }
		public ConstVector(ConstVector src) => native = src.native;
		public ConstVector(Vector3d src) => native = src;
		public ConstVector(Vector3 src) => native = src;
		public ConstVector(Vector2d src) => native = src;
		public ConstVector(Vector2 src) => native = new Vector3d(src.x, src.y);
		public ConstVector(double x, double y, double z) => native = new Vector3d(x, y, z);
		public ConstVector(double x, double y) => native = new Vector3d(x, y);
		public ConstVector(double all) => native = new Vector3d(all, all, all);

		public ConstVector(IList<Value> src)
		{
			switch (src.Count)
			{
			case 0:
				return;
			case 1:
				var all = src[0].ToDouble();
				native = new Vector3d(all, all, all);
				return;
			case 2:
				native = new Vector3d(src[0].ToDouble(), src[1].ToDouble());
				return;
			default:
				native = new Vector3d(src[0].ToDouble(), src[1].ToDouble(), src[2].ToDouble());
				return;
			}
		}
		public ConstVector(IList<double> src)
		{
			switch (src.Count)
			{
			case 0:
				return;
			case 1:
				var all = src[0];
				native = new Vector3d(all, all, all);
				return;
			case 2:
				native = new Vector3d(src[0], src[1]);
				return;
			default:
				native = new Vector3d(src[0], src[1], src[2]);
				return;
			}
		}
		public ConstVector(IList<float> src)
		{
			switch (src.Count)
			{
			case 0:
				return;
			case 1:
				var all = src[0];
				native = new Vector3d(all, all, all);
				return;
			case 2:
				native = new Vector3d(src[0], src[1]);
				return;
			default:
				native = new Vector3d(src[0], src[1], src[2]);
				return;
			}
		}

		[Description("The X-coordinate")]
		public double X => native.x;
		[Description("The Y-coordinate")]
		public double Y => native.y;
		[Description("The Z-coordinate")]
		public double Z => native.z;
		[Description("Size of the vector - `sqrt(x*x+y*y+z*z)`.")]
		public double Size => native.magnitude;
		[Description("Alias to size of the vector - `sqrt(x*x+y*y+z*z)`.")]
		public double Magnitude => native.magnitude;
		[Description("Square size of the vector - `x*x+y*y+z*z`.")]
		public double SquareSize => native.sqrMagnitude;
		[Description("Get normalized vector (size 1).")]
		public Vector Normalized => new Vector(native.normalized);

		[Description("Native UnityEngine.Vector3 (`float x,y,z`).")]
		public Vector3 Vector3 => native;
		[Description("Native UnityEngine.Vector2 (`float x,y`).")]
		public Vector2 Vector2 => Vector3;
		[Description("Index the coordinates as double[3]")]
		public double this[int i] => native[i];

		public static implicit operator Vector3d(ConstVector v) => v.Native;
		public static implicit operator Vector3(ConstVector v) => v.Native;
		public static implicit operator Vector2d(ConstVector v) => new Vector2d(v.X, v.Y);
		public static implicit operator Vector2(ConstVector v) => new Vector2((float)v.X, (float)v.Y);
		public static explicit operator ConstVector(Vector3d v) => new Vector(v);
		public static explicit operator ConstVector(Vector3 v) => new Vector(v);
		public static explicit operator ConstVector(Vector2d v) => new Vector(v);
		public static explicit operator ConstVector(Vector2 v) => new Vector(v);

		public static Vector operator +(ConstVector a)
			=> new Vector(a.Native);
		public static Vector operator -(ConstVector a)
			=> new Vector(-a.Native);

		public static Vector operator +(ConstVector a, ConstVector b)
			=> new Vector(a.Native + b.Native);
		public static Vector operator +(Vector3d a, ConstVector b)
			=> new Vector(a + b.Native);
		public static Vector operator +(ConstVector a, Vector3d b)
			=> new Vector(a.Native + b);
		public static Vector operator +(Vector3 a, ConstVector b)
			=> new Vector(a + b.Native);
		public static Vector operator +(ConstVector a, Vector3 b)
			=> new Vector(a.Native + b);

		public static Vector operator -(ConstVector a, ConstVector b)
			=> new Vector(a.Native - b.Native);
		public static Vector operator -(Vector3d a, ConstVector b)
			=> new Vector(a - b.Native);
		public static Vector operator -(ConstVector a, Vector3d b)
			=> new Vector(a.Native - b);
		public static Vector operator -(Vector3 a, ConstVector b)
			=> new Vector(a - b.Native);
		public static Vector operator -(ConstVector a, Vector3 b)
			=> new Vector(a.Native - b);

		public static Vector operator *(ConstVector a, ConstVector b)
			=> new Vector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		public static Vector operator *(Vector3d a, ConstVector b)
			=> new Vector(a.x * b.X, a.y * b.Y, a.z * b.Z);
		public static Vector operator *(ConstVector a, Vector3d b)
			=> new Vector(a.X * b.x, a.Y * b.y, a.Z * b.z);
		public static Vector operator *(Vector3 a, ConstVector b)
			=> new Vector(a.x * b.X, a.y * b.Y, a.z * b.Z);
		public static Vector operator *(ConstVector a, Vector3 b)
			=> new Vector(a.X * b.x, a.Y * b.y, a.Z * b.z);

		public static Vector operator /(ConstVector a, ConstVector b)
			=> new Vector(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		public static Vector operator /(Vector3d a, ConstVector b)
			=> new Vector(a.x / b.X, a.y / b.Y, a.z / b.Z);
		public static Vector operator /(ConstVector a, Vector3d b)
			=> new Vector(a.X / b.x, a.Y / b.y, a.Z / b.z);
		public static Vector operator /(Vector3 a, ConstVector b)
			=> new Vector(a.x / b.X, a.y / b.Y, a.z / b.Z);
		public static Vector operator /(ConstVector a, Vector3 b)
			=> new Vector(a.X / b.x, a.Y / b.y, a.Z / b.z);

		public static Vector operator *(ConstVector a, double f)
			=> new Vector(a.X * f, a.Y * f, a.Z * f);
		public static Vector operator *(double f, ConstVector b)
			=> new Vector(f * b.X, f * b.Y, f * b.Z);
		public static Vector operator /(ConstVector a, double f)
			=> new Vector(a.X / f, a.Y / f, a.Z / f);

		public bool Equals(ConstVector other)
			=> other != null && native == other.Native;
		public bool Equals(Vector other)
			=> other != null && native == other.Native;
		public override bool Equals(object obj)
		{
			if (obj is Vector v)
				return Equals(v);
			if (obj is Vector3d v3d)
				return Native == v3d;
			if (obj is Vector3 v3)
				return (Vector3)Native == v3;
			if (Z < 0 || Z > 0) // zero and NaN are acceptable
				return false;
			if (obj is Vector2d v2d)
				return X == v2d.x && Y == v2d.y;
			if (obj is Vector2 v2)
				return (float)X == v2.x && (float)Y == v2.y;
			return false;
		}
		public override int GetHashCode()
			=> Native.GetHashCode();

		bool IOperators.Unary(ref Value self, OpCode op)
		{
			switch (op)
			{
			case OpCode.Plus:
				self = new Value(new Vector(Native));
				return true;
			case OpCode.Neg:
				self = new Value(new Vector(-Native));
				return true;
			}
			return false;
		}
		bool IOperators.Binary(ref Value lhs, OpCode op, ref Value rhs)
		{
			if (VectorCreator.ToVector3d(lhs, out var a))
			{
				if (VectorCreator.ToVector3d(rhs, out var b))
				{
					switch (op)
					{
					case OpCode.Add:
						lhs = new Value(new Vector(a + b));
						return true;
					case OpCode.Sub:
						lhs = new Value(new Vector(a - b));
						return true;
					case OpCode.Mul:
						lhs = new Value(new Vector(a.x * b.x, a.y * b.y, a.z * b.z));
						return true;
					case OpCode.Div:
						lhs = new Value(new Vector(a.x / b.x, a.y / b.y, a.z / b.z));
						return true;
					}
					return false;
				}
				if (rhs.IsNumerOrChar)
				{
					double f = rhs.ToDouble();
					switch (op)
					{
					case OpCode.Mul:
						lhs = new Value(new Vector(a.x * f, a.y * f, a.z * f));
						return true;
					case OpCode.Div:
						lhs = new Value(new Vector(a.x * f, a.y * f, a.z * f));
						return true;
					}
					return false;
				}
				return false;
			}
			if (op == OpCode.Mul && lhs.IsNumerOrChar && VectorCreator.ToVector3d(rhs, out var v))
			{
				double f = lhs.ToDouble();
				lhs = new Value(new Vector(v.x * f, v.y * f, v.z * f));
				return true;
			}
			return false;
		}
		[MoonSharpUserDataMetamethod("__unm"), Browsable(false)]
		public DynValue Neg(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 1)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				-VectorCreator.ToVector(args[0].ToObject()));
		}
		[MoonSharpUserDataMetamethod("__add"), Browsable(false)]
		public DynValue Add(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				+ VectorCreator.ToVector(args[1].ToObject()));
		}
		[MoonSharpUserDataMetamethod("__sub"), Browsable(false)]
		public DynValue Sub(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				- VectorCreator.ToVector(args[1].ToObject()));
		}
		[MoonSharpUserDataMetamethod("__mul"), Browsable(false)]
		public DynValue Mul(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				* VectorCreator.ToVector(args[1].ToObject()));
		}
		[MoonSharpUserDataMetamethod("__div"), Browsable(false)]
		public DynValue Div(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				/ VectorCreator.ToVector(args[1].ToObject()));
		}

		bool IConvert.Convert(ref Value self, Descriptor to)
		{
			if (to.Type == typeof(ConstVector))
			{
				self = new Value(new ConstVector((ConstVector)self.obj));
				return true;
			}
			if (to.Type == typeof(Vector))
			{
				self = new Value(new Vector((ConstVector)self.obj));
				return true;
			}
			if (to.Type == typeof(string))
			{
				self = new Value(ToString());
				return true;
			}
			if (to.Type == typeof(Vector3d))
			{
				self = new Value(to, Native);
				return true;
			}
			if (to.Type == typeof(Vector3))
			{
				self = new Value(to, (Vector3)Native);
				return true;
			}
			if (to.Type == typeof(Vector2d))
			{
				self = new Value(to, new Vector2d(X, Y));
				return true;
			}
			if (to.Type == typeof(Vector2))
			{
				self = new Value(to, new Vector2((float)X, (float)Y));
				return true;
			}
			return false;
		}
	}
}

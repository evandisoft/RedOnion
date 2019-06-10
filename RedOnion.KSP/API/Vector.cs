using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Creator(typeof(VectorCreator))]
	public partial class Vector : InteropObject, IEquatable<Vector>
	{
		public static MemberList MemberList { get; } = new MemberList(

@"3D vector / coordinate. All the usual operators were implemented,
multiplication and division can use both vector (per-axis) and number (all-axes).
Beware that multiplication is scaling, not cross product or dot - use appropriate function for these.",

		new IMember[]
		{
			new Native<Vector, Vector3d>(
				"native",
				"Native Vector3d (`double x,y,z`).",
				v => v.Native, (v, value) => v.Native = value),
			new Native<Vector, Vector3>(
				"vector3",
				"Native UnityEngine.Vector3 (`float x,y,z`).",
				v => v.Native, (v, value) => v.Native = value),
			new Native<Vector, Vector2>(
				"vector2",
				"Native UnityEngine.Vector2 (`float x,y`).",
				v => (Vector3)v.Native, (v, value) => v.Native = (Vector3)value),
			new Double<Vector>(
				"x", "The X-coordinate",
				v => v.X, (v, value) => v.X = value),
			new Double<Vector>(
				"y", "The Y-coordinate",
				v => v.Y, (v, value) => v.Y = value),
			new Double<Vector>(
				"z", "The Z-coordinate",
				v => v.Z, (v, value) => v.Z = value),
			new Function(
				"scale", "void",
				"Scale the vector by a factor (all axes if number is provided, per-axis if Vector)."
				+ " Multiplication does the same.",
				() => ScaleMethod.Instance),
			new Function(
				"shrink", "void",
				"Shrink the vector by a factor (all axes if number is provided, per-axis if Vector)."
				+ " Division does the same.",
				() => ShrinkMethod.Instance),
			new Double<Vector>(
				"size",
				"Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.",
				v => v.Size, (v, value) => v.Size = value),
			new Double<Vector>(
				"magnitude",
				"Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.",
				v => v.Size, (v, value) => v.Size = value),
			new Double<Vector>(
				"squareSize",
				"Square size of the vector - `x*x+y*y+z*z`. Scale if setting.",
				v => v.SquareSize, (v, value) => v.SquareSize = value),
			new Interop<Vector>(
				"normalized", "Vector",
				"Get normalized vector (size 1).",
				v => v.Normalized),
		});

		public Vector() : base(MemberList) { }
		public Vector(Vector src) : this() => Native = src.Native;
		public Vector(Vector3d native) : this() => Native = native;
		public Vector(Vector3d native, bool readOnly) : this(native) => ReadOnly = readOnly;
		public Vector(Vector3 native) : this() => Native = native;
		public Vector(Vector2d native) : this() => Native = native;
		public Vector(Vector2 native) : this() => Native = new Vector3d(native.x, native.y);
		public Vector(double x, double y, double z) : this() => Native = new Vector3d(x, y, z);
		public Vector(double x, double y) : this() => Native = new Vector3d(x, y);
		public Vector(double all) : this() => Native = new Vector3d(all, all, all);
		public Vector(IList<Value> list) : this()
		{
			switch (list.Count)
			{
			case 0:
				return;
			case 1:
				var all = list[0].ToDouble();
				Native = new Vector3d(all, all, all);
				return;
			case 2:
				Native = new Vector3d(list[0].ToDouble(), list[1].ToDouble());
				return;
			default:
				Native = new Vector3d(list[0].ToDouble(), list[1].ToDouble(), list[2].ToDouble());
				return;
			}
		}

		public Vector3d native;
		public Vector3d Native
		{
			get => native;
			set => native = value;
		}
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
		public double Size
		{
			get => Native.magnitude;
			set => Native *= value/Native.magnitude;
		}
		public double Magnitude
		{
			get => Native.magnitude;
			set => Native *= value/Native.magnitude;
		}
		public double SquareSize
		{
			get => Native.sqrMagnitude;
			set => Native *= value*value/Native.magnitude;
		}
		public Vector Normalized
			=> new Vector(Native.normalized);

		public override string ToString()
			=> string.Format(Value.Culture, "[{0}, {1}, {2}]", X, Y, Z);

		public override bool Unary(ref Value self, OpCode op)
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
		public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
		{
			if (VectorCreator.ToVector3d(lhs, out var a) && VectorCreator.ToVector3d(rhs, out var b))
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
			}
			return false;
		}
		public override DynValue MetaIndex(Script script, string metaname)
		{
			switch (metaname)
			{
			case "__unm":
				return DynValue.NewCallback(Neg);
			case "__add":
				return DynValue.NewCallback(Add);
			case "__sub":
				return DynValue.NewCallback(Sub);
			case "__mul":
				return DynValue.NewCallback(Mul);
			case "__div":
				return DynValue.NewCallback(Div);
			}
			return null;
		}
		DynValue Neg(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 1)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				-VectorCreator.ToVector(args[0].ToObject()));
		}
		DynValue Add(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				+ VectorCreator.ToVector(args[1].ToObject()));
		}
		DynValue Sub(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				- VectorCreator.ToVector(args[1].ToObject()));
		}
		DynValue Mul(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				* VectorCreator.ToVector(args[1].ToObject()));
		}
		DynValue Div(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 2)
				throw new InvalidOperationException("Unexpected number of arguments: " + args.Count);
			return UserData.Create(
				VectorCreator.ToVector(args[0].ToObject())
				/ VectorCreator.ToVector(args[1].ToObject()));
		}

		public bool Equals(Vector other)
			=> other != null && Native == other.Native;
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

		public static implicit operator Vector3d(Vector v) => v.Native;
		public static implicit operator Vector3(Vector v) => v.Native;
		public static implicit operator Vector2d(Vector v) => new Vector2d(v.X, v.Y);
		public static implicit operator Vector2(Vector v) => new Vector2((float)v.X, (float)v.Y);

		public static Vector operator +(Vector a)
			=> new Vector(a.Native);
		public static Vector operator -(Vector a)
			=> new Vector(-a.Native);

		public static Vector operator +(Vector a, Vector b)
			=> new Vector(a.Native + b.Native);
		public static Vector operator +(Vector3d a, Vector b)
			=> new Vector(a + b.Native);
		public static Vector operator +(Vector a, Vector3d b)
			=> new Vector(a.Native + b);
		public static Vector operator +(Vector3 a, Vector b)
			=> new Vector(a + b.Native);
		public static Vector operator +(Vector a, Vector3 b)
			=> new Vector(a.Native + b);

		public static Vector operator -(Vector a, Vector b)
			=> new Vector(a.Native - b.Native);
		public static Vector operator -(Vector3d a, Vector b)
			=> new Vector(a - b.Native);
		public static Vector operator -(Vector a, Vector3d b)
			=> new Vector(a.Native - b);
		public static Vector operator -(Vector3 a, Vector b)
			=> new Vector(a - b.Native);
		public static Vector operator -(Vector a, Vector3 b)
			=> new Vector(a.Native - b);

		public static Vector operator *(Vector a, Vector b)
			=> new Vector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		public static Vector operator *(Vector3d a, Vector b)
			=> new Vector(a.x * b.X, a.y * b.Y, a.z * b.Z);
		public static Vector operator *(Vector a, Vector3d b)
			=> new Vector(a.X * b.x, a.Y * b.y, a.Z * b.z);
		public static Vector operator *(Vector3 a, Vector b)
			=> new Vector(a.x * b.X, a.y * b.Y, a.z * b.Z);
		public static Vector operator *(Vector a, Vector3 b)
			=> new Vector(a.X * b.x, a.Y * b.y, a.Z * b.z);

		public static Vector operator /(Vector a, Vector b)
			=> new Vector(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		public static Vector operator /(Vector3d a, Vector b)
			=> new Vector(a.x / b.X, a.y / b.Y, a.z / b.Z);
		public static Vector operator /(Vector a, Vector3d b)
			=> new Vector(a.X / b.x, a.Y / b.y, a.Z / b.z);
		public static Vector operator /(Vector3 a, Vector b)
			=> new Vector(a.x / b.X, a.y / b.Y, a.z / b.Z);
		public static Vector operator /(Vector a, Vector3 b)
			=> new Vector(a.X / b.x, a.Y / b.y, a.Z / b.z);

		public double this[int i]
		{
			get => native[i];
			set => native[i] = value;
		}
		/* TODO
		public override Value IndexGet(Value index)
			=> index.IsNumber ? new Value(native[index.Int]) : Get(index.String);
		public override bool IndexSet(Value index, Value value)
		{
			if (index.IsNumber)
			{
				native[index.Int] = value.Double;
				return true;
			}
			return Set(index.String, value);
		}
		*/
		public override DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type == DataType.Number)
				return DynValue.NewNumber(native[(int)index.Number]);
			return base.Index(script, index, isDirectIndexing);
		}
		public override bool SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			if (index.Type == DataType.Number)
			{
				native[(int)index.Number] = value.Number;
				return true;
			}
			return base.SetIndex(script, index, value, isDirectIndexing);
		}
	}
}

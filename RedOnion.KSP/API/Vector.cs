using MoonSharp.Interpreter;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using System;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Creator(typeof(VectorCreator))]
	public partial class Vector : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.Proxy|ObjectFeatures.Operators,

		"3D vector / coordinate",

		new IMember[]
		{
			new Native<Vector, Vector3d>(
				"native",
				"Native Vector3d (convertible to UnityEngine.Vector3).",
				v => v.Native, (v, value) => v.Native = value),
			new Native<Vector, Vector3>(
				"vector3",
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
		public Vector(Vector src) : this() => Native = src.Native;
		public Vector(Vector3d native) : this() => Native = native;
		public Vector(Vector3d native, bool readOnly) : this(native) => ReadOnly = readOnly;
		public Vector(Vector3 native) : this() => Native = native;
		public Vector(Vector2d native) : this() => Native = native;
		public Vector(Vector2 native) : this() => Native = new Vector3d(native.x, native.y);
		public Vector(double x, double y, double z) : this() => Native = new Vector3d(x, y, z);
		public Vector(double x, double y) : this() => Native = new Vector3d(x, y);
		public Vector(double all) : this() => Native = new Vector3d(all, all, all);
		public Vector(IListObject list) : this()
		{
			switch (list.Count)
			{
			case 0:
				return;
			case 1:
				var all = list[0].Double;
				Native = new Vector3d(all, all, all);
				return;
			case 2:
				Native = new Vector3d(list[0].Double, list[1].Double);
				return;
			default:
				Native = new Vector3d(list[0].Double, list[1].Double, list[2].Double);
				return;
			}
		}

		public Vector3d Native { get; set; }
		public override object Target => Native;
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

		public override bool Operator(OpCode op, Value arg, bool selfRhs, out Value result)
		{
			if (op.Unary())
			{
				switch (op)
				{
				case OpCode.Plus:
					result = new Value(new Vector(Native));
					return true;
				case OpCode.Neg:
					result = new Value(new Vector(-Native));
					return true;
				}
				result = new Value();
				return false;
			}
			if (VectorCreator.ToVector3d(arg, out var b))
			{
				var a = Native;
				if (selfRhs)
				{
					a = b;
					b = Native;
				}
				switch (op)
				{
				case OpCode.Add:
					result = new Value(new Vector(a + b));
					return true;
				case OpCode.Sub:
					result = new Value(new Vector(a - b));
					return true;
				case OpCode.Mul:
					result = new Value(new Vector(a.x * b.x, a.y * b.y, a.z * b.z));
					return true;
				case OpCode.Div:
					result = new Value(new Vector(a.x / b.x, a.y / b.y, a.z / b.z));
					return true;
				}
			}
			result = new Value();
			return false;
		}
		public override DynValue LuaOperator(string metaname)
		{
			switch (metaname)
			{
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
	}
}

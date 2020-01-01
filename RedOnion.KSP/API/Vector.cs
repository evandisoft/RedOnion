using System;
using System.Collections.Generic;
using System.ComponentModel;
using MoonSharp.Interpreter;
using RedOnion.ROS;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Creator(typeof(VectorCreator))]
	[Description(
@"3D vector / coordinate. All the usual operators were implemented,
multiplication and division can use both vector (per-axis) and number (all-axes).
Beware that multiplication is scaling, not cross product or dot - use appropriate function for these.")]
	public struct Vector : IEquatable<Vector>, IOperators, IConvert
	{
		Vector3d _native;

		[Description("Native KSP `Vector3d` (`double x, y, z`).")]
		public Vector3d native
		{
			get => _native;
			set => _native = value;
		}

		public override string ToString()
			=> string.Format(Value.Culture, "[{0}, {1}, {2}]", x, y, z);

		public Vector(Vector src) => _native = src.native;
		public Vector(Vector3d src) => _native = src;
		public Vector(Vector3 src) => _native = src;
		public Vector(Vector2d src) => _native = src;
		public Vector(Vector2 src) => _native = new Vector3d(src.x, src.y);
		public Vector(double x, double y, double z) => _native = new Vector3d(x, y, z);
		public Vector(double x, double y) => _native = new Vector3d(x, y);
		public Vector(double all) => _native = new Vector3d(all, all, all);

		public Vector(IList<Value> src)
		{
			switch (src.Count)
			{
			case 0:
				_native = new Vector3d();
				return;
			case 1:
				var all = src[0].ToDouble();
				_native = new Vector3d(all, all, all);
				return;
			case 2:
				_native = new Vector3d(src[0].ToDouble(), src[1].ToDouble());
				return;
			default:
				_native = new Vector3d(src[0].ToDouble(), src[1].ToDouble(), src[2].ToDouble());
				return;
			}
		}
		public Vector(IList<double> src)
		{
			switch (src.Count)
			{
			case 0:
				_native = new Vector3d();
				return;
			case 1:
				var all = src[0];
				_native = new Vector3d(all, all, all);
				return;
			case 2:
				_native = new Vector3d(src[0], src[1]);
				return;
			default:
				_native = new Vector3d(src[0], src[1], src[2]);
				return;
			}
		}
		public Vector(IList<float> src)
		{
			switch (src.Count)
			{
			case 0:
				_native = new Vector3d();
				return;
			case 1:
				var all = src[0];
				_native = new Vector3d(all, all, all);
				return;
			case 2:
				_native = new Vector3d(src[0], src[1]);
				return;
			default:
				_native = new Vector3d(src[0], src[1], src[2]);
				return;
			}
		}

		[Description("The X-coordinate")]
		public double x
		{
			get => _native.x;
			set => _native.x = value;
		}
		[Description("The Y-coordinate")]
		public double y
		{
			get => _native.y;
			set => _native.y = value;
		}
		[Description("The Z-coordinate")]
		public double z
		{
			get => _native.z;
			set => _native.z = value;
		}

		[Description("Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public double size
		{
			get => _native.magnitude;
			set => _native *= value/ _native.magnitude;
		}
		[Description("Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public double magnitude
		{
			get => _native.magnitude;
			set => _native *= value/ _native.magnitude;
		}
		[Description("Square size of the vector - `x*x+y*y+z*z`. Scale if setting.")]
		public double squareSize
		{
			get => _native.sqrMagnitude;
			set => _native *= value* value / _native.magnitude;
		}

		[Description("Get normalized vector (size 1).")]
		public Vector normalized => new Vector(_native.normalized);

		[Description("Dot product of this vector and another vector.")]
		public double dot(Vector rhs)
			=> Vector3d.Dot(native, rhs.native);
		[Description("Angle between this vector and another vector (0..180).")]
		public double angle(Vector rhs)
			=> Vector3d.Angle(native, rhs.native);
		[Description("Angle between this vector and another vector given point above the plane (-180..180)."
			+ " Note that the vectors are not projected onto the plane,"
			+ " the angle of cross product of the two and the third vector being above 90 makes the result negative.")]
		public double angle(Vector rhs, Vector axis)
		{
			var a = angle(rhs);
			if (Vector3d.Angle(axis.native, Vector3d.Cross(native, rhs.native)) > 90)
				a = -a;
			return a;
		}
		[Description("Cross product of this vector with another vector."
			+ " Note that Unity uses left-handed coordinate system, so `ship.away.cross(ship.velocity)`"
			+ " points down (towards south pole) in prograde-orbit (which is the usual).")]
		public Vector cross(Vector rhs)
			=> new Vector(Vector3d.Cross(native, rhs));
		[Description("Project this vector onto another vector.")]
		public Vector projectOnVector(Vector normal)
			=> new Vector(Vector3d.Project(native, normal));
		[Description("Project this vector onto plane specified by normal vector.")]
		public Vector projectOnPlane(Vector normal)
			=> new Vector(Vector3d.Exclude(normal, native));
		[Description("Project this vector onto another vector (alias to `projectOnVector`).")]
		public Vector project(Vector normal)
			=> new Vector(Vector3d.Project(native, normal));
		[Description("Project this vector onto plane specified by normal vector (alias to `projectOnPlane`).")]
		public Vector exclude(Vector normal)
			=> new Vector(Vector3d.Exclude(normal, native));
		[Description("Rotate vector by an angle around axis.")]
		public Vector rotate(double angle, Vector axis)
			=> new Vector(QuaternionD.AngleAxis(angle, axis) * native);

		public static implicit operator Vector3d(Vector v) => v.native;
		public static implicit operator Vector3(Vector v) => v.native;
		public static implicit operator Vector2d(Vector v) => new Vector2d(v.x, v.y);
		public static implicit operator Vector2(Vector v) => new Vector2((float)v.x, (float)v.y);
		public static explicit operator Vector(Vector3d v) => new Vector(v);
		public static explicit operator Vector(Vector3 v) => new Vector(v);
		public static explicit operator Vector(Vector2d v) => new Vector(v);
		public static explicit operator Vector(Vector2 v) => new Vector(v);

		[Description("UnityEngine.Vector3 (`float x,y,z`).")]
		public Vector3 Vector3
		{
			get => _native;
			set => _native = value;
		}
		[Description("UnityEngine.Vector2 (`float x,y`).")]
		public Vector2 Vector2
		{
			get => Vector3;
			set => Vector3 = value;
		}

		/* removed all modifying methods (and index_set)

		[Description("Index the coordinates as double[3]")]
		public double this[int i]
		{
			get => _native[i];
			set => _native[i] = value;
		}

		[Description("Normalize vector (set size to 1).")]
		public void normalize()
			=> _native = _native.normalized;

		[Description("Scale the vector by a factor (all axes). Multiplication does the same.")]
		public void scale(double factor)
			=> _native *= factor;
		[Description("Scale individual axis. Multiplication does the same.")]
		public void scale(Vector v)
			=> _native = Vector3d.Scale(_native, v.native);

		public void scale(Vector3d v)
			=> _native = Vector3d.Scale(_native, v);
		public void scale(Vector3 v)
			=> _native = Vector3d.Scale(_native, v);

		[Description("Shrink the vector by a factor (all axes). Division does the same.")]
		public void shrink(double factor)
			=> _native /= factor;
		[Description("Shrink individual axis. Division does the same.")]
		public void shrink(Vector v)
			=> _native = new Vector3d(x / v.x, y / v.y, z / v.z);

		public void shrink(Vector3d v)
			=> _native = new Vector3d(x / v.x, y / v.y, z / v.z);
		public void shrink(Vector3 v)
			=> _native = new Vector3d(x / v.x, y / v.y, z / v.z);

		*/

		public static Vector operator +(Vector a)
			=> new Vector(a.native);
		public static Vector operator -(Vector a)
			=> new Vector(-a.native);

		public static Vector operator +(Vector a, Vector b)
			=> new Vector(a.native + b.native);
		public static Vector operator +(Vector3d a, Vector b)
			=> new Vector(a + b.native);
		public static Vector operator +(Vector a, Vector3d b)
			=> new Vector(a.native + b);
		public static Vector operator +(Vector3 a, Vector b)
			=> new Vector(a + b.native);
		public static Vector operator +(Vector a, Vector3 b)
			=> new Vector(a.native + b);

		public static Vector operator -(Vector a, Vector b)
			=> new Vector(a.native - b.native);
		public static Vector operator -(Vector3d a, Vector b)
			=> new Vector(a - b.native);
		public static Vector operator -(Vector a, Vector3d b)
			=> new Vector(a.native - b);
		public static Vector operator -(Vector3 a, Vector b)
			=> new Vector(a - b.native);
		public static Vector operator -(Vector a, Vector3 b)
			=> new Vector(a.native - b);

		public static Vector operator *(Vector a, Vector b)
			=> new Vector(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vector operator *(Vector3d a, Vector b)
			=> new Vector(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vector operator *(Vector a, Vector3d b)
			=> new Vector(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vector operator *(Vector3 a, Vector b)
			=> new Vector(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Vector operator *(Vector a, Vector3 b)
			=> new Vector(a.x * b.x, a.y * b.y, a.z * b.z);

		public static Vector operator /(Vector a, Vector b)
			=> new Vector(a.x / b.x, a.y / b.y, a.z / b.z);
		public static Vector operator /(Vector3d a, Vector b)
			=> new Vector(a.x / b.x, a.y / b.y, a.z / b.z);
		public static Vector operator /(Vector a, Vector3d b)
			=> new Vector(a.x / b.x, a.y / b.y, a.z / b.z);
		public static Vector operator /(Vector3 a, Vector b)
			=> new Vector(a.x / b.x, a.y / b.y, a.z / b.z);
		public static Vector operator /(Vector a, Vector3 b)
			=> new Vector(a.x / b.x, a.y / b.y, a.z / b.z);

		public static Vector operator *(Vector a, double f)
			=> new Vector(a.x * f, a.y * f, a.z * f);
		public static Vector operator *(double f, Vector b)
			=> new Vector(f * b.x, f * b.y, f * b.z);
		public static Vector operator /(Vector a, double f)
			=> new Vector(a.x / f, a.y / f, a.z / f);

		public bool Equals(Vector other)
			=> _native == other.native;
		public override bool Equals(object obj)
		{
			if (obj is Vector v)
				return Equals(v);
			if (obj is Vector3d v3d)
				return native == v3d;
			if (obj is Vector3 v3)
				return (Vector3)native == v3;
			if (z < 0 || z > 0) // zero and NaN are acceptable
				return false;
			if (obj is Vector2d v2d)
				return x == v2d.x && y == v2d.y;
			if (obj is Vector2 v2)
				return (float)x == v2.x && (float)y == v2.y;
			return false;
		}
		public override int GetHashCode()
			=> native.GetHashCode();

		bool IOperators.Unary(ref Value self, OpCode op)
		{
			switch (op)
			{
			case OpCode.Plus:
				self = new Value(new Vector(native));
				return true;
			case OpCode.Neg:
				self = new Value(new Vector(-native));
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
				if (rhs.IsNumberOrChar)
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
			if (op == OpCode.Mul && lhs.IsNumberOrChar && VectorCreator.ToVector3d(rhs, out var v))
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
			if (to.Type == typeof(Vector))
			{
				self = new Value(new Vector((Vector)self.obj));
				return true;
			}
			if (to.Type == typeof(Vector))
			{
				self = new Value(new Vector((Vector)self.obj));
				return true;
			}
			if (to.Type == typeof(string))
			{
				self = new Value(ToString());
				return true;
			}
			if (to.Type == typeof(Vector3d))
			{
				self = new Value(to, native);
				return true;
			}
			if (to.Type == typeof(Vector3))
			{
				self = new Value(to, (Vector3)native);
				return true;
			}
			if (to.Type == typeof(Vector2d))
			{
				self = new Value(to, new Vector2d(x, y));
				return true;
			}
			if (to.Type == typeof(Vector2))
			{
				self = new Value(to, new Vector2((float)x, (float)y));
				return true;
			}
			return false;
		}

		[Browsable(false), MoonSharpHidden]
		public static readonly Vector zero = new Vector(Vector3d.zero);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector one = new Vector(Vector3d.one);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector forward = new Vector(Vector3d.forward);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector fwd = forward;
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector back = new Vector(Vector3d.back);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector up = new Vector(Vector3d.up);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector down = new Vector(Vector3d.down);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector left = new Vector(Vector3d.left);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector right = new Vector(Vector3d.right);
		[Browsable(false), MoonSharpHidden]
		public static readonly Vector none = new Vector(new Vector3d(double.NaN, double.NaN, double.NaN));
	}
}

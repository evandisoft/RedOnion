using System;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.ROS;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Creator(typeof(VectorCreator))]
	[Description("3D vector / coordinate. All the usual operators were implemented,"
		+ " multiplication and division can use both vector (per-axis) and number (all-axes)."
		+ " Beware that multiplication is scaling, not cross product or dot - use appropriate function for these."
		+ " See also [ConstVector.md](ConstVector) which represents read-only base class.")]
	public class Vector : ConstVector
	{
		public Vector() { }
		public Vector(ConstVector src) : base(src) { }
		public Vector(Vector src) : base(src) { }
		public Vector(Vector3d src) : base(src) { }
		public Vector(Vector3 src) : base(src) { }
		public Vector(Vector2d src) : base(src) { }
		public Vector(Vector2 src) : base(src) { }
		public Vector(double x, double y, double z) : base(x, y, z) { }
		public Vector(double x, double y) : base(x, y) { }
		public Vector(double all) : base(all) { }
		public Vector(IList<Value> src) : base(src) { }
		public Vector(IList<double> src) : base(src) { }
		public Vector(IList<float> src) : base(src) { }

		[Description("Native Vector3d(`double x, y, z`).")]
		public new Vector3d Native
		{
			get => native;
			set => native = value;
		}

		[Description("The X-coordinate")]
		public new double X
		{
			get => native.x;
			set => native = new Vector3d(value, Y, Z);
		}
		[Description("The Y-coordinate")]
		public new double Y
		{
			get => native.y;
			set => native = new Vector3d(X, value, Z);
		}
		[Description("The Z-coordinate")]
		public new double Z
		{
			get => native.z;
			set => native = new Vector3d(X, Y, value);
		}

		[Description("Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public new double Size
		{
			get => native.magnitude;
			set => native *= value/native.magnitude;
		}
		[Description("Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public new double Magnitude
		{
			get => native.magnitude;
			set => native *= value/native.magnitude;
		}
		[Description("Square size of the vector - `x*x+y*y+z*z`. Scale if setting.")]
		public new double SquareSize
		{
			get => native.sqrMagnitude;
			set => native *= value*value/native.magnitude;
		}
		[Description("Normalize vector (set size to 1).")]
		public void Normalize()
			=> native = native.normalized;

		[Description("Native UnityEngine.Vector3 (`float x,y,z`).")]
		public new Vector3 Vector3
		{
			get => native;
			set => native = value;
		}
		[Description("Native UnityEngine.Vector2 (`float x,y`).")]
		public new Vector2 Vector2
		{
			get => Vector3;
			set => Vector3 = value;
		}
		[Description("Index the coordinates as double[3]")]
		public new double this[int i]
		{
			get => native[i];
			set => native[i] = value;
		}

		[Description("Scale the vector by a factor (all axes). Multiplication does the same.")]
		public void Scale(double factor)
			=> native *= factor;
		[Description("Scale individual axis. Multiplication does the same.")]
		public void Scale(ConstVector v)
			=> native = Vector3d.Scale(native, v.Native);

		public void Scale(Vector3d v)
			=> native = Vector3d.Scale(native, v);
		public void Scale(Vector3 v)
			=> native = Vector3d.Scale(native, v);

		[Description("Shrink the vector by a factor (all axes). Division does the same.")]
		public void Shrink(double factor)
			=> native /= factor;
		[Description("Shrink individual axis. Division does the same.")]
		public void Shrink(ConstVector v)
			=> native = new Vector3d(X / v.X, Y / v.Y, Z / v.Z);

		public void Shrink(Vector3d v)
			=> native = new Vector3d(X / v.x, Y / v.y, Z / v.z);
		public void Shrink(Vector3 v)
			=> native = new Vector3d(X / v.x, Y / v.y, Z / v.z);

		public static implicit operator Vector3d(Vector v) => v.Native;
		public static implicit operator Vector3(Vector v) => v.Native;
		public static implicit operator Vector2d(Vector v) => new Vector2d(v.X, v.Y);
		public static implicit operator Vector2(Vector v) => new Vector2((float)v.X, (float)v.Y);
		public static explicit operator Vector(Vector3d v) => new Vector(v);
		public static explicit operator Vector(Vector3 v) => new Vector(v);
		public static explicit operator Vector(Vector2d v) => new Vector(v);
		public static explicit operator Vector(Vector2 v) => new Vector(v);
	}
}

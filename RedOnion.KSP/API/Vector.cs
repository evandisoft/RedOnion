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
		public new Vector3d native
		{
			get => base.protectedNative;
			set => base.protectedNative = value;
		}

		[Description("The X-coordinate")]
		public new double x
		{
			get => base.protectedNative.x;
			set => base.protectedNative = new Vector3d(value, y, z);
		}
		[Description("The Y-coordinate")]
		public new double y
		{
			get => base.protectedNative.y;
			set => base.protectedNative = new Vector3d(x, value, z);
		}
		[Description("The Z-coordinate")]
		public new double z
		{
			get => base.protectedNative.z;
			set => base.protectedNative = new Vector3d(x, y, value);
		}

		[Description("Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public new double size
		{
			get => base.protectedNative.magnitude;
			set => base.protectedNative *= value/ base.protectedNative.magnitude;
		}
		[Description("Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public new double magnitude
		{
			get => base.protectedNative.magnitude;
			set => base.protectedNative *= value/ base.protectedNative.magnitude;
		}
		[Description("Square size of the vector - `x*x+y*y+z*z`. Scale if setting.")]
		public new double squareSize
		{
			get => base.protectedNative.sqrMagnitude;
			set => base.protectedNative *= value* value / base.protectedNative.magnitude;
		}
		[Description("Normalize vector (set size to 1).")]
		public void normalize()
			=> base.protectedNative = base.protectedNative.normalized;

		[Description("Native UnityEngine.Vector3 (`float x,y,z`).")]
		public new Vector3 Vector3
		{
			get => base.protectedNative;
			set => base.protectedNative = value;
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
			get => base.protectedNative[i];
			set => base.protectedNative[i] = value;
		}

		[Description("Scale the vector by a factor (all axes). Multiplication does the same.")]
		public void scale(double factor)
			=> base.protectedNative *= factor;
		[Description("Scale individual axis. Multiplication does the same.")]
		public void scale(ConstVector v)
			=> base.protectedNative = Vector3d.Scale(base.protectedNative, v.native);

		public void scale(Vector3d v)
			=> base.protectedNative = Vector3d.Scale(base.protectedNative, v);
		public void scale(Vector3 v)
			=> base.protectedNative = Vector3d.Scale(base.protectedNative, v);

		[Description("Shrink the vector by a factor (all axes). Division does the same.")]
		public void shrink(double factor)
			=> base.protectedNative /= factor;
		[Description("Shrink individual axis. Division does the same.")]
		public void shrink(ConstVector v)
			=> base.protectedNative = new Vector3d(x / v.x, y / v.y, z / v.z);

		public void shrink(Vector3d v)
			=> base.protectedNative = new Vector3d(x / v.x, y / v.y, z / v.z);
		public void shrink(Vector3 v)
			=> base.protectedNative = new Vector3d(x / v.x, y / v.y, z / v.z);

		public static implicit operator Vector3d(Vector v) => v.native;
		public static implicit operator Vector3(Vector v) => v.native;
		public static implicit operator Vector2d(Vector v) => new Vector2d(v.x, v.y);
		public static implicit operator Vector2(Vector v) => new Vector2((float)v.x, (float)v.y);
		public static explicit operator Vector(Vector3d v) => new Vector(v);
		public static explicit operator Vector(Vector3 v) => new Vector(v);
		public static explicit operator Vector(Vector2d v) => new Vector(v);
		public static explicit operator Vector(Vector2 v) => new Vector(v);
	}
}

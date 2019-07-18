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
			get => _native;
			set => _native = value;
		}

		[Description("The X-coordinate")]
		public new double x
		{
			get => _native.x;
			set => _native = new Vector3d(value, y, z);
		}
		[Description("The Y-coordinate")]
		public new double y
		{
			get => _native.y;
			set => _native = new Vector3d(x, value, z);
		}
		[Description("The Z-coordinate")]
		public new double z
		{
			get => _native.z;
			set => _native = new Vector3d(x, y, value);
		}

		[Description("Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public new double size
		{
			get => _native.magnitude;
			set => _native *= value/ _native.magnitude;
		}
		[Description("Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.")]
		public new double magnitude
		{
			get => _native.magnitude;
			set => _native *= value/ _native.magnitude;
		}
		[Description("Square size of the vector - `x*x+y*y+z*z`. Scale if setting.")]
		public new double squareSize
		{
			get => _native.sqrMagnitude;
			set => _native *= value* value / _native.magnitude;
		}
		[Description("Normalize vector (set size to 1).")]
		public void normalize()
			=> _native = _native.normalized;

		[Description("Native UnityEngine.Vector3 (`float x,y,z`).")]
		public new Vector3 Vector3
		{
			get => _native;
			set => _native = value;
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
			get => _native[i];
			set => _native[i] = value;
		}

		[Description("Scale the vector by a factor (all axes). Multiplication does the same.")]
		public void scale(double factor)
			=> _native *= factor;
		[Description("Scale individual axis. Multiplication does the same.")]
		public void scale(ConstVector v)
			=> _native = Vector3d.Scale(_native, v.native);

		public void scale(Vector3d v)
			=> _native = Vector3d.Scale(_native, v);
		public void scale(Vector3 v)
			=> _native = Vector3d.Scale(_native, v);

		[Description("Shrink the vector by a factor (all axes). Division does the same.")]
		public void shrink(double factor)
			=> _native /= factor;
		[Description("Shrink individual axis. Division does the same.")]
		public void shrink(ConstVector v)
			=> _native = new Vector3d(x / v.x, y / v.y, z / v.z);

		public void shrink(Vector3d v)
			=> _native = new Vector3d(x / v.x, y / v.y, z / v.z);
		public void shrink(Vector3 v)
			=> _native = new Vector3d(x / v.x, y / v.y, z / v.z);

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

using System;
using System.Globalization;
using UnityEngine;

namespace RedOnion.UI
{
	[Serializable]
	public struct Padding : IEquatable<Padding>
	{
		[SerializeField]
		public float left;
		[SerializeField]
		public float xgap;
		[SerializeField]
		public float right;

		[SerializeField]
		public float top;
		[SerializeField]
		public float ygap;
		[SerializeField]
		public float bottom;

		public static bool operator ==(Padding lhs, Padding rhs)
			=> lhs.Equals(rhs);
		public static bool operator !=(Padding lhs, Padding rhs)
			=> !lhs.Equals(rhs);
		public bool Equals(Padding other)
			=> left == other.left && xgap == other.xgap && right == other.right
			&& top == other.top && ygap == other.ygap && bottom == other.bottom;
		public override bool Equals(object obj)
			=> obj is Padding && Equals((Padding)obj);
		public override int GetHashCode() // some arbitrarily selected primes
			=> unchecked(left.GetHashCode() * 41 + xgap.GetHashCode() * 107 + right.GetHashCode() * 173
			+ top.GetHashCode() * 239 + ygap.GetHashCode() * 433 + bottom.GetHashCode() * 587);

		public Padding(float all)
		{
			left	= all;
			xgap    = all;
			right   = all;
			top     = all;
			ygap    = all;
			bottom  = all;
		}
		public Padding(float horizontal, float vertical)
		{
			left    = horizontal;
			xgap    = horizontal;
			right   = horizontal;
			top     = vertical;
			ygap    = vertical;
			bottom  = vertical;
		}
		public Padding(float xOuter, float xInnter, float yOuter, float yInner)
		{
			left   = xOuter;
			xgap   = xInnter;
			right  = xOuter;
			top    = yOuter;
			ygap   = yInner;
			bottom = yOuter;
		}
		public Padding(float left, float xgap, float right, float top, float ygap, float bottom)
		{
			this.left	= left;
			this.xgap   = xgap;
			this.right  = right;
			this.top	= top;
			this.ygap   = ygap;
			this.bottom = bottom;
		}

		public float All
		{
			get => left == xgap && xgap == right && right == top && top == ygap && ygap == bottom
				? left : float.NaN;
			set
			{
				left    = value;
				xgap    = value;
				right   = value;
				top     = value;
				ygap    = value;
				bottom  = value;
			}
		}
		public float Horizontal
		{
			get => left == xgap && xgap == right ? left : float.NaN;
			set
			{
				left    = value;
				xgap    = value;
				right   = value;
			}
		}
		public float Vertical
		{
			get => top == ygap && ygap == bottom ? top : float.NaN;
			set
			{
				top     = value;
				ygap    = value;
				bottom  = value;
			}
		}

		public float Left
		{
			get => left;
			set => left = value;
		}
		public float XGap
		{
			get => xgap;
			set => xgap = value;
		}
		public float Right
		{
			get => right;
			set => right = value;
		}
		public float Top
		{
			get => top;
			set => top = value;
		}
		public float YGap
		{
			get => ygap;
			set => ygap = value;
		}
		public float Bottom
		{
			get => bottom;
			set => bottom = value;
		}

		public float X0
		{
			get => left;
			set => left = value;
		}
		public float X1
		{
			get => xgap;
			set => xgap = value;
		}
		public float X2
		{
			get => right;
			set => right = value;
		}
		public float Y0
		{
			get => top;
			set => top = value;
		}
		public float Y1
		{
			get => ygap;
			set => ygap = value;
		}
		public float Y2
		{
			get => bottom;
			set => bottom = value;
		}

		public Vector3 X3
		{
			get => new Vector3(left, right, xgap);
			set
			{
				left	= value.x;
				xgap    = value.y;
				right   = value.z;
			}
		}
		public Vector3 Y3
		{
			get => new Vector3(top, bottom, ygap);
			set
			{
				top     = value.x;
				ygap    = value.y;
				bottom  = value.z;
			}
		}

		public Padding(Vector3 x, Vector3 y)
		{
			left    = x.x;
			xgap    = x.y;
			right   = x.z;
			top     = y.x;
			ygap    = y.y;
			bottom  = y.z;
		}

		public override string ToString()
			=> string.Format(CultureInfo.InvariantCulture,
				"({0}:{1}:{2}; {3}:{4}:{5})",
				left, xgap, right,
				top, ygap, bottom);

		public float GetOuter(bool verticalAxis)
			=> verticalAxis ? top + bottom : left + right;
		public float GetInner(bool verticalAxis)
			=> verticalAxis ? ygap : xgap;
		public float GetFirst(bool verticalAxis)
			=> verticalAxis ? top : left;
		public float GetLast(bool verticalAxis)
			=> verticalAxis ? bottom : right;
	}
}

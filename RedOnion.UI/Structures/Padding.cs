using System;
using System.Globalization;
using UnityEngine;

namespace RedOnion.UI
{
	/// <summary>
	/// Element's inner padding (4 values)
	/// </summary>
	[Serializable]
	public struct Padding : IEquatable<Padding>
	{
		[SerializeField]
		public float left;
		[SerializeField]
		public float right;
		[SerializeField]
		public float top;
		[SerializeField]
		public float bottom;

		public static bool operator ==(Padding lhs, Padding rhs)
			=> lhs.Equals(rhs);
		public static bool operator !=(Padding lhs, Padding rhs)
			=> !lhs.Equals(rhs);
		public bool Equals(Padding other)
			=> left == other.left && right == other.right
			&& top == other.top && bottom == other.bottom;
		public override bool Equals(object obj)
			=> obj is Padding && Equals((Padding)obj);
		public override int GetHashCode() // same primes as in Anchors
			=> unchecked(left.GetHashCode() * 37 + right.GetHashCode() * 101
			+ top.GetHashCode() * 277 + bottom.GetHashCode() * 613);

		public Padding(float all)
		{
			left    = all;
			right   = all;
			top     = all;
			bottom  = all;
		}
		public Padding(float horizontal, float vertical)
		{
			left    = horizontal;
			right   = horizontal;
			top     = vertical;
			bottom  = vertical;
		}
		public Padding(float left, float right, float top, float bottom)
		{
			this.left   = left;
			this.right  = right;
			this.top    = top;
			this.bottom = bottom;
		}

		public float All
		{
			get => left == right && right == top && top == bottom
				? left : float.NaN;
			set
			{
				left    = value;
				right   = value;
				top     = value;
				bottom  = value;
			}
		}
		public float Horizontal
		{
			get => left == right ? left : float.NaN;
			set
			{
				left    = value;
				right   = value;
			}
		}
		public float Vertical
		{
			get => top == bottom ? top : float.NaN;
			set
			{
				top     = value;
				bottom  = value;
			}
		}

		public float Left
		{
			get => left;
			set => left = value;
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
		public float Bottom
		{
			get => bottom;
			set => bottom = value;
		}

		public override string ToString()
			=> string.Format(CultureInfo.InvariantCulture,
				"({0}:{1}; {2}:{3})", left, right, top, bottom);

		public float GetFirst(bool verticalAxis)
			=> verticalAxis ? top : left;
		public float GetLast(bool verticalAxis)
			=> verticalAxis ? bottom : right;

		public RectOffset ToRectOffset()
			=> new RectOffset(
				Mathf.RoundToInt(left),
				Mathf.RoundToInt(right),
				Mathf.RoundToInt(top),
				Mathf.RoundToInt(bottom));
	}
}

using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

namespace RedOnion.UI
{
	[Serializable, Description("Element's inner padding (4 values).")]
	public struct Padding : IEquatable<Padding>
	{
		[SerializeField, Description("Padding on the left side.")]
		public float left;
		[SerializeField, Description("Padding on the right side.")]
		public float right;
		[SerializeField, Description("Padding above the content.")]
		public float top;
		[SerializeField, Description("Padding below the content.")]
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

		[Description("Set all values to the one specified.")]
		public Padding(float all)
		{
			left    = all;
			right   = all;
			top     = all;
			bottom  = all;
		}
		[Description("Set `left = right = horizontal` and `top = bottom = vertical`.")]
		public Padding(float horizontal, float vertical)
		{
			left    = horizontal;
			right   = horizontal;
			top     = vertical;
			bottom  = vertical;
		}
		[Description("Specify all the values.")]
		public Padding(float left, float right, float top, float bottom)
		{
			this.left   = left;
			this.right  = right;
			this.top    = top;
			this.bottom = bottom;
		}

		[Description("One value for all (if same or setting), `NaN` if not.")]
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
		[Description("Value of `left` and `right` if same, `NaN` if not.")]
		public float Horizontal
		{
			get => left == right ? left : float.NaN;
			set
			{
				left    = value;
				right   = value;
			}
		}
		[Description("value of `top` and `bottom` if same, `NaN` if not.")]
		public float Vertical
		{
			get => top == bottom ? top : float.NaN;
			set
			{
				top     = value;
				bottom  = value;
			}
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

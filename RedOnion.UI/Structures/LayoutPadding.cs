using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

namespace RedOnion.UI
{
	[Serializable, Description("Inner padding and spacing (6 values).")]
	public struct LayoutPadding : IEquatable<LayoutPadding>
	{
		[SerializeField, Description("Padding on the left side.")]
		public float left;
		[SerializeField, Description("Horizontal spacing between elements.")]
		public float xgap;
		[SerializeField, Description("Padding on the right side.")]
		public float right;

		[SerializeField, Description("Padding above the content.")]
		public float top;
		[SerializeField, Description("Vertical spacing between elements.")]
		public float ygap;
		[SerializeField, Description("Padding below the content.")]
		public float bottom;

		public static bool operator ==(LayoutPadding lhs, LayoutPadding rhs)
			=> lhs.Equals(rhs);
		public static bool operator !=(LayoutPadding lhs, LayoutPadding rhs)
			=> !lhs.Equals(rhs);
		public bool Equals(LayoutPadding other)
			=> left == other.left && xgap == other.xgap && right == other.right
			&& top == other.top && ygap == other.ygap && bottom == other.bottom;
		public override bool Equals(object obj)
			=> obj is LayoutPadding && Equals((LayoutPadding)obj);
		public override int GetHashCode() // some arbitrarily selected primes
			=> unchecked(left.GetHashCode() * 41 + xgap.GetHashCode() * 107 + right.GetHashCode() * 173
			+ top.GetHashCode() * 239 + ygap.GetHashCode() * 433 + bottom.GetHashCode() * 587);

		[Description("Set all values to the one specified.")]
		public LayoutPadding(float all)
		{
			left	= all;
			xgap    = all;
			right   = all;
			top     = all;
			ygap    = all;
			bottom  = all;
		}
		[Description("Set `left = xgap = right = horizontal` and `top = ygap = bottom = vertical`.")]
		public LayoutPadding(float horizontal, float vertical)
		{
			left    = horizontal;
			xgap    = horizontal;
			right   = horizontal;
			top     = vertical;
			ygap    = vertical;
			bottom  = vertical;
		}
		[Description("Specify all the values.")]
		public LayoutPadding(float left, float xgap, float right, float top, float ygap, float bottom)
		{
			this.left	= left;
			this.xgap   = xgap;
			this.right  = right;
			this.top	= top;
			this.ygap   = ygap;
			this.bottom = bottom;
		}
		[Description("Combine `padding` (4 floats - around the content) and `spacing` (2D vector, between elements).")]
		public LayoutPadding(Padding padding, Vector2 spacing)
		{
			left    = padding.left;
			xgap    = spacing.x;
			right   = padding.right;
			top     = padding.top;
			ygap    = spacing.y;
			bottom  = padding.bottom;
		}

		[Description("One value for all (if same or setting), `NaN` if not.")]
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
		[Description("Value of `left`, `xgap` and `right` if same, `NaN` if not.")]
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
		[Description("value of `top`, `ygap` and `bottom` if same, `NaN` if not.")]
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

		public LayoutPadding(Vector3 x, Vector3 y)
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

		public RectOffset ToRectOffset()
			=> new RectOffset(
				Mathf.RoundToInt(left),
				Mathf.RoundToInt(right),
				Mathf.RoundToInt(top),
				Mathf.RoundToInt(bottom));

		[Description("The padding (`left, right, top, bottom`).")]
		public Padding Padding
		{
			get => new Padding(left, right, top, bottom);
			set
			{
				left	= value.left;
				right	= value.right;
				top		= value.top;
				bottom	= value.bottom;
			}
		}
		[Description("The spacing (`xgap, ygap`).")]
		public Vector2 Spacing
		{
			get => new Vector2(xgap, ygap);
			set
			{
				xgap = value.x;
				ygap = value.y;
			}
		}
	}
}

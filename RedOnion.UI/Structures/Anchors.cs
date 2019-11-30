using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

namespace RedOnion.UI
{
	[Serializable, Description("Anchor definition for positioning UI element within a cell.")]
	public struct Anchors : IEquatable<Anchors>
	{
		[SerializeField, Description("Fraction (0..1) of container's width to anchor left side of the element to.")]
		public float left;
		[SerializeField, Description("Fraction (0..1) of container's width to anchor right side of the element to.")]
		public float right;
		[SerializeField, Description("Fraction (0..1) of container's height to anchor top side of the element to.")]
		public float top;
		[SerializeField, Description("Fraction (0..1) of container's height to anchor bottom side of the element to.")]
		public float bottom;

		public static bool operator ==(Anchors lhs, Anchors rhs)
			=> lhs.Equals(rhs);
		public static bool operator !=(Anchors lhs, Anchors rhs)
			=> !lhs.Equals(rhs);
		public bool Equals(Anchors other)
			=> left == other.left && right == other.right
			&& top == other.top && bottom == other.bottom;
		public override bool Equals(object obj)
			=> obj is Anchors && Equals((Anchors)obj);
		public override int GetHashCode() // some primes as in Padding
			=> unchecked(left.GetHashCode() * 37 + right.GetHashCode() * 101
			+ top.GetHashCode() * 277 + bottom.GetHashCode() * 613);

		[Description("Position the element in top-left corner.")]
		public static readonly Anchors TopLeft      = new Anchors(0f, 0f, 0f, 0f);
		[Description("Position the element in top-right corner.")]
		public static readonly Anchors TopRight     = new Anchors(1f, 1f, 0f, 0f);
		[Description("Position the element in bottom-left corner.")]
		public static readonly Anchors BottomLeft   = new Anchors(0f, 0f, 1f, 1f);
		[Description("Position the element in bottom-right corner.")]
		public static readonly Anchors BottomRight  = new Anchors(1f, 1f, 1f, 1f);

		[Description("Fill the entire cell.")]
		public static readonly Anchors Fill         = new Anchors(0f, 1f, 0f, 1f);
		[Description("Fill the left side of the cell (top-down, anchor to left, keep the width).")]
		public static readonly Anchors FillLeft     = new Anchors(0f, 0f, 0f, 1f);
		[Description("Fill the right side of the cell (top-down, anchor to right, keep the width).")]
		public static readonly Anchors FillRight    = new Anchors(1f, 1f, 0f, 1f);
		[Description("Fill the top of the cell (left-right, anchor to top, keep the height).")]
		public static readonly Anchors FillTop      = new Anchors(0f, 1f, 0f, 0f);
		[Description("Fill the bottom of the cell (left-right, anchor to bottom, keep the height).")]
		public static readonly Anchors FillBottom   = new Anchors(0f, 1f, 1f, 1f);

		[Description("Position the element in the middle/center of the cell.")]
		public static readonly Anchors Middle       = new Anchors(.5f, .5f, .5f, .5f);
		[Description("Position the element in the middle of left side.")]
		public static readonly Anchors MiddleLeft   = new Anchors(0f, 0f, .5f, .5f);
		[Description("Position the element in the middle of right side.")]
		public static readonly Anchors MiddleRight  = new Anchors(1f, 1f, .5f, .5f);
		[Description("Position the element in the middle of the top.")]
		public static readonly Anchors MiddleTop    = new Anchors(.5f, .5f, 0f, 0f);
		[Description("Position the element in the middle of the bottom.")]
		public static readonly Anchors MiddleBottom = new Anchors(.5f, .5f, 1f, 1f);

		[Description("Stretch the element horizontally and place it in the middle/center (vertically).")]
		public static readonly Anchors Horizontal	= new Anchors(0f, 1f, .5f, .5f);
		[Description("Stretch the element vertically and place it in the middle/center (horizontally).")]
		public static readonly Anchors Vertical		= new Anchors(.5f, .5f, 0f, 1f);

		[Description("Marker for invalid / unused / default anchors.")]
		public static readonly Anchors Invalid      = new Anchors(float.NaN, float.NaN, float.NaN, float.NaN);

		// aliases from TextAnchor
		[Description("`TopLeft`, name taken from `TextAnchor`.")]
		public static readonly Anchors UpperLeft = TopLeft;
		[Description("`MiddleTop`, name taken from `TextAnchor`.")]
		public static readonly Anchors UpperCenter = MiddleTop;
		[Description("`TopRight`, name taken from `TextAnchor`.")]
		public static readonly Anchors UpperRight = TopRight;
		[Description("`Middle`, name taken from `TextAnchor`.")]
		public static readonly Anchors MiddleCenter = Middle;
		[Description("`BottomLeft`, name taken from `TextAnchor`.")]
		public static readonly Anchors LowerLeft = BottomLeft;
		[Description("`MiddleLeft`, name taken from `TextAnchor`.")]
		public static readonly Anchors LowerCenter = MiddleLeft;
		[Description("`BottomRight`, name taken from `TextAnchor`.")]
		public static readonly Anchors LowerRight = BottomRight;

		[Description("Create anchors by specifying all four values.")]
		public Anchors(float left, float right, float top, float bottom)
		{
			this.left   = left;
			this.right  = right;
			this.top    = top;
			this.bottom = bottom;
		}
		public Anchors(RectTransform rt)
		{
			var min = rt.anchorMin;
			var max = rt.anchorMax;
			left    = min.x;
			right   = max.x;
			top     = 1f-max.y;
			bottom  = 1f-min.y;
		}

		private static readonly Anchors[] TextAnchorTable = new Anchors[]
		{
			TopLeft, MiddleTop, TopRight,
			MiddleLeft, Middle, MiddleRight,
			BottomLeft, MiddleBottom, BottomRight
		};

		[Description("Create anchors from `TextAnchor`.")]
		public Anchors(TextAnchor anchor)
		{
			this = TextAnchorTable[(int)anchor];
		}
		[Description("Convert to `TextAnchor`.")]
		public TextAnchor ToTextAnchor()
		{
			var x = float.IsNaN(left) ? right : float.IsNaN(right) ? left : (left + right) * 0.5f;
			var y = float.IsNaN(top) ? bottom : float.IsNaN(bottom) ? top : (top + bottom) * 0.5f;
			var min = 1f/3f;
			var max = 2f/3f;
			x = x <= min ? 0f : x >= max ? 1f : .5f;
			y = y <= min ? 0f : y >= max ? 1f : .5f;
			for (int i = 0; i < TextAnchorTable.Length; i++)
			{
				var test = TextAnchorTable[i];
				if (test.left == x && test.top == y)
					return (TextAnchor)i;
			}
			return TextAnchor.MiddleCenter;
		}

		public float GetMin(bool verticalAxis)
			=> verticalAxis ? top : left;
		public float GetMax(bool verticalAxis)
			=> verticalAxis ? bottom : right;

		public static float GetMin(RectTransform rt, bool verticalAxis)
			=> verticalAxis ? 1f-rt.anchorMax.y : rt.anchorMin.x;
		public static float GetMax(RectTransform rt, bool verticalAxis)
			=> verticalAxis ? 1f-rt.anchorMin.y : rt.anchorMax.x;
		public static void SetMin(RectTransform rt, float v, bool verticalAxis)
		{
			if (verticalAxis) rt.anchorMax = new Vector2(rt.anchorMax.x, 1f-v);
			else rt.anchorMin = new Vector2(v, rt.anchorMin.y);
		}
		public static void SetMax(RectTransform rt, float v, bool verticalAxis)
		{
			if (verticalAxis) rt.anchorMin = new Vector2(rt.anchorMin.x, 1f-v);
			else rt.anchorMax = new Vector2(v, rt.anchorMax.y);
		}

		public static float GetSize(RectTransform rt, bool verticalAxis)
		{
			var rect = rt.rect;
			return verticalAxis ? rect.height : rect.width;
		}
		public static void SetInsetAndSize(RectTransform rt,
			float inset, float size, float limit, float total,
			bool verticalAxis)
		{
			if (size > limit)
				size = limit;
			var flex = limit - size - inset;
			var apos = rt.anchoredPosition;
			var amin = rt.anchorMin;
			var amax = rt.anchorMax;
			var pivot = rt.pivot;
			var delta = rt.sizeDelta;
			if (verticalAxis)
			{
				// inset = 10, size = 40, limit = 90, total = 100
				// min = 0.5, max = 0.5, pivot = 0.5 => 30..70
				var ymin = inset + amin.y * flex;		// 10 + 40/2 = 30
				var ymax = limit - (1f-amax.y) * flex;	// 90 - 40/2 = 70
				apos.y = ymin + (ymax-ymin) * pivot.y;	// 30 + (70-30)/2 = 50
				delta.y = ymax-ymin - total*(amax.y-amin.y); // 40
			}
			else
			{
				// inset = 10, size = 40, limit = 90 (edge = 100)
				// min = 0, max = 1, pivot = 0.5 => 10..90
				var xmin = inset + amin.x * flex;		// 10 + 0*40 = 10
				var xmax = limit - (1f-amax.x) * flex;	// 90 - 0*40 = 90
				apos.x = xmin + (xmax-xmin) * pivot.x;	// 10 + (90-10)*0.5 = 50
				delta.x = xmax-xmin - total*(amax.x-amin.x); // 80 - 100 = -20
				// for min=max=1: xmin=50, xmax=90, pos=70, dsz=40
			}
			rt.anchoredPosition = apos;
			rt.sizeDelta = delta;
		}

		public override string ToString()
		{
			if (left == right)
			{
				if (top == bottom)
				{
					if (left == 0f)
					{
						if (top == 0f)
							return "TopLeft";
						if (top == .5f)
							return "MiddleLeft";
						if (top == 1f)
							return "BottomLeft";
					}
					else if (left == 1f)
					{
						if (top == 0f)
							return "TopRight";
						if (top == .5f)
							return "MiddleRight";
						if (top == 1f)
							return "BottomRight";
					}
					else if (left == .5f)
					{
						if (top == 0f)
							return "MiddleTop";
						if (top == .5f)
							return "Middle";
						if (top == 1f)
							return "MiddleBottom";
					}
				}
				else if (top == 0f && bottom == 1f)
				{
					if (left == 0f)
						return "FillLeft";
					if (left == .5f)
						return "Vertical";
					if (left == 1f)
						return "FillRight";
				}
			}
			else if (left == 0f && right == 1f)
			{
				if (top == bottom)
				{
					if (top == 0f)
						return "FillTop";
					if (top == 1f)
						return "FillBottom";
				}
				else if (top == 0f && bottom == 1f)
					return "Fill";
			}
			else if (Equals(Horizontal))
				return "Horizontal";
			return string.Format(CultureInfo.InvariantCulture,
				"({0}:{1}; {2}:{3})", left, right, top, bottom);
		}
	}
}

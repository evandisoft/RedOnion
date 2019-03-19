using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace RedOnion.UI
{
	[Serializable]
	public struct SizeConstraint
	{
		[SerializeField]
		public float minimal;
		[SerializeField]
		public float preferred;
		[SerializeField]
		public float flexible;

		public SizeConstraint(float all)
		{
			minimal		= all;
			preferred	= all;
			flexible    = all;
		}
		public SizeConstraint(float minimal, float preferred, float flexible)
		{
			this.minimal    = minimal;
			this.preferred  = preferred;
			this.flexible   = flexible;
		}

		public float All
		{
			get => minimal == preferred && preferred == flexible ? minimal : float.NaN;
			set
			{
				minimal     = value;
				preferred   = value;
				flexible    = value;
			}
		}

		public float Minimal
		{
			get => minimal;
			set => minimal = value;
		}
		public float Preferred
		{
			get => preferred;
			set => preferred = value;
		}
		public float Flexible
		{
			get => flexible;
			set => flexible = value;
		}

		public override string ToString()
			=> string.Format(CultureInfo.InvariantCulture,
				"({0}:{1}:{2})",
				minimal, preferred, flexible);

		public static float GetMinimal(RectTransform child, bool verticalAxis)
			=> verticalAxis ? LayoutUtility.GetMinHeight(child) : LayoutUtility.GetMinWidth(child);
		public static float GetPreferred(RectTransform child, bool verticalAxis)
			=> verticalAxis ? LayoutUtility.GetPreferredHeight(child) : LayoutUtility.GetPreferredWidth(child);
		public static float GetFlexible(RectTransform child, bool verticalAxis)
			=> verticalAxis ? LayoutUtility.GetFlexibleHeight(child) : LayoutUtility.GetFlexibleWidth(child);
		public SizeConstraint(RectTransform child, bool verticalAxis)
		{
			minimal		= GetMinimal(child, verticalAxis);
			preferred	= GetPreferred(child, verticalAxis);
			flexible	= GetFlexible(child, verticalAxis);
		}
	}

	[Serializable]
	public struct SizeConstraints
	{
		[SerializeField]
		public SizeConstraint x;
		[SerializeField]
		public SizeConstraint y;

		public SizeConstraints(float all)
		{
			x = new SizeConstraint(all);
			y = new SizeConstraint(all);
		}
		public SizeConstraints(SizeConstraint x, SizeConstraint y)
		{
			this.x = x;
			this.y = y;
		}

		public SizeConstraint X
		{
			get => x;
			set => x = value;
		}
		public SizeConstraint Y
		{
			get => y;
			set => y = value;
		}
		public SizeConstraint Horizontal
		{
			get => x;
			set => x = value;
		}
		public SizeConstraint Vertical
		{
			get => y;
			set => y = value;
		}

		public override string ToString()
			=> string.Format(CultureInfo.InvariantCulture,
				"({0}:{1}:{2}; {3}:{4}:{5})",
				x.minimal, x.preferred, x.flexible,
				y.minimal, y.preferred, y.flexible);

		public SizeConstraint this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return x;
				case 1:
					return y;
				default:
					throw new IndexOutOfRangeException("Invalid SizeConstraints index: " + index);
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid SizeConstraints index: " + index);
				}
			}
		}

		public static SizeConstraints Get(RectTransform child)
			=> new SizeConstraints(
				new SizeConstraint(child, false),
				new SizeConstraint(child, true));
	}
}

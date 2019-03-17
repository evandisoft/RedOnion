using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// EXPERIMENTAL!
// The idea is to reduce the number of components needed to use Unity UI
// and make the switching between layouts easier / smoother.
// Should integrate LayoutGroup (Horizontal/Vertical...) with LayoutElement (PreferHeight...)
// and also supplement ContentSizeFitter/AspectRatioFitter where appropriate
// (e.g. when containing Text - ContentSizeFitter - or Icon - AspectRatioFitter)

namespace RedOnion.UI.Components
{
	[AddComponentMenu("Layout/RedOnion"), RequireComponent(typeof(RectTransform))]
	public class LayoutComponent : UIBehaviour, ILayoutGroup, ILayoutElement
	{
		[NonSerialized]
		protected RectTransform rt;
		protected RectTransform RT
		{
			get
			{
				if (rt == null)
					rt = GetComponent<RectTransform>();
				return rt;
			}
		}

		[SerializeField]
		protected Layout layout;
		[SerializeField]
		protected Padding padding;
		[SerializeField]
		protected Anchors? overrideAnchors;
		[SerializeField]
		protected SizeConstraints size = new SizeConstraints(float.NaN);
		[NonSerialized]
		protected SizeConstraints calc = new SizeConstraints(float.NaN);

		int ILayoutElement.layoutPriority => 0;
		float ILayoutElement.minWidth => size.x.minimal >= 0f ? size.x.minimal : calc.x.minimal;
		float ILayoutElement.minHeight => size.y.minimal >= 0f ? size.y.minimal : calc.y.minimal;
		float ILayoutElement.preferredWidth => size.x.preferred >= 0f ? size.x.preferred : calc.x.preferred;
		float ILayoutElement.preferredHeight => size.y.preferred >= 0f ? size.y.preferred : calc.y.preferred;
		float ILayoutElement.flexibleWidth => size.x.flexible >= 0f ? size.x.flexible : calc.x.flexible;
		float ILayoutElement.flexibleHeight => size.y.flexible >= 0f ? size.y.flexible : calc.y.flexible;

		public Anchors Anchors
		{
			get => new Anchors(RT);
			set
			{
				var rt = RT;
				var oldMin = rt.anchorMin;
				var oldMax = rt.anchorMax;
				var newMin = new Vector2(value.left, 1f-value.bottom);
				var newMax = new Vector2(value.right, 1f-value.top);
				if (oldMin == newMin && oldMax == newMax)
					return;
				rt.anchorMin = newMin;
				rt.anchorMax = newMax;
				SetDirty();
			}
		}

		public Layout Layout
		{
			get => layout;
			set => Set(ref layout, value);
		}
		public Padding Padding
		{
			get => padding;
			set => Set(ref padding, value);
		}
		public Anchors? OverrideAnchors
		{
			get => overrideAnchors;
			set => Set(ref overrideAnchors, value);
		}

		public SizeConstraints Constraints
		{
			get => size;
			set => Set(ref size, value);
		}

		public float MinWidth
		{
			get => size.x.minimal;
			set => Set(ref size.x.minimal, value);
		}
		public float MinHeight
		{
			get => size.y.minimal;
			set => Set(ref size.y.minimal, value);
		}
		public float PreferWidth
		{
			get => size.x.preferred;
			set => Set(ref size.x.preferred, value);
		}
		public float PreferHeight
		{
			get => size.y.preferred;
			set => Set(ref size.y.preferred, value);
		}
		public float FlexWidth
		{
			get => size.x.flexible;
			set => Set(ref size.x.flexible, value);
		}
		public float FlexHeight
		{
			get => size.y.flexible;
			set => Set(ref size.y.flexible, value);
		}

		protected void Set<T>(ref T field, T value)
		{
			if ((field != null || value != null) && (field == null || !field.Equals(value)))
			{
				field = value;
				SetDirty();
			}
		}
		protected void SetDirty()
		{
			if (!IsActive())
				return;
			if (!CanvasUpdateRegistry.IsRebuildingLayout())
				LayoutRebuilder.MarkLayoutForRebuild(rt);
			else StartCoroutine(DelaySetDirty(rt));
		}
		private IEnumerator DelaySetDirty(RectTransform rectTransform)
		{
			yield return null;
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		void ILayoutElement.CalculateLayoutInputHorizontal()
		{
			switch (layout)
			{
			default:
				PepareAuto(RT, false);
				return;
			case Layout.Horizontal:
				PrepareLine(RT, false, false);
				return;
			case Layout.Vertical:
				PrepareLine(RT, false, true);
				return;
			case Layout.FlowHorizontal:
				PrepareFlow(RT, false, false);
				return;
			case Layout.FlowVertical:
				PrepareFlow(RT, false, true);
				return;
			}
		}
		void ILayoutElement.CalculateLayoutInputVertical()
		{
			switch (layout)
			{
			default:
				PepareAuto(RT, true);
				return;
			case Layout.Horizontal:
				PrepareLine(RT, true, false);
				return;
			case Layout.Vertical:
				PrepareLine(RT, true, true);
				return;
			case Layout.FlowHorizontal:
				PrepareFlow(RT, true, false);
				return;
			case Layout.FlowVertical:
				PrepareFlow(RT, true, true);
				return;
			}
		}
		void ILayoutController.SetLayoutHorizontal()
		{
			switch (layout)
			{
			default:
				SetAuto(RT, false);
				return;
			case Layout.Horizontal:
				SetLine(RT, false, false);
				return;
			case Layout.Vertical:
				SetLine(RT, false, true);
				return;
			}
		}
		void ILayoutController.SetLayoutVertical()
		{
			switch (layout)
			{
			default:
				SetAuto(RT, true);
				return;
			case Layout.Horizontal:
				SetLine(RT, true, false);
				return;
			case Layout.Vertical:
				SetLine(RT, true, true);
				return;
			}
		}

		protected void PepareAuto(RectTransform rt, bool verticalAxis)
		{
		}
		protected void SetAuto(RectTransform rt, bool verticalAxis)
		{
			int n = rt.childCount;
			if (n == 0) return;
			var total = Anchors.GetSize(rt, verticalAxis);
			var limit = total-padding.GetLast(verticalAxis);
			var first = padding.GetFirst(verticalAxis);
			for (int i = 0; i < n; i++)
			{
				var child = rt.GetChild(i) as RectTransform;
				if (child == null) continue;
				var prefer = SizeConstraint.GetPreferred(child, verticalAxis);
				Anchors.SetInsetAndSize(child, first, prefer, limit, total, verticalAxis);
			}
		}

		protected void PrepareLine(RectTransform rt, bool verticalAxis, bool vertical)
		{
		}
		protected void SetLine(RectTransform rt, bool verticalAxis, bool vertical)
		{
		}

		protected void PrepareFlow(RectTransform rt, bool verticalAxis, bool vertical)
		{
		}
		protected void SetFlow(RectTransform rt, bool verticalAxis, bool vertical)
		{
		}
	}
}

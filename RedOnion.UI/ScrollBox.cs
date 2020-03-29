using RedOnion.Attributes;
using System;
using System.ComponentModel;
using UnityEngine;
using UUI = UnityEngine.UI;

// TODO: custom layout for view-port margin

namespace RedOnion.UI
{
	[WorkInProgress, Description("Scrollable panel")]
	public class ScrollBox : Panel
	{
		[Unsafe, Description("Game object of the view port.")]
		public GameObject ViewPort { get; }

		protected UUI.Image ViewImage { get; }
		protected UUI.Mask ViewMask { get; }
		protected UUI.ScrollRect ScrollRect { get; }
		protected UUI.ContentSizeFitter SizeFitter { get; }
		protected UUI.Image BoxImage { get; }

		protected Scrollbar hscroll, vscroll;

		public ScrollBox(Layout layout = Layout.Vertical)
		{
			var content = RectTransform;
			content.pivot = new Vector2(0f, 1f);
			content.anchorMin = new Vector2(0f, 1f);
			content.anchorMax = new Vector2(1f, 1f);
			SizeFitter = GameObject.AddComponent<UUI.ContentSizeFitter>();
			SizeFitter.horizontalFit = UUI.ContentSizeFitter.FitMode.PreferredSize;
			SizeFitter.verticalFit = UUI.ContentSizeFitter.FitMode.PreferredSize;

			ViewPort = new GameObject() { layer = UILayer };
			var viewRect = ViewPort.AddComponent<RectTransform>();
			viewRect.pivot = new Vector2(0f, 1f);
			viewRect.anchorMin = new Vector2(0f, 0f);
			viewRect.anchorMax = new Vector2(1f, 1f);
			viewRect.sizeDelta = new Vector2(0f, 0f);
			ViewImage = ViewPort.AddComponent<UUI.Image>();
			ViewMask = ViewPort.AddComponent<UUI.Mask>();
			ViewMask.showMaskGraphic = false;

			RootObject = new GameObject() { layer = UILayer };
			RectTransform = RootObject.AddComponent<RectTransform>();
			RectTransform.pivot = new Vector2(.5f, .5f);
			RectTransform.anchorMin = new Vector2(.5f, .5f);
			RectTransform.anchorMax = new Vector2(.5f, .5f);

			GameObject.transform.SetParent(ViewPort.transform, false);
			ViewPort.transform.SetParent(RootObject.transform, false);

			ScrollRect = RootObject.AddComponent<Components.ScrollView>();
			ScrollRect.content = content;
			ScrollRect.viewport = viewRect;

			BoxImage = RootObject.AddComponent<Components.BackgroundImage>();
			BoxImage.sprite = Skin.scrollView.normal.background;
			BoxImage.type = UUI.Image.Type.Sliced;

			Layout = layout;
			ScrollRect.horizontal = layout != Layout.Vertical;
			ScrollRect.vertical = layout != Layout.Horizontal;
		}

		[Description("Scrolling and bar visibility.")]
		public enum Scroll
		{
			[Description("No scrolling in that direction.")]
			No = -2,
			[Description("Scrolling without scrollbar.")]
			Hide = -1,
			[Description("Scrolling with permanently visible scrollbar.")]
			Show = UUI.ScrollRect.ScrollbarVisibility.Permanent,
			[Description("Scrolling with auto-hiding scrollbar.")]
			Auto = UUI.ScrollRect.ScrollbarVisibility.AutoHide,
			[Description("Scrolling with auto-hiding scrollbar and view-port expand/shrink.")]
			Size = UUI.ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport,
		}

		[Description("Horizontally scrollable.")]
		public Scroll Horizontal
		{
			get => !ScrollRect.horizontal ? Scroll.No : hscroll == null ? Scroll.Hide : (Scroll)ScrollRect.horizontalScrollbarVisibility;
			set
			{
				if (value == Scroll.No)
				{
					if (!ScrollRect.horizontal)
						return;
					ScrollRect.horizontal = false;
					if (hscroll == null)
						return;
					ScrollRect.horizontalScrollbar = null;
					hscroll.Dispose();
					hscroll = null;
					return;
				}
				ScrollRect.horizontal = true;
				if (value == Scroll.Hide)
				{
					if (hscroll == null)
						return;
					ScrollRect.horizontalScrollbar = null;
					hscroll.Dispose();
					hscroll = null;
					return;
				}
				if (hscroll == null)
				{
					hscroll = new Scrollbar(vertical: false);
					hscroll.RootObject.transform.SetParent(RootObject.transform, false);
					hscroll.Parent = this;
					ScrollRect.horizontalScrollbar = hscroll.Component;
				}
				ScrollRect.horizontalScrollbarVisibility = (UUI.ScrollRect.ScrollbarVisibility)value;
			}
		}
		[Description("Vertically scrollable.")]
		public Scroll Vertical
		{
			get => !ScrollRect.horizontal ? Scroll.No : vscroll == null ? Scroll.Hide : (Scroll)ScrollRect.verticalScrollbarVisibility;
			set
			{
				if (value == Scroll.No)
				{
					if (!ScrollRect.vertical)
						return;
					ScrollRect.vertical = false;
					if (vscroll == null)
						return;
					ScrollRect.verticalScrollbar = null;
					vscroll.Dispose();
					vscroll = null;
					return;
				}
				ScrollRect.vertical = true;
				if (value == Scroll.Hide)
				{
					if (vscroll == null)
						return;
					ScrollRect.verticalScrollbar = null;
					vscroll.Dispose();
					vscroll = null;
					return;
				}
				if (vscroll == null)
				{
					vscroll = new Scrollbar(vertical: true);
					vscroll.RootObject.transform.SetParent(RootObject.transform, false);
					vscroll.Parent = this;
					ScrollRect.verticalScrollbar = vscroll.Component;
				}
				ScrollRect.horizontalScrollbarVisibility = (UUI.ScrollRect.ScrollbarVisibility)value;
			}
		}
		[Description("Elastic drag (can be dragged outside of normal bounds). Setting this to `false` switches to clamped mode (if previously `true`).")]
		public bool Elastic
		{
			get => ScrollRect.movementType == UUI.ScrollRect.MovementType.Elastic;
			set
			{
				if (value) ScrollRect.movementType = UUI.ScrollRect.MovementType.Elastic;
				else if (Elastic) ScrollRect.movementType = UUI.ScrollRect.MovementType.Clamped;
			}
		}
		[Description("Clamped mode (cannot be dragged outside of bounds). Setting this to `false` switches to unrestricted mode (if previously `true`).")]
		public bool Clamped
		{
			get => ScrollRect.movementType == UUI.ScrollRect.MovementType.Clamped;
			set
			{
				if (value) ScrollRect.movementType = UUI.ScrollRect.MovementType.Clamped;
				else if (Clamped) ScrollRect.movementType = UUI.ScrollRect.MovementType.Unrestricted;
			}
		}

		public Vector2 BarSpacing
		{
			get => new Vector2(-ScrollRect.verticalScrollbarSpacing, -ScrollRect.horizontalScrollbarSpacing);
			set
			{
				ScrollRect.verticalScrollbarSpacing = -value.x;
				ScrollRect.horizontalScrollbarSpacing = -value.y;
			}
		}
		public Vector2 BarWidth
		{
			get => new Vector2(vscroll?.Width ?? 0f, hscroll?.Height ?? 0f);
			set
			{
				if (vscroll != null)
					vscroll.Width = value.x;
				if (hscroll != null)
					hscroll.Height = value.y;
			}
		}
	}
}

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

		[Description("Horizontally scrollable.")]
		public bool horizontal
		{
			get => ScrollRect.horizontal;
			set => ScrollRect.horizontal = value;
		}
		[Description("Vertocally scrollable.")]
		public bool vertical
		{
			get => ScrollRect.vertical;
			set => ScrollRect.vertical = value;
		}
		[Description("Elastic drag (can be dragged outside of normal bounds). Setting this to `false` switches to clamped mode (if previously `true`).")]
		public bool elastic
		{
			get => ScrollRect.movementType == UUI.ScrollRect.MovementType.Elastic;
			set
			{
				if (value) ScrollRect.movementType = UUI.ScrollRect.MovementType.Elastic;
				else if (elastic) ScrollRect.movementType = UUI.ScrollRect.MovementType.Clamped;
			}
		}
		[Description("Clamped mode (cannot be dragged outside of bounds). Setting this to `false` switches to unrestricted mode (if previously `true`).")]
		public bool clamped
		{
			get => ScrollRect.movementType == UUI.ScrollRect.MovementType.Clamped;
			set
			{
				if (value) ScrollRect.movementType = UUI.ScrollRect.MovementType.Clamped;
				else if (clamped) ScrollRect.movementType = UUI.ScrollRect.MovementType.Unrestricted;
			}
		}
	}
}

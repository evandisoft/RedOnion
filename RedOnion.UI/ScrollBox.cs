using RedOnion.Attributes;
using System;
using System.ComponentModel;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[WorkInProgress]
	public class ScrollBox : Panel
	{
		public GameObject ViewPort { get; }
		public UUI.Image ViewImage { get; }
		public UUI.Mask ViewMask { get; }
		public UUI.ScrollRect ScrollRect { get; }
		public UUI.ContentSizeFitter SizeFitter { get; }
		public UUI.Image BoxImage { get; }

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
		}
	}
}

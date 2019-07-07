using System;
using UE = UnityEngine;
using UUI = UnityEngine.UI;
using ROC = RedOnion.UI.Components;
using KSP.UI;

namespace RedOnion.KSP.API.Namespaces
{
	public static class Unity_Namespace
	{
		public static Type Object = typeof(UE.Object);
		public static Type GameObject = typeof(UE.GameObject);
		public static Type Canvas = typeof(UE.Canvas);
		public static Type CanvasGroup = typeof(UE.CanvasGroup);
		public static Type RectTransform = typeof(UE.RectTransform);
		public static Type LayerMask = typeof(UE.LayerMask);

		public static Type DefaultControls = typeof(UUI.DefaultControls);
		public static Type GridLayout = typeof(UUI.GridLayoutGroup);
		public static Type HorizontalLayout = typeof(UUI.HorizontalLayoutGroup);
		public static Type VerticalLayout = typeof(UUI.VerticalLayoutGroup);
		public static Type LayoutRebuilder = typeof(UUI.LayoutRebuilder);
		public static Type LayoutUtility = typeof(UUI.LayoutUtility);
		public static Type ContentSizeFitter = typeof(UUI.ContentSizeFitter);
		public static Type AspectRatioFitter = typeof(UUI.AspectRatioFitter);
		public static Type SizeFitter = typeof(UUI.ContentSizeFitter);
		public static Type RatioFitter = typeof(UUI.AspectRatioFitter);

		public static Type Text = typeof(UUI.Text);
		public static Type Button = typeof(UUI.Button);
		public static Type Image = typeof(UUI.Image);
		public static Type RawImage = typeof(UUI.RawImage);
		public static Type BackgroundImage = typeof(ROC.BackgroundImage);
		public static Type DragHandler = typeof(ROC.DragHandler);
		public static Type LayoutComponent = typeof(ROC.LayoutComponent);

		public static Type Screen = typeof(UE.Screen);
		public static Type Sprite = typeof(UE.Sprite);
		public static Type Texture = typeof(UE.Texture);
		public static Type Texture2D = typeof(UE.Texture2D);

		public static Type UIMaster = typeof(UIMasterController);
		public static Type UISkinDef = typeof(UISkinDef);
		public static Type UISkinManager = typeof(UISkinManager);
		public static Type UIStyle = typeof(UIStyle);
		public static Type UIStyleState = typeof(UIStyleState);
	}
}

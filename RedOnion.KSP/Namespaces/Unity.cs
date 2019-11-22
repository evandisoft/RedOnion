using System;
using UE = UnityEngine;
using UUI = UnityEngine.UI;
using UIC = RedOnion.UI.Components;
using KSP.UI;
using System.ComponentModel;
using RedOnion.KSP.Utilities;
using RedOnion.KSP.Attributes;

namespace RedOnion.KSP.Namespaces
{
	[SafeProps, DisplayName("Unity"), DocBuild("RedOnion.KSP/Namespaces/Unity")]
	[Description("Shortcuts to (unsafe) Unity API.")]
	public static class Unity_Namespace
	{
		[Description("UnityEngine.Debug - debug tools, logging.")]
		public static Type Debug = typeof(UE.Debug);
		[Description("UnityEngine.Color - color (also in `ui.color`).")]
		public static Type Color = typeof(UE.Color);
		[Description("UnityEngine.Rect - bounds of a rectangle (int).")]
		public static Type Rect = typeof(UE.Rect);

		[Description("UnityEngine.Screen - screen bounds eetc.")]
		public static Type Screen = typeof(UE.Screen);
		[Description("UnityEngine.Sprite - sprite/image.")]
		public static Type Sprite = typeof(UE.Sprite);
		[Description("UnityEngine.Texture - texture base class.")]
		public static Type Texture = typeof(UE.Texture);
		[Description("UnityEngine.Texture2D - 2D texture (e.g. for icons).")]
		public static Type Texture2D = typeof(UE.Texture2D);

		[Description("KSP UIMasterController.")]
		public static Type UIMaster = typeof(UIMasterController);
		[Description("KSP UISkinDef.")]
		public static Type UISkinDef = typeof(UISkinDef);
		[Description("KSP UISkinManager.")]
		public static Type UISkinManager = typeof(UISkinManager);
		[Description("KSP UIStyle.")]
		public static Type UIStyle = typeof(UIStyle);
		[Description("KSP UIStyleState.")]
		public static Type UIStyleState = typeof(UIStyleState);

		[Description("UnityEngine.Vector2")]
		public static Type Vector2 = typeof(UE.Vector2);
		[Description("UnityEngine.Vector3")]
		public static Type Vector3 = typeof(UE.Vector3);
		[Description("UnityEngine.Vector4")]
		public static Type Vector4 = typeof(UE.Vector4);
		[Description("UnityEngine.Quaternion")]
		public static Type Quaternion = typeof(UE.Quaternion);

		[Description("UnityEngine.Vector2d")]
		public static Type Vector2d = typeof(UE.Vector2d);
		[Description("Vector3d")]
		public static Type Vector3d = typeof(Vector3d);
		[Description("UnityEngine.Vector4d")]
		public static Type Vector4d = typeof(UE.Vector4d);
		[Description("UnityEngine.QuaternionD")]
		public static Type QuaternionD = typeof(UE.QuaternionD);

		[Description("UnityEngine.Object")]
		public static Type Object = typeof(UE.Object);
		[Description("UnityEngine.GameObject")]
		public static Type GameObject = typeof(UE.GameObject);
		[Description("UnityEngine.Canvas")]
		public static Type Canvas = typeof(UE.Canvas);
		[Description("UnityEngine.CanvasGroup")]
		public static Type CanvasGroup = typeof(UE.CanvasGroup);
		[Description("UnityEngine.RectTransform")]
		public static Type RectTransform = typeof(UE.RectTransform);
		[Description("UnityEngine.LayerMask")]
		public static Type LayerMask = typeof(UE.LayerMask);

		[Description("UnityEngine.UI.DefaultControls")]
		public static Type DefaultControls = typeof(UUI.DefaultControls);
		[Description("UnityEngine.UI.GridLayoutGroup")]
		public static Type GridLayout = typeof(UUI.GridLayoutGroup);
		[Description("UnityEngine.UI.HorizontalLayoutGroup")]
		public static Type HorizontalLayout = typeof(UUI.HorizontalLayoutGroup);
		[Description("UnityEngine.UI.VerticalLayoutGroup")]
		public static Type VerticalLayout = typeof(UUI.VerticalLayoutGroup);
		[Description("UnityEngine.UI.LayoutRebuilder")]
		public static Type LayoutRebuilder = typeof(UUI.LayoutRebuilder);
		[Description("UnityEngine.UI.LayoutUtility")]
		public static Type LayoutUtility = typeof(UUI.LayoutUtility);
		[Description("UnityEngine.UI.ContentSizeFitter")]
		public static Type ContentSizeFitter = typeof(UUI.ContentSizeFitter);
		[Description("UnityEngine.UI.AspectRatioFitter")]
		public static Type AspectRatioFitter = typeof(UUI.AspectRatioFitter);
		[Description("UnityEngine.UI.ContentSizeFitter")]
		public static Type SizeFitter = typeof(UUI.ContentSizeFitter);
		[Description("UnityEngine.UI.AspectRatioFitter")]
		public static Type RatioFitter = typeof(UUI.AspectRatioFitter);

		[Description("UnityEngine.UI.Text")]
		public static Type Text = typeof(UUI.Text);
		[Description("UnityEngine.UI.Button")]
		public static Type Button = typeof(UUI.Button);
		[Description("UnityEngine.UI.Image")]
		public static Type Image = typeof(UUI.Image);
		[Description("UnityEngine.UI.RawImage")]
		public static Type RawImage = typeof(UUI.RawImage);

		[Description("RedOnion.UI.Components.BackgroundImage")]
		public static Type BackgroundImage = typeof(UIC.BackgroundImage);
		[Description("RedOnion.UI.Components.DragHandler")]
		public static Type DragHandler = typeof(UIC.DragHandler);
		[Description("RedOnion.UI.Components.LayoutComponent")]
		public static Type LayoutComponent = typeof(UIC.LayoutComponent);
	}
}

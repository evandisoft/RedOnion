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
		public static readonly Type Debug = typeof(UE.Debug);
		[Description("UnityEngine.Color - color (also in `ui.color`).")]
		public static readonly Type Color = typeof(UE.Color);
		[Description("UnityEngine.Rect - bounds of a rectangle (int).")]
		public static readonly Type Rect = typeof(UE.Rect);

		[Description("UnityEngine.Screen - screen bounds eetc.")]
		public static readonly Type Screen = typeof(UE.Screen);
		[Description("UnityEngine.Sprite - sprite/image.")]
		public static readonly Type Sprite = typeof(UE.Sprite);
		[Description("UnityEngine.Texture - texture base class.")]
		public static readonly Type Texture = typeof(UE.Texture);
		[Description("UnityEngine.Texture2D - 2D texture (e.g. for icons).")]
		public static readonly Type Texture2D = typeof(UE.Texture2D);

		[Description("KSP UIMasterController.")]
		public static readonly Type UIMaster = typeof(UIMasterController);
		[Description("KSP UISkinDef.")]
		public static readonly Type UISkinDef = typeof(UISkinDef);
		[Description("KSP UISkinManager.")]
		public static readonly Type UISkinManager = typeof(UISkinManager);
		[Description("KSP UIStyle.")]
		public static readonly Type UIStyle = typeof(UIStyle);
		[Description("KSP UIStyleState.")]
		public static readonly Type UIStyleState = typeof(UIStyleState);

		[Description("UnityEngine.Vector2")]
		public static readonly Type Vector2 = typeof(UE.Vector2);
		[Description("UnityEngine.Vector3")]
		public static readonly Type Vector3 = typeof(UE.Vector3);
		[Description("UnityEngine.Vector4")]
		public static readonly Type Vector4 = typeof(UE.Vector4);
		[Description("UnityEngine.Quaternion")]
		public static readonly Type Quaternion = typeof(UE.Quaternion);

		[Description("UnityEngine.Vector2d")]
		public static readonly Type Vector2d = typeof(UE.Vector2d);
		[Description("Vector3d")]
		public static readonly Type Vector3d = typeof(Vector3d);
		[Description("UnityEngine.Vector4d")]
		public static readonly Type Vector4d = typeof(UE.Vector4d);
		[Description("UnityEngine.QuaternionD")]
		public static readonly Type QuaternionD = typeof(UE.QuaternionD);

		[Description("UnityEngine.Object")]
		public static readonly Type Object = typeof(UE.Object);
		[Description("UnityEngine.GameObject")]
		public static readonly Type GameObject = typeof(UE.GameObject);
		[Description("UnityEngine.Canvas")]
		public static readonly Type Canvas = typeof(UE.Canvas);
		[Description("UnityEngine.CanvasGroup")]
		public static readonly Type CanvasGroup = typeof(UE.CanvasGroup);
		[Description("UnityEngine.RectTransform")]
		public static readonly Type RectTransform = typeof(UE.RectTransform);
		[Description("UnityEngine.LayerMask")]
		public static readonly Type LayerMask = typeof(UE.LayerMask);

		[Description("UnityEngine.UI.DefaultControls")]
		public static readonly Type DefaultControls = typeof(UUI.DefaultControls);
		[Description("UnityEngine.UI.GridLayoutGroup")]
		public static readonly Type GridLayout = typeof(UUI.GridLayoutGroup);
		[Description("UnityEngine.UI.HorizontalLayoutGroup")]
		public static readonly Type HorizontalLayout = typeof(UUI.HorizontalLayoutGroup);
		[Description("UnityEngine.UI.VerticalLayoutGroup")]
		public static readonly Type VerticalLayout = typeof(UUI.VerticalLayoutGroup);
		[Description("UnityEngine.UI.LayoutRebuilder")]
		public static readonly Type LayoutRebuilder = typeof(UUI.LayoutRebuilder);
		[Description("UnityEngine.UI.LayoutUtility")]
		public static readonly Type LayoutUtility = typeof(UUI.LayoutUtility);
		[Description("UnityEngine.UI.ContentSizeFitter")]
		public static readonly Type ContentSizeFitter = typeof(UUI.ContentSizeFitter);
		[Description("UnityEngine.UI.AspectRatioFitter")]
		public static readonly Type AspectRatioFitter = typeof(UUI.AspectRatioFitter);
		[Description("UnityEngine.UI.ContentSizeFitter")]
		public static readonly Type SizeFitter = typeof(UUI.ContentSizeFitter);
		[Description("UnityEngine.UI.AspectRatioFitter")]
		public static readonly Type RatioFitter = typeof(UUI.AspectRatioFitter);

		[Description("UnityEngine.UI.Text")]
		public static readonly Type Text = typeof(UUI.Text);
		[Description("UnityEngine.UI.Button")]
		public static readonly Type Button = typeof(UUI.Button);
		[Description("UnityEngine.UI.Image")]
		public static readonly Type Image = typeof(UUI.Image);
		[Description("UnityEngine.UI.RawImage")]
		public static readonly Type RawImage = typeof(UUI.RawImage);

		[Description("RedOnion.UI.Components.BackgroundImage")]
		public static readonly Type BackgroundImage = typeof(UIC.BackgroundImage);
		[Description("RedOnion.UI.Components.DragHandler")]
		public static readonly Type DragHandler = typeof(UIC.DragHandler);
		[Description("RedOnion.UI.Components.LayoutComponent")]
		public static readonly Type LayoutComponent = typeof(UIC.LayoutComponent);
	}
}

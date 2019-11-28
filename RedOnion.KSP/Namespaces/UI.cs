using RedOnion.KSP.Attributes;
using RedOnion.KSP.Utilities;
using System;
using System.ComponentModel;

namespace RedOnion.KSP.Namespaces
{
	[SafeProps, DisplayName("UI"), DocBuild("RedOnion.KSP/Namespaces/UI")]
	[Description("User interface, see [RedOnion.UI](../../RedOnion.UI/README.md)")]
	public static class UI_Namespace
	{
		[Description("UnityEngine.Color")]
		public static readonly Type Color = typeof(UnityEngine.Color);

		[Description("Window - all elements must belong to some window.")]
		public static readonly Type Window = typeof(UI_Window);
		[Description("Scene flags for limiting life-span of windows when switching scenes.")]
		public static readonly Type SceneFlags = typeof(UI.SceneFlags);

		[Description("Anchors for specifying where to place elements (fill, center, ...)")]
		public static readonly Type Anchors = typeof(UI.Anchors);
		[Description("Padding - empty space inside an element / around contained elements.")]
		public static readonly Type Padding = typeof(UI.Padding);
		[Description("Layout - horizontal/vertical")]
		public static readonly Type Layout = typeof(UI.Layout);
		[Description("LayoutPadding - Padding + Spacing")]
		public static readonly Type LayoutPadding = typeof(UI.LayoutPadding);

		// unused for now
		public static readonly Type SizeConstraint = typeof(UI.SizeConstraint);
		public static readonly Type SizeConstraints = typeof(UI.SizeConstraints);
		public static readonly Type Constraint = typeof(UI.SizeConstraint);
		public static readonly Type Constraints = typeof(UI.SizeConstraints);

		[Description("Element - base class for all elements/controls.")]
		public static readonly Type Element = typeof(UI.Element);
		[Description("Content panel (a box to hold other elements, manages layout).")]
		public static readonly Type Panel = typeof(UI.Panel);
		[Description("Line of text")]
		public static readonly Type Label = typeof(UI.Label);
		[Description("Clickable button (or toggle-button).")]
		public static readonly Type Button = typeof(UI.Button);
		[Description("Toggle - experimental. WIP")]
		public static readonly Type Toggle = typeof(UI.Toggle);
		[Description("Line of editable text (or multi-line text editor).")]
		public static readonly Type TextBox = typeof(UI.TextBox);
		[Description("Image / Icon.")]
		public static readonly Type Image = typeof(UI.Image);
		[Description("UnityEngine.UI.Image.Type")]
		public static readonly Type ImageType = typeof(UnityEngine.UI.Image.Type);
	}
}

using RedOnion.KSP.Attributes;
using RedOnion.KSP.Utilities;
using System;
using System.ComponentModel;

namespace RedOnion.KSP.Namespaces
{
	[SafeProps, DisplayName("UI"), DocBuild("RedOnion.KSP/Namespaces/UI")]
	public static class UI_Namespace
	{
		public static readonly Type Color = typeof(UnityEngine.Color);
		public static readonly Type Window = typeof(Window);
		public static readonly Type SceneFlags = typeof(UI.SceneFlags);
		public static readonly Type Anchors = typeof(UI.Anchors);
		public static readonly Type Padding = typeof(UI.Padding);
		public static readonly Type Layout = typeof(UI.Layout);
		public static readonly Type LayoutPadding = typeof(UI.LayoutPadding);
		public static readonly Type SizeConstraint = typeof(UI.SizeConstraint);
		public static readonly Type SizeConstraints = typeof(UI.SizeConstraints);
		public static readonly Type Constraint = typeof(UI.SizeConstraint);
		public static readonly Type Constraints = typeof(UI.SizeConstraints);
		public static readonly Type Element = typeof(UI.Element);
		public static readonly Type Panel = typeof(UI.Panel);
		public static readonly Type Label = typeof(UI.Label);
		public static readonly Type Button = typeof(UI.Button);
		public static readonly Type Toggle = typeof(UI.Toggle);
		public static readonly Type TextBox = typeof(UI.TextBox);
		public static readonly Type Image = typeof(UI.Image);
		public static readonly Type ImageType = typeof(UnityEngine.UI.Image.Type);
	}
}

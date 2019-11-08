using System;
using System.ComponentModel;

namespace RedOnion.KSP.Namespaces
{
	[DisplayName("UI")]
	public static class UI_Namespace
	{
		public static Type Window = typeof(Window);
		public static Type SceneFlags = typeof(UI.SceneFlags);
		public static Type Anchors = typeof(UI.Anchors);
		public static Type Padding = typeof(UI.Padding);
		public static Type Layout = typeof(UI.Layout);
		public static Type LayoutPadding = typeof(UI.LayoutPadding);
		public static Type SizeConstraint = typeof(UI.SizeConstraint);
		public static Type SizeConstraints = typeof(UI.SizeConstraints);
		public static Type Constraint = typeof(UI.SizeConstraint);
		public static Type Constraints = typeof(UI.SizeConstraints);
		public static Type Element = typeof(UI.Element);
		public static Type Panel = typeof(UI.Panel);
		public static Type Label = typeof(UI.Label);
		public static Type Button = typeof(UI.Button);
		public static Type Toggle = typeof(UI.Toggle);
		public static Type TextBox = typeof(UI.TextBox);
		public static Type Image = typeof(UI.Image);
		public static Type ImageType = typeof(UnityEngine.UI.Image.Type);
	}
}

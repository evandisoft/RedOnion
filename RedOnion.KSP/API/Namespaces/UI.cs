using System;
using RedOnion.UI;

namespace RedOnion.KSP.API.Namespaces
{
	public class UI_Namespace
	{
		public static UI_Namespace Instance { get; } = new UI_Namespace();
		private UI_Namespace() { }

		public Type Window = typeof(API_UI.Window);
		public Type Anchors = typeof(UI.Anchors);
		public Type Padding = typeof(UI.Padding);
		public Type Layout = typeof(UI.Layout);
		public Type LayoutPadding = typeof(UI.LayoutPadding);
		public Type SizeConstraint = typeof(UI.SizeConstraint);
		public Type SizeConstraints = typeof(UI.SizeConstraints);
		public Type Constraint = typeof(UI.SizeConstraint);
		public Type Constraints = typeof(UI.SizeConstraints);
		public Type Element = typeof(UI.Element);
		public Type Panel = typeof(UI.Panel);
		public Type Label = typeof(UI.Label);
		public Type Button = typeof(UI.Button);
		public Type TextBox = typeof(UI.TextBox);
	}
}

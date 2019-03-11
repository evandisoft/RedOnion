using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class LeftAlignedButton : Button {
		public LeftAlignedButton(string text, Action action) : base(text, action)
		{
		}

		protected override void ProtectedUpdate(Rect rect)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.button);
			}
			style.alignment = TextAnchor.LowerCenter;
			base.ProtectedUpdate(rect);
		}

		protected override void ProtectedUpdate()
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.button);
			}
			style.alignment = TextAnchor.LowerCenter;
			base.ProtectedUpdate();
		}
	}
}

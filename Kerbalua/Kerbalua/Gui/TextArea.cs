using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class TextArea:UIElement {
		public GUIContent content = new GUIContent("");
		// 
		public int cursorIndex = 0;
		public int selectIndex = 0;

		protected override void ProtectedUpdate(Rect rect)
		{
			if (Visible) {
				if (style != null) {
					content.text = GUI.TextArea(rect, content.text, style);
				} else {
					content.text = GUI.TextArea(rect, content.text);
				}
			}
		}

		protected override void ProtectedUpdate()
		{
			if (Visible) {
				if (style != null) {
					content.text = GUILayout.TextArea(content.text, style);
				} else {
					content.text = GUILayout.TextArea(content.text);
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace Kerbalua.Gui {
    public class TextArea {
		public GUIContent content = new GUIContent("");

		public void Render(Rect rect, GUIStyle style = null)
		{
			if (style == null) {
				content.text = GUI.TextArea(rect, content.text);
			} else {
				content.text = GUI.TextArea(rect, content.text, style);
			}
		}
	}
}

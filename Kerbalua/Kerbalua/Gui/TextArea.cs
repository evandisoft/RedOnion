using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class TextArea:UIElement,ILayoutRenderer {
		public GUIContent content = new GUIContent("");

		public virtual void Render(Rect rect,GUIStyle style=null)
		{
			ClearCharEvent();
			SetNextControlAsMainControl();
			if (style != null) {
				content.text = GUI.TextArea(rect, content.text,style);
			} else {
				content.text = GUI.TextArea(rect, content.text);
			}
		}

		public void Render()
		{
			ClearCharEvent();
			SetNextControlAsMainControl();
			content.text = GUILayout.TextArea(content.text);
		}
	}
}

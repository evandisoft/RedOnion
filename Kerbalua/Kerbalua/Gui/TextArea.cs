using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class TextArea {
		public GUIContent content = new GUIContent("");

		public void Render(Rect rect)
		{
			content.text = GUI.TextArea(rect, content.text);
		}
	}
}

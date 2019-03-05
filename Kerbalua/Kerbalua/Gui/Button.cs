using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class Button:ILayoutRenderer {
		public GUIContent content = new GUIContent("");
		public Action action;

		public Button(string text, Action action)
		{
			content.text = text;
			this.action = action;
		}

		public void Render(Rect rect)
		{
			if (GUI.Button(rect, content)) {
				action.Invoke();
			}
		}

		public void Render()
		{
			if (GUILayout.Button(content)) {
				action.Invoke();
			}
		}
	}
}

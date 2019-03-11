using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class Button:UIElement {
		public GUIContent content = new GUIContent("");
		public Action action;

		public Button(string text, Action action)
		{
			content.text = text;
			this.action = action;
		}

		protected override void ProtectedUpdate(Rect rect)
		{
			if (GUI.Button(rect, content)) {
				action.Invoke();
			}
		}

		protected override void ProtectedUpdate()
		{
			if (GUILayout.Button(content)) {
				action.Invoke();
			}
		}
	}
}

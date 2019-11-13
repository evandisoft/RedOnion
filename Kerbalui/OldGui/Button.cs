using System;
using UnityEngine;

namespace Kerbalui.Obsolete {
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
			RunBaseControl(() =>
			{
				if (style == null)
				{
					if (GUI.Button(rect, content))
					{
						action.Invoke();
					}
				}
				else
				{
					if (GUI.Button(rect, content, style))
					{
						action.Invoke();
					}
				}
			});
		}

		protected override void ProtectedUpdate()
		{
			RunBaseControl(() =>
			{
				if (style == null) {
					if (GUILayout.Button(content)) {
						action.Invoke();
					}
				} else {
					if (GUILayout.Button(content, style)) {
						action.Invoke();
					}
				}
			});
		}
	}
}

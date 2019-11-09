using System;
using UnityEngine;

namespace LiveRepl.UI.Elements {
	public class Button:Element {
		public GUIContent content = new GUIContent("");
		public Action action;
		public GUIStyle style;

		public Button(string text, Action action)
		{
			content.text = text;
			this.action = action;
		}

		public override void Update()
		{
			if (style==null)
			{
				style=GUI.skin.button;
			}

			LabelNextControl();
			if (GUI.Button(rect, content, style))
			{
				action.Invoke();
			}
		}
	}
}

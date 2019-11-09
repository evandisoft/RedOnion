using System;
using UnityEngine;

namespace LiveRepl.UI.Controls {
	public class Button:TextElement {
		//public GUIContent content = new GUIContent("");
		public Action action;
		//public GUIStyle style;

		public Button(string text, Action action)
		{
			content.text = text;
			this.action = action;
		}

		public override void SetDefaultStyle()
		{
			style=GUI.skin.button;
		}

		public override void Update()
		{
			InitStyle();

			LabelNextControl();
			if (GUI.Button(rect, content, style))
			{
				action.Invoke();
			}
		}
	}
}

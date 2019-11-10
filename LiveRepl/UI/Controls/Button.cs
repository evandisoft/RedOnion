using System;
using LiveRepl.UI.Controls.Abstract;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Controls {
	public class Button:ContentControl {
		public Action action;

		public Button(string text, Action action)
		{
			content.text = text;
			this.action = action;
		}

		public override GUIStyle DefaultStyle()
		{
			return Window.defaultSkin.button;
		}

		protected override void ControlUpdate()
		{
			if (GUI.Button(rect, content, StyleOrDefault))
			{
				action.Invoke();
			}
		}
	}
}

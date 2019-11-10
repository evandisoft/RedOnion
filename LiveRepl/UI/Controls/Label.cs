using System;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class Label : ContentControl
	{
		public Label(string text)
		{
			content.text=text;
		}

		public override GUIStyle DefaultStyle()
		{
			return Window.DefaultSkin.label;
		}

		protected override void ControlUpdate()
		{
			GUI.Label(rect, content, StyleOrDefault);
		}
	}
}

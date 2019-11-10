using System;
using LiveRepl.UI.Controls.Abstract;
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
			return Window.defaultSkin.label;
		}

		protected override void ControlUpdate()
		{
			GUI.Label(rect, content, StyleOrDefault);
		}
	}
}

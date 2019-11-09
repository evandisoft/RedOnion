using System;
using LiveRepl.UI.Base;
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
			return ScriptWindow.defaultSkin.label;
		}

		protected override void ControlUpdate()
		{
			GUI.Label(rect, content, StyleOrDefault);
		}
	}
}

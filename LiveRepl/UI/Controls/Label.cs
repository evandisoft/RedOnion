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

		public override void Update()
		{
			LabelNextControl();
			GUI.Label(rect, content, StyleOrDefault);
		}
	}
}

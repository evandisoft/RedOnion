using System;
using LiveRepl.UI.Controls.Abstract;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class TextField : EditableText
	{
		public override GUIStyle DefaultStyle()
		{
			return Window.defaultSkin.textField;
		}

		protected override void ControlUpdate()
		{
			content.text=GUI.TextField(rect, content.text, StyleOrDefault);
		}
	}
}

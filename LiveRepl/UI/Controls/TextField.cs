using System;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class TextField : EditableTextControl
	{
		public override GUIStyle DefaultStyle()
		{
			return ScriptWindow.defaultSkin.textField;
		}

		protected override void ControlUpdate()
		{
			content.text=GUI.TextField(rect, content.text, StyleOrDefault);
		}
	}
}

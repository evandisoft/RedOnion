using System;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class TextArea:EditableTextControl
	{
		public override GUIStyle DefaultStyle()
		{
			return ScriptWindow.defaultSkin.textArea;
		}

		public override void Update()
		{
			LabelNextControl();
			content.text=GUI.TextArea(rect, content.text, StyleOrDefault);
		}
	}
}

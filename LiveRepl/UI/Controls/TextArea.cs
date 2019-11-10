using System;
using LiveRepl.UI.Controls.Abstract;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class TextArea:EditableText
	{
		public override GUIStyle DefaultStyle()
		{
			return Window.defaultSkin.textArea;
		}

		protected override void ControlUpdate()
		{
			content.text=GUI.TextArea(rect, content.text, StyleOrDefault);
		}
	}
}

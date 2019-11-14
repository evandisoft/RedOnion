using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls
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

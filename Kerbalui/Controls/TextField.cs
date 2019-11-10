using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls
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

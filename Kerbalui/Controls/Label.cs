using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls
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

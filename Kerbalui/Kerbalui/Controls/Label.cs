using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls
{
	public class Label : ContentControl
	{
		public Label(string text):base(GUI.skin.label)
		{
			Content.text=text;
		}

		public Label() : base(GUI.skin.label)
		{
			Content.text="";
		}

		protected override void ControlUpdate()
		{
			GUI.Label(rect, Content, Style);
		}
	}
}

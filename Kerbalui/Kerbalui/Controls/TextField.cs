using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls
{
	public class TextField : EditableText
	{
		public TextField():base(GUI.skin.textField)
		{
		}

		protected override void ControlUpdate()
		{
			Content.text=GUI.TextField(rect, Content.text, Style);
		}
	}
}

using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls
{
	public class TextArea:EditableText
	{
		public TextArea() : base(GUI.skin.textArea)
		{
		}

		protected override void ControlUpdate()
		{
			Content.text=GUI.TextArea(rect, Content.text, Style);
		}
	}
}

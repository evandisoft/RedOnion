using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public class TextArea:TextElement
	{
		public override void SetDefaultStyle()
		{
			style=GUI.skin.textArea;
		}

		public override void Update()
		{
			InitStyle();

			LabelNextControl();
			content.text=GUI.TextArea(rect, content.text,style);
		}
	}
}

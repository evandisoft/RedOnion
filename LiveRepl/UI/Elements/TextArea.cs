using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public class TextArea:TextElement
	{
		public override void Update()
		{
			if (style == null)
			{
				style=GUI.skin.textArea;
			}

			LabelNextControl();
			content.text=GUI.TextArea(rect, content.text,style);
		}
	}
}

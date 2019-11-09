using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public class TextField : TextElement
	{
		public override void Update()
		{
			if (style == null)
			{
				style=GUI.skin.textField;
			}

			LabelNextControl();
			content.text=GUI.TextField(rect, content.text, style);
		}
	}
}

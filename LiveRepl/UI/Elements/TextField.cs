using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public class TextField : TextElement
	{
		public override void SetDefaultStyle()
		{
			style=GUI.skin.textField;
		}

		public override void Update()
		{
			InitStyle();

			LabelNextControl();
			content.text=GUI.TextField(rect, content.text, style);
		}

		
	}
}

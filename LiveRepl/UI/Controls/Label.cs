using System;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class Label : TextElement
	{
		public Label(string text)
		{
			content.text=text;
		}

		public override void SetDefaultStyle()
		{
			style=GUI.skin.label;
		}

		public override void Update()
		{
			InitStyle();

			LabelNextControl();
			GUI.Label(rect, content, style);
		}
	}
}

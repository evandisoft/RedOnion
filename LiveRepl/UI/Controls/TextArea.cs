using System;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class TextArea:EditableTextControl
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

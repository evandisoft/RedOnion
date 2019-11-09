using System;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public class TextField : EditableTextControl
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

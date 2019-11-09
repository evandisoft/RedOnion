using System;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Controls
{
	public abstract class TextElement:Control
	{
		public GUIContent content=new GUIContent("");
		public GUIStyle style;

		public abstract void SetDefaultStyle();
		public void InitStyle()
		{
			if (style==null)
			{
				SetDefaultStyle();
			}
		}
	}
}

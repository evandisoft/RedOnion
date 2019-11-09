using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public abstract class TextElement:Element
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

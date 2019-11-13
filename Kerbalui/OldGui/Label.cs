using System;
using UnityEngine;

namespace Kerbalui.Obsolete
{
	public class Label : UIElement
	{
		public GUIContent content = new GUIContent("");
		public Label(string text)
		{
			content.text = text;
		}

		protected override void ProtectedUpdate(Rect rect)
		{
			RunBaseControl(() =>
			{
				if (Visible)
				{
					if (style != null)
					{
						GUI.Label(rect, content.text, style);
					}
					else
					{
						GUI.Label(rect, content.text);
					}
				}
			});
		}

		protected override void ProtectedUpdate()
		{
			RunBaseControl(() => 
			{
				if (Visible)
				{
					if (style != null)
					{
						GUILayout.Label(content.text, style);
					}
					else
					{
						GUILayout.Label(content.text);
					}
				}
			});
		}
	}
}

using System;
using UnityEngine;

namespace Kerbalui.Gui
{
	public class TextArea : UIElement
	{
		public GUIContent content = new GUIContent("");
		// 
		public int cursorIndex = 0;
		public int selectIndex = 0;

		protected override void ProtectedUpdate(Rect rect)
		{
			if (Visible)
			{
				if (Event.current.type==EventType.KeyDown && HasFocus())
				{
					Debug.Log("Key is before "+Event.current);
					//GUI.SetNextControlName(ControlName);
				}

				RunBaseControl(() =>
				{
					if (style != null)
					{
						content.text = GUI.TextArea(rect, content.text, style);
					}
					else
					{
						content.text = GUI.TextArea(rect, content.text);
					}
				});

				if (Event.current.type==EventType.KeyDown && HasFocus())
				{
					Debug.Log("Key is after "+Event.current);
				}
			}
		}

		protected override void ProtectedUpdate()
		{
			if (Visible)
			{
				if (style != null)
				{
					content.text = GUILayout.TextArea(content.text, style);
				}
				else
				{
					content.text = GUILayout.TextArea(content.text);
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class ScrollableTextArea : TextArea {
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;

		public override void Render(Rect rect,GUIStyle style=null)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
			}

			Vector2 contentSize=style.CalcSize(content);
			Rect outputContentRect = new Rect(0, 0, rect.width, rect.height);

			scrollPos = GUI.BeginScrollView(rect, scrollPos, outputContentRect);
			{
				base.Render(outputContentRect, style);

				if (resetScroll) {
					scrollPos.y = rect.height;
					resetScroll = false;
				}
			}
			GUI.EndScrollView();
		}

		public void ResetScroll()
		{
			resetScroll = true;
		}
	}
}

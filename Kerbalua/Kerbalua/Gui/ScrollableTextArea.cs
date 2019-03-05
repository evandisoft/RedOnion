using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class ScrollableTextArea : TextArea {
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;

		public override void Render(Rect rect,GUIStyle style=null)
		{
			// Manually handling scrolling because there is a coordinate issue
			// with the default method. The system measures the mouse vertical
			// position from the bottom instead of the top of the screen for scrolling
			// purposes for whatever bizarre reason.
			if (Event.current.isScrollWheel) {
				float delta = 0;
				if (Input.GetAxis("Mouse ScrollWheel") > 0) {
					delta = -1;
				} else {
					delta = 1;
				}

				Debug.Log($"{Event.current.type},{Event.current.mousePosition},{rect}");

				Event.current.Use();
			}

			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
			}

			Vector2 contentSize=style.CalcSize(content);
			Rect outputContentRect = new Rect(0, 0, contentSize.x, contentSize.y);

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

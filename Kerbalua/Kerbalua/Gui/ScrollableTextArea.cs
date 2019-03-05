using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class ScrollableTextArea : TextArea {
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;

		public override void Render(Rect rect,GUIStyle style=null)
		{
			GUI.BeginGroup(rect);
			{
				rect.x = 0;
				rect.y = 0;

				HandleScrolling(rect);

				if (style == null) {
					style = new GUIStyle(GUI.skin.textArea);
				}

				Vector2 contentSize = style.CalcSize(content);
				Rect contentRect = new Rect(0, 0,
					Math.Max(contentSize.x, rect.width),
					Math.Max(contentSize.y, rect.height)
					);

				scrollPos = GUI.BeginScrollView(rect, scrollPos, contentRect);
				{
					base.Render(contentRect, style);

					if (resetScroll) {
						scrollPos.y = rect.height;
						resetScroll = false;
					}
				}
				GUI.EndScrollView();
			}
			GUI.EndGroup();
		}

		// Manually handling scrolling because there is a coordinate issue
		// with the default method. The system measures the mouse vertical
		// position from the bottom instead of the top of the screen for scrolling
		// purposes for whatever bizarre reason.
		void HandleScrolling(Rect rect)
		{
			if (Event.current.isScrollWheel && GUIUtil.MouseInRect(rect)) {
				float delta = 0;
				if (Input.GetAxis("Mouse ScrollWheel") > 0) {
					delta = -1;
				} else {
					delta = 1;
				}

				scrollPos.y += delta * 20;
				Event.current.Use();
			}
		}

		public void ResetScroll()
		{
			resetScroll = true;
		}
	}
}

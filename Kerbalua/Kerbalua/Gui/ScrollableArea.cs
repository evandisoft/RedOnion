using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class ScrollableArea {
		public IRectRender renderable;
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;

		public ScrollableArea(IRectRender renderable)
		{
			this.renderable = renderable;
		}

		public void Render(Rect rect)
		{
			Rect contentRect = renderable.GetContentRect();

			scrollPos = GUI.BeginScrollView(rect, scrollPos, contentRect);
			{
				renderable.Render(contentRect);

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

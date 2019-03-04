using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class OutputBox {
		public GUIContent content = new GUIContent("");
		public Vector2 scrollPos = new Vector2();
		bool resetScroll = false;

		public void Render(Rect rect)
		{
			GUIStyle outputStyle = new GUIStyle(GUI.skin.textArea) {
				alignment = TextAnchor.LowerLeft
			};

			float outputHeight = outputStyle.CalcHeight(content, rect.width);
			Rect outputContentRect = new Rect(rect);
			outputContentRect.height = Math.Max(outputHeight, rect.height);
			scrollPos = GUI.BeginScrollView(rect, scrollPos, outputContentRect);
			content.text = GUI.TextArea(outputContentRect, content.text, outputStyle);

			if (resetScroll) {
				scrollPos.y = rect.height;
				resetScroll = false;
			}

			GUI.EndScrollView();
		}

		public void ResetScroll()
		{
			resetScroll = true;
		}
	}
}

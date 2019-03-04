using UnityEngine;

namespace Kerbalua.Gui {
	public class CompletionBox {
		public GUIContent content = new GUIContent("");
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;

		public void Render(Rect rect)
		{
			GUIStyle style = new GUIStyle(GUI.skin.textArea);
			Rect completionContentRect = rect;
			completionContentRect.height = Mathf.Max(rect.height, style.CalcHeight(content, rect.width));

			scrollPos = GUI.BeginScrollView(rect, scrollPos, completionContentRect);
			{
				content.text = GUI.TextArea(completionContentRect, content.text);

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

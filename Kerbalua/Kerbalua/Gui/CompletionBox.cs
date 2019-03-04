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
			Vector2 contentSize = style.CalcSize(content);
			completionContentRect.height = Mathf.Max(rect.height, contentSize.y);
			completionContentRect.width = Mathf.Max(rect.width, contentSize.x);

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

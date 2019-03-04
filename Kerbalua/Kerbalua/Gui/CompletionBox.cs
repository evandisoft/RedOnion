using UnityEngine;

namespace Kerbalua.Gui {
	public class CompletionBox {
		public GUIContent content = new GUIContent("");

		public void Render(Rect rect)
		{
			GUIStyle style = new GUIStyle(GUI.skin.textArea);
			content.text = GUI.TextArea(rect, content.text);
		}
	}
}

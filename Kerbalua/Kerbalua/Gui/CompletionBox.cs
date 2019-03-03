using System;
using UnityEngine;
using Kerbalua.Completion;
using Kerbalua.Other;

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

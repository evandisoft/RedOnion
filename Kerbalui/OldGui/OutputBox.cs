using System;
using UnityEngine;

namespace Kerbalui.Obsolete {
	public class OutputBox:ScrollableTextArea {
		public const int OUTPUT_LENGTH_LIMIT = 10000;

		void CommonOutputProcessing()
		{
			ResetScroll();

		}

		public void AddReturnValue(string str)
		{
			content.text += "\nr> " + str;
			CommonOutputProcessing();
		}

		public void AddOutput(string str)
		{
			content.text += "\no> " + str;
			CommonOutputProcessing();
		}

		public void AddError(string str)
		{
			content.text += "\ne> " + str;
			CommonOutputProcessing();
		}

		public void AddSourceString(string str)
		{
			content.text += "\ni> " + str;
			CommonOutputProcessing();
		}

		public void AddFileContent(string str)
		{
			content.text += "\nf> " + str;
			CommonOutputProcessing();
		}

		protected override void ProtectedUpdate(Rect rect)
        {
			style = new GUIStyle(GUI.skin.textArea) {
				alignment = TextAnchor.LowerLeft,
				font = GUILibUtil.GetMonoSpaceFont()
			};
			if (content.text.Length > OUTPUT_LENGTH_LIMIT) {
				content.text = content.text.Substring(content.text.Length - OUTPUT_LENGTH_LIMIT, OUTPUT_LENGTH_LIMIT);
			}
			///Debug.Log("cursor index is " + cursorIndex);

			base.ProtectedUpdate(rect);
		}
	}
}

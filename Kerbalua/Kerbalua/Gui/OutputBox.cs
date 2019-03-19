using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class OutputBox:ScrollableTextArea {
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
				font = GUIUtil.GetMonoSpaceFont()
			};
			///Debug.Log("cursor index is " + cursorIndex);

			base.ProtectedUpdate(rect);
		}
	}
}

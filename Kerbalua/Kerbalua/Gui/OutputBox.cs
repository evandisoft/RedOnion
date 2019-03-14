using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class OutputBox:ScrollableTextArea {
		public void AddReturnValue(string str)
		{
			content.text += "\nr> " + str;
		}

		public void AddIO(string str)
		{
			content.text += "\no> " + str;
		}

		public void AddError(string str)
		{
			content.text += "\ne> " + str;
		}

		public void AddSourceString(string str)
		{
			content.text += "\ni> " + str;
		}

		public void AddFileContent(string str)
		{
			content.text += "\nf> " + str;
		}

		protected override void ProtectedUpdate(Rect rect)
        {
			style = new GUIStyle(GUI.skin.textArea) {
				alignment = TextAnchor.LowerLeft,
				font = GUIUtil.GetMonoSpaceFont()
			};

			base.ProtectedUpdate(rect);
		}
	}
}

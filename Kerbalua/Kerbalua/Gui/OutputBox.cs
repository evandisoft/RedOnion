using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class OutputBox:ScrollableTextArea {


		public override void Render(Rect rect, GUIStyle style = null)
		{
			GUIStyle outputStyle = new GUIStyle(GUI.skin.textArea) {
				alignment = TextAnchor.LowerLeft,
				//font = GUIUtil.GetMonoSpaceFont()
			};

			base.Render(rect, outputStyle);
		}
	}
}

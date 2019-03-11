using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class OutputBox:ScrollableTextArea {
		protected override void ProtectedUpdate(Rect rect)
        {
			style = new GUIStyle(GUI.skin.textArea) {
				alignment = TextAnchor.LowerLeft,
				//font = GUIUtil.GetMonoSpaceFont()
			};

			base.ProtectedUpdate(rect);
		}
	}
}

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class ButtonBar {
		public List<Button> buttons = new List<Button>();

		public void Render(Rect rect,bool vertical=true)
		{
			GUILayout.BeginArea(rect);

			if (vertical) {
				GUILayout.BeginVertical();
			} else {
				GUILayout.BeginHorizontal();
			}


			foreach (var button in buttons) {
				button.Render();
			}

			if (vertical) {
				GUILayout.EndVertical();
			} else {
				GUILayout.EndHorizontal();
			}

			GUILayout.EndArea();
		}
	}
}

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class ButtonBar {
		public List<Button> buttons = new List<Button>();

		public void Render(Rect rect)
		{
			GUILayout.BeginArea(rect);

			GUILayout.BeginVertical();

			foreach(var button in buttons) {
				button.Render();
			}

			GUILayout.EndVertical();

			GUILayout.EndArea();
		}
	}
}

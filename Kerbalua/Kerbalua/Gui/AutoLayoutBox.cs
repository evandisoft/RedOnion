using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class AutoLayoutBox {
		public List<ILayoutRenderer> renderables = new List<ILayoutRenderer>();

		public void Update(Rect rect,bool vertical=true)
		{
			GUILayout.BeginArea(rect);
			{
				if (vertical) {
					GUILayout.BeginVertical();
				} else {
					GUILayout.BeginHorizontal();
				}

				foreach (var button in renderables) {
					button.Update();
				}

				if (vertical) {
					GUILayout.EndVertical();
				} else {
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndArea();
		}
	}
}

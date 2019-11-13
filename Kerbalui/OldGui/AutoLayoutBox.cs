using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalui.Obsolete {
	public class AutoLayoutBox {
		public List<ILayoutRenderer> renderables = new List<ILayoutRenderer>();

		public virtual void Update(Rect rect,bool vertical=true,GUIStyle style=null)
		{
			GUILayout.BeginArea(rect);
			{
				if (vertical) {
					GUILayout.BeginVertical();
				} else {
					GUILayout.BeginHorizontal();
				}

				foreach (var renderable in renderables) {
					renderable.Update();
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

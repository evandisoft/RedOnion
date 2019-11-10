using System;
using UnityEngine;

namespace Kerbalui.Gui {
	public interface IRectRenderer {
		void Update(Rect rect, bool visible = true, GUIStyle style = null);
	}
}

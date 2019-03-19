using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public interface IRectRenderer {
		void Update(Rect rect, bool visible = true, GUIStyle style = null);
	}
}

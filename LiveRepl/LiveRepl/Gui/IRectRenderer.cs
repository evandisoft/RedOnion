using System;
using UnityEngine;

namespace LiveRepl.Gui {
	public interface IRectRenderer {
		void Update(Rect rect, bool visible = true, GUIStyle style = null);
	}
}

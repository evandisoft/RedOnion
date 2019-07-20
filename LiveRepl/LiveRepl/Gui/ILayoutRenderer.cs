using System;
using UnityEngine;

namespace LiveRepl.Gui {
	/// <summary>
	/// Can render itself with GUILayout.
	/// </summary>
	public interface ILayoutRenderer {
		void Update(bool visible = true, GUIStyle style = null);
	}
}

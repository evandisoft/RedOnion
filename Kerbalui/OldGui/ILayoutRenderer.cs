using System;
using UnityEngine;

namespace Kerbalui.Obsolete {
	/// <summary>
	/// Can render itself with GUILayout.
	/// </summary>
	public interface ILayoutRenderer {
		void Update(bool visible = true, GUIStyle style = null);
	}
}

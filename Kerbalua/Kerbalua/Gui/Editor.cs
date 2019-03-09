using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kerbalua.Gui {
    public class Editor:EditingArea{
		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>
		public new KeyBindings KeyBindings = new KeyBindings();

		public override void Render(Rect rect,GUIStyle style=null)
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.Render(rect, style);
		}
	}
}

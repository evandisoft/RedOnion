using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kerbalui.Obsolete {
    public class Editor:EditingArea{
		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>
		public new KeyBindings KeyBindings = new KeyBindings();

		protected override void ProtectedUpdate(Rect rect)
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			//if (HasFocus()) {
			//	int id = GUIUtility.keyboardControl;
			//	Debug.Log("Id for editor is " + id);
			//}
			base.ProtectedUpdate(rect);
		}
	}
}

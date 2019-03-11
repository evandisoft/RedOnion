using System;
using UnityEngine;
using Kerbalua.Completion;

namespace Kerbalua.Gui {
	public class InputBox:EditingArea {
		public new KeyBindings KeyBindings = new KeyBindings();

		protected override void ProtectedUpdate(Rect rect)
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			//if (HasFocus()) {
			//	int id = GUIUtility.keyboardControl;
			//	Debug.Log("Id for inputBox is " + id);
			//}
			base.ProtectedUpdate(rect);
		}
	}
}

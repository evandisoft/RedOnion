using System;
using UnityEngine;
using Kerbalua.Completion;

namespace Kerbalua.Gui {
	public class InputBox:EditingArea {
		public KeyBindings KeyBindings = new KeyBindings();

		public override void Render(Rect rect, GUIStyle style = null)
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.Render(rect, style);
		}
	}
}

﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kerbalua.Gui {
    public class Editor:EditingArea{
		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>
		public new KeyBindings KeyBindings = new KeyBindings();

		protected override void ProtectedUpdate(Rect rect)
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);

			base.ProtectedUpdate(rect);
		}
	}
}
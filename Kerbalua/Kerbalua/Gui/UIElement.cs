using System;
using UnityEngine;

namespace Kerbalua.Gui {
	/// <summary>
	/// Subclasses of this class must use SetNextControlAsMainControl() in
	/// their Render code to set which control will respond to HasFocus() and 
	/// GrabFocus().
	/// </summary>
	public class UIElement {
		static long NextID = 0;
		/// <summary>
		/// Auto-generated unique name for this control, to be used as the name
		/// in GUI.SetNextControlName(name), so that GUI.GetNameOfFocusedControl 
		/// and GUI.FocusControl can be used for managing focus of that control.
		/// </summary>
		public readonly string ControlName = "Control-"+NextID++;

		/// <summary>
		/// Assigns the next control the unique name generated in ControlName.
		/// This is used as the relevant control for HasFocus() and TakeFocus()
		/// </summary>
		protected void SetNextControlAsMainControl()
		{
			GUI.SetNextControlName(ControlName);
		}

		/// <summary>
		/// Returns true if this control has focus.
		/// </summary>
		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl() == ControlName;
		}

		/// <summary>
		/// Make the main control for this element grab the focus.
		/// </summary>
		public void GrabFocus()
		{
			GUI.FocusControl(ControlName);
		}
	}
}

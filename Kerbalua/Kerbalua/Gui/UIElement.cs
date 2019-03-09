using System;
using UnityEngine;

namespace Kerbalua.Gui {
	/// <summary>
	/// Subclasses of this class must use SetNextControlAsMainControl() in
	/// their Render code to set which control will respond to HasFocus() and 
	/// GrabFocus().
	/// </summary>
	public class UIElement:IFocusable {
		static long NextID = 0;
		/// <summary>
		/// Auto-generated unique name for this control, to be used as the name
		/// in GUI.SetNextControlName(name), so that GUI.GetNameOfFocusedControl 
		/// and GUI.FocusControl can be used for managing focus of that control.
		/// </summary>
		public readonly string ControlName = "Control-"+NextID++;

		string IFocusable.ControlName => ControlName;

		/// <summary>
		/// Assigns the next control the unique name generated in ControlName.
		/// This is used as the relevant control for HasFocus() and TakeFocus()
		/// Must be called within a Render.
		/// </summary>
		protected void SetNextControlAsMainControl()
		{
			GUI.SetNextControlName(ControlName);
		}

		/// <summary>
		/// Returns true if this control has focus.
		/// Must be called within a Render.
		/// </summary>
		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl() == ControlName;
		}

		/// <summary>
		/// Make the main control for this element grab the focus.
		/// Must be called within a Render.
		/// </summary>
		public void GrabFocus()
		{
			GUI.FocusControl(ControlName);
		}

		protected void ClearCharEvent()
		{
			if (Event.current.type == EventType.KeyDown) {
				GUIUtil.ConsumeMarkedCharEvent(Event.current);
			}
		}
	}
}

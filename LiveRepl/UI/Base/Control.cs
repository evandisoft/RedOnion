using System;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	/// <summary>
	/// An Control wraps the functionality of a specific IMGUI control
	/// </summary>
	public abstract class Control:UIElement
	{
		static long NextID = 0;
		public readonly string ControlName = "Control-"+NextID++;

		/// <summary>
		/// </summary>
		/// <returns><c>true</c>, if this control has focus, <c>false</c> otherwise.</returns>
		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl()==ControlName;
		}

		/// <summary>
		/// Sets this control to be focused.
		/// </summary>
		public void GrabFocus()
		{
			GUI.FocusControl(ControlName);
		}

		/// <summary>
		/// Calls the base update, then sets the next control, then calls the Control's update.
		/// </summary>
		public override void Update()
		{
			base.Update();
			GUI.SetNextControlName(ControlName);
			ControlUpdate();
		}

		/// <summary>
		/// The Control's defined update function containing the call to a GUI control function.
		/// </summary>
		protected abstract void ControlUpdate();
	}
}

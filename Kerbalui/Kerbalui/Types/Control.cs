using System;
using UnityEngine;

namespace Kerbalui.Types
{
	/// <summary>
	/// An Control wraps the functionality of a specific IMGUI control
	/// </summary>
	public abstract class Control:Element
	{
		static long nextID = 0;
		public readonly string controlName = "Control-"+nextID++;

		/// <summary>
		/// </summary>
		/// <returns><c>true</c>, if this control has focus, <c>false</c> otherwise.</returns>
		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl()==controlName;
		}

		/// <summary>
		/// Sets this control to be focused.
		/// </summary>
		public void GrabFocus()
		{
			GUI.FocusControl(controlName);
		}

		protected override void TypeSpecificUpdate()
		{
			GUI.SetNextControlName(controlName);
			ControlUpdate();
		}

		/// <summary>
		/// The Control's defined update function containing the call to a GUI control function.
		/// </summary>
		protected abstract void ControlUpdate();
	}
}
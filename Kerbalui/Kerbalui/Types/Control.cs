using System;
using Kerbalui.Util;
using UnityEngine;
using static MunOS.Debugging.QueueLogger;

namespace Kerbalui.Types
{
	/// <summary>
	/// An Control wraps the functionality of a specific IMGUI control
	/// </summary>
	public abstract class Control:Element
	{
		static long nextID = 0;

		public string ControlName { get; } = "Control-" + nextID++;

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
			UILogger.Log("Control ", ControlName, "is grabbing focus");
			GUI.FocusControl(ControlName);
		}

		protected override void TypeSpecificUpdate()
		{
			GUI.SetNextControlName(ControlName);
			ControlUpdate();
		}

		/// <summary>
		/// The Control's defined update function containing the call to a GUI control function.
		/// </summary>
		protected abstract void ControlUpdate();
	}
}

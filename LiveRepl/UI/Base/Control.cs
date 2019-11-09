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
		/// This associates the ControlName of this element with its control
		/// for the IMGUI system, allowing it to be focused with GUI.FocusControl(ControlName),
		/// and allowing its focus to be checked with GUI.GetNameOfFocusedControl()==ControlName.
		/// </summary>
		protected void LabelNextControl()
		{
			GUI.SetNextControlName(ControlName);
		}

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

		public override void SetRect(Rect rect)
		{
			this.rect=rect;
		}
	}
}

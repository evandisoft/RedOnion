using System;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	/// <summary>
	/// An "Control" wraps the functionality of an IMGUI control
	/// </summary>
	public abstract class Control:UIElement
	{
		static long NextID = 0;
		public readonly string ControlName = "Control-"+NextID++;

		protected void LabelNextControl()
		{
			GUI.SetNextControlName(ControlName);
		}

		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl()==ControlName;
		}

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

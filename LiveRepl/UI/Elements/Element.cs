using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	/// <summary>
	/// An Element represents at general type of object as opposed to specific custom parts.
	/// A Pane or Widget represents a specific part of a UI, while An Element would be something
	/// like a textarea which will be used in multiple places doesn't have hardwired references
	/// to parts of the UI structure, and can be reused in other UIs.
	/// </summary>
	public abstract class Element:IRectRenderable
	{
		static long NextID = 0;
		public readonly string ControlName = "Control-"+NextID++;
		public bool RectRender=true;
		public Rect rect;

		protected void LabelNextControl()
		{
			GUI.SetNextControlName(ControlName);
		}

		public abstract void Update();

		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl()==ControlName;
		}

		public void GrabFocus()
		{
			GUI.FocusControl(ControlName);
		}

		public void SetRect(Rect rect)
		{
			this.rect=rect;
		}
	}
}

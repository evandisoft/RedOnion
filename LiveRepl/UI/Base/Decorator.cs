using System;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	/// <summary>
	/// A Decorator contains, rather than extends, a Control, and adds extra functionality.
	/// </summary>
	public abstract class Decorator:UIElement
	{
		public override void SetRect(Rect rect)
		{
			base.SetRect(rect);
			SetChildRect();
		}

		/// <summary>
		/// Implemented by the subclass. Sets the rect of the child after its own rect was set by SetRect
		/// </summary>
		protected abstract void SetChildRect();
	}
}

using System;
using UnityEngine;

namespace Kerbalui.Types
{
	/// <summary>
	/// A Decorator contains, rather than extends, a Control, and adds extra functionality.
	/// </summary>
	public abstract class Decorator:Element
	{
		public override void SetRect(Rect newRect)
		{
			base.SetRect(newRect);
			SetChildRect();
		}

		protected override void TypeSpecificUpdate()
		{
			DecoratorUpdate();
		}

		protected abstract void DecoratorUpdate();

		/// <summary>
		/// Implemented by the subclass. Sets the rect of the child after its own rect was set by SetRect
		/// </summary>
		protected abstract void SetChildRect();
	}
}

using System;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	// Base class of all UI parts
	public abstract class UIElement
	{
		public Rect rect;
		/// <summary>
		/// needsRecalculation is not yet used. Not sure what I want to do with it yet.
		/// One use for this is that until the first update, some controls don't have their real style set,
		/// and as such don't have their real content size properly accounted for in rect calculation
		/// </summary>
		public bool needsRecalculation;
		public virtual void SetRect(Rect rect)
		{
			this.rect=rect;
		}

		/// <summary>
		/// The main update function. Calls TypeSpecificUpdate which will do different things depending on the
		/// type of subclass that is implementing it.
		/// 
		/// Trigers a Rect recalculation if "needsRecalculation" was true
		/// </summary>
		public void Update()
		{
			TypeSpecificUpdate();
			if (needsRecalculation)
			{
				SetRect(rect);
				needsRecalculation=false;
			}
		}

		protected abstract void TypeSpecificUpdate();
	}
}

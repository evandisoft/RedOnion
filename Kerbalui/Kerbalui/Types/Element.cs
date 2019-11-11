using System;
using UnityEngine;

namespace Kerbalui.Types
{
	// Base class of all UI parts
	public abstract class Element
	{
		public Rect rect;

		/// <summary>
		/// If the update function for this element should be called. Setting this to false will disable update
		/// and also disable taking up space in a spacer.
		/// </summary>
		public bool active=true;

		/// <summary>
		/// needsRecalculation triggers an recalculation of rects after the first update.
		/// I'm setting it to true by default because before the first update the defaultSkin is not set.
		/// And it needs to be set for some things to run properly.
		/// </summary>
		public bool needsResize=true;
		public virtual void SetRect(Rect rect)
		{
			this.rect=rect;
			needsResize=false;
		}

		/// <summary>
		/// The main update function. Calls TypeSpecificUpdate which will do different things depending on the
		/// type of subclass that is implementing it.
		/// 
		/// Trigers a Rect recalculation if "needsRecalculation" was true
		/// </summary>
		public void Update()
		{
			if (!active) return;

			TypeSpecificUpdate();
			if (needsResize)
			{
				SetRect(rect);
			}
		}

		protected abstract void TypeSpecificUpdate();

		public virtual Vector2 MinSize => new Vector2();
	}
}

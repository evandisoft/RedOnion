using System;
using UnityEngine;

namespace Kerbalui.Types
{
	// Base class of all UI parts
	public abstract class Element
	{
		public Rect rect;

		/// <summary>
		/// Active determines whether this element's update will be ran and whether it takes up
		/// space in a Spacer.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public bool Active { get; set; } = true;

		/// <summary>
		/// needsRecalculation triggers an recalculation of rects after the first update.
		/// I'm setting it to true by default because before the first update the defaultSkin is not set.
		/// And it needs to be set for some things to run properly.
		/// </summary>
		public bool needsResize=true;
		public virtual void SetRect(Rect newRect)
		{
			rect=newRect;
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
			if (!Active) return;

			//bool prevEnabled=GUI.enabled;
			//GUI.enabled=Enabled;
			TypeSpecificUpdate();
			if (needsResize)
			{
				SetRect(rect);
			}
			//GUI.enabled=prevEnabled;
		}

		protected abstract void TypeSpecificUpdate();

		public virtual Vector2 MinSize => new Vector2();
	}
}

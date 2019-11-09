using System;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	// Base class of all UI parts
	public abstract class UIElement
	{
		public Rect rect;
		public bool needsRecalculation;
		public virtual void SetRect(Rect rect)
		{
			this.rect=rect;
		}

		public virtual void Update()
		{

		}
	}
}

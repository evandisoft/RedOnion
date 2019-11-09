using System;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	// Base class of all UI parts
	public abstract class UIElement
	{
		public Rect rect;
		public bool needsRecalculation;
		public abstract void SetRect(Rect rect);
		public abstract void Update();
	}
}

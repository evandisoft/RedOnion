using System;
using System.Collections.Generic;
using LiveRepl.UI.Elements;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	/// <summary>
	/// A Pane here is a specific custom division of a UI.
	/// You extend Pane, and build and connect custom parts in a tree structure.
	/// With explicit references. It determines how its children will be
	/// displayed.
	/// </summary>
	public abstract class Pane:IRectRenderable
	{
		public Rect rect;
		List<IUpdateable> updateables=new List<IUpdateable>();

		public virtual void Update()
		{
			GUI.BeginGroup(rect);
			foreach(var updateable in updateables)
			{
				updateable.Update();
			}
			GUI.EndGroup();
		}

		protected void RegisterForUpdate(IUpdateable updateable)
		{
			updateables.Add(updateable);
		}

		public abstract void SetRect(Rect rect);

		public Rect FillPane()
		{
			return new Rect(0, 0, rect.width, rect.height);
		}
	}
}

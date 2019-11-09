using System;
using System.Collections.Generic;
using LiveRepl.UI.Controls;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	/// <summary>
	/// A Pane has multiple sub Controls, and displays them within a GUI.BeginGroup on each update
	/// </summary>
	public abstract class Pane:UIElement
	{
		List<UIElement> elements=new List<UIElement>();

		public override void Update()
		{
			GUI.BeginGroup(rect);
			foreach(var updateable in elements)
			{
				updateable.Update();
			}
			GUI.EndGroup();
		}

		protected void RegisterForUpdate(UIElement element)
		{
			elements.Add(element);
		}

		public Rect FillPane()
		{
			return new Rect(0, 0, rect.width, rect.height);
		}
	}
}

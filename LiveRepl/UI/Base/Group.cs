using System;
using System.Collections.Generic;
using LiveRepl.UI.Controls;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	/// <summary>
	/// A Group has multiple sub Controls, and displays them within a GUI.BeginGroup on each update
	/// </summary>
	public abstract class Group:UIElement
	{
		List<UIElement> elements=new List<UIElement>();

		public override void Update()
		{
			GUI.BeginGroup(rect);
			foreach(var element in elements)
			{
				element.Update();
			}
			GUI.EndGroup();
		}

		/// <summary>
		/// Registers an element to be updated whenever update is called.
		/// </summary>
		/// <param name="element">Element.</param>
		protected void RegisterForUpdate(UIElement element)
		{
			elements.Add(element);
		}

		public Rect FillGroup()
		{
			return new Rect(0, 0, rect.width, rect.height);
		}
	}
}

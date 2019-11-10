using System;
using System.Collections.Generic;
using Kerbalui.Controls;
using UnityEngine;

namespace Kerbalui.Types
{
	/// <summary>
	/// A Group has multiple sub Elements, and displays them within a GUI.BeginGroup on each update
	/// </summary>
	public abstract class Group:Element
	{
		List<Element> elements=new List<Element>();

		/// <summary>
		/// Starts a GUI.BeginGroup, then calls GroupUpdate (which may be overriden in a subclass), and then
		/// calls update on all the elements.
		/// </summary>
		protected override void TypeSpecificUpdate()
		{
			GUI.BeginGroup(rect);
			GroupUpdate();
			foreach (var element in elements)
			{
				element.Update();
			}
			GUI.EndGroup();
		}

		/// <summary>
		/// Subclasses can extend this for it to be called at the start of GUI.BeginGroup.
		/// </summary>
		protected virtual void GroupUpdate()
		{
		}

		/// <summary>
		/// Registers an element to be updated whenever update is called.
		/// </summary>
		/// <param name="element">Element.</param>
		protected void RegisterForUpdate(Element element)
		{
			elements.Add(element);
		}

		public Rect FillGroup()
		{
			return new Rect(0, 0, rect.width, rect.height);
		}

		public override void SetRect(Rect rect)
		{
			base.SetRect(rect);
			SetChildRects();
		}

		/// <summary>
		/// Called by SetRect. This is implemented by the subclass and sets the all the rects for the children.
		/// </summary>
		protected abstract void SetChildRects();
	}
}

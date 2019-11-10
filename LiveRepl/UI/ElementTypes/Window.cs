using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveRepl.UI.ElementTypes
{
	/// <summary>
	/// Base class for a Window
	/// </summary>
	public abstract class Window:Element
	{
		static int NextID = 0;
		public readonly int WindowID=NextID++;

		static public GUISkin DefaultSkin=new GUISkin();
		public GUIContent TitleContent=new GUIContent("");
		public GUIStyle TitleStyle;
		Rect TitleRect;
		Rect ContentRect;

		public GUIStyle TitleStyleOrDefault
		{
			get
			{
				if (TitleStyle==null)
				{
					return DefaultSkin.window;
				}
				return TitleStyle;
			}
		}

		List<Element> elements=new List<Element>();

		public Window(string title)
		{
			TitleContent.text=title;
			needsRecalculation=true;
		}

		/// <summary>
		/// Registers an element to be updated whenever update is called.
		/// </summary>
		/// <param name="element">Element.</param>
		protected void RegisterForUpdate(Element element)
		{
			elements.Add(element);
		}

		protected override void TypeSpecificUpdate()
		{
			DefaultSkin=GUI.skin;
			GUI.DragWindow(TitleRect);
			rect=GUI.Window(WindowID, rect, PointlessFunc, TitleContent);
		}

		void PointlessFunc(int id)
		{
			WindowsUpdate();
		}

		protected virtual void WindowsUpdate()
		{
			GUI.BeginGroup(ContentRect);
			foreach (var element in elements)
			{
				element.Update();
			}
			GUI.EndGroup();
		}

		public override void SetRect(Rect rect)
		{
			base.SetRect(rect);

			TitleRect=new Rect(0, 0, rect.width, Math.Max(TitleStyleOrDefault.CalcSize(TitleContent).y, 10));
			ContentRect=new Rect(0, TitleRect.height, rect.width, rect.height-TitleRect.height);

			SetChildRects();
		}

		/// <summary>
		/// Called by SetRect. This is implemented by the subclass and sets the all the rects for the children.
		/// </summary>
		protected abstract void SetChildRects();
	}
}

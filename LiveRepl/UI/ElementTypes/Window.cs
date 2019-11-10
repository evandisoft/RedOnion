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
		static int nextID = 0;
		public readonly int windowID=nextID++;

		static public GUISkin defaultSkin=new GUISkin();
		public GUIContent titleContent=new GUIContent("");
		public GUIStyle titleStyle;
		Rect titleRect;
		Group content;

		public GUIStyle TitleStyleOrDefault
		{
			get
			{
				if (titleStyle==null)
				{
					return defaultSkin.textField;
				}
				return titleStyle;
			}
		}

		public Window(string title)
		{
			titleContent.text=title;
			needsRecalculation=true;
		}

		protected void AssignContent(Group content)
		{
			this.content=content;
		}

		protected override void TypeSpecificUpdate()
		{
			defaultSkin=GUI.skin;

			rect=GUI.Window(windowID, rect, PointlessFunc, titleContent);
		}

		void PointlessFunc(int id)
		{
			GUI.DragWindow(titleRect);
			WindowsUpdate();
		}

		protected virtual void WindowsUpdate()
		{
			content?.Update();
		}

		public override void SetRect(Rect rect)
		{
			base.SetRect(rect);

			titleRect=new Rect(0, 0, rect.width, Math.Max(TitleStyleOrDefault.CalcSize(titleContent).y, 10));
			content.SetRect(new Rect(0, titleRect.height, rect.width, rect.height-titleRect.height));
		}
	}
}

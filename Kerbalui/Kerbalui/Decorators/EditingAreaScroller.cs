using System;
using Kerbalui.EditingChanges;
using Kerbalui.EventHandling;
using Kerbalui.Interfaces;
using Kerbalui.Types;
using Kerbalui.Util;
using UnityEngine;
using static RedOnion.Debugging.QueueLogger;

namespace Kerbalui.Decorators
{
	public class EditingAreaScroller:Decorator, IEditingArea
	{
		protected EditingArea editingArea;

		public KeyBindings keybindings= new KeyBindings();

		public EditingAreaScroller(EditingArea editingArea)
		{
			this.editingArea=editingArea;
			horizontalStyle=new GUIStyle(GUI.skin.horizontalScrollbar);
			verticalStyle=new GUIStyle(GUI.skin.verticalScrollbar);
		}

		public string Text { get => editingArea.Text; set => editingArea.Text=value; }
		public int CursorIndex { get => editingArea.CursorIndex; set => editingArea.CursorIndex=value; }
		public int SelectIndex { get => editingArea.SelectIndex; set => editingArea.SelectIndex=value; }


		public bool HasFocus() => editingArea.HasFocus();
		public void GrabFocus() => editingArea.GrabFocus();

		public Vector2 scrollPos = new Vector2();
		bool resetScroll;
		protected Vector2 lastScrollViewVector2 = new Vector2();
		protected Vector2 lastContentVector2 = new Vector2();

		public virtual bool HorizontalScrollBarPresent { get; set; } = false;// rect.width<editingArea.rect.width;

		public virtual bool VerticalScrollBarPresent { get; set; } = false;//rect.height<editingArea.rect.height;

		public string ControlName => editingArea.ControlName;

		public bool ReceivedInput => editingArea.ReceivedInput;

		GUIStyle horizontalStyle;
		GUIStyle verticalStyle;


		public float ScrollbarWidth=>Math.Max(15,15*KerbaluiSettings.UI_SCALE);

		public virtual void ResetScroll()
		{
			resetScroll = true;
		}

		public override void SetRect(Rect newRect)
		{
			verticalStyle.fixedWidth=ScrollbarWidth;
			horizontalStyle.fixedHeight=ScrollbarWidth;
			base.SetRect(newRect);
		}

		protected override void SetChildRect()
		{
			Vector2 contentSize=editingArea.MinSize;
			editingArea.SetRect(new Rect(0, 0,
				Math.Max(contentSize.x, rect.width),
				Math.Max(contentSize.y, rect.height)
				));
		}

		protected override void DecoratorUpdate()
		{
			//bool receivedInput=false;
			if (HasFocus())
			{
				keybindings.ExecuteAndConsumeIfMatched(Event.current);
			}

			if ((Event.current.type==EventType.MouseDown || Event.current.type==EventType.ScrollWheel) && !HasFocus() && GUILibUtil.MouseInRect(rect))//.Contains(Event.current.mousePosition))
			{
				UILogger.DebugLogArray("Grabbing focus for control",editingArea.ControlName,"Event was", Event.current.type);
				//Debug.Log("edit area scroller grabbing mouse");
				GrabFocus();
			}


			scrollPos = GUI.BeginScrollView(rect, scrollPos, editingArea.rect, 
				HorizontalScrollBarPresent, VerticalScrollBarPresent,horizontalStyle, verticalStyle);
			{
				editingArea.Update();

				lastScrollViewVector2 = new Vector2(rect.width, rect.height);
				lastContentVector2 = new Vector2(rect.width, rect.height);

			}
			GUI.EndScrollView();

			if (resetScroll)
			{
				scrollPos.y = editingArea.rect.height;
				resetScroll = false;
			}

			if (editingArea.ReceivedInput)
			{
				//Complogger.Log("EditingAreaScroller",editingArea.editableText.ControlName," ReceivedInput");
				AdjustScrollX();
				AdjustScrollY();
			}
		}

		void AdjustScrollX()
		{
			//Debug.Log("Adjusting scroll x");
			float cursorX = editingArea.CursorX();
			float diff = lastContentVector2.x - lastScrollViewVector2.x;
			float contentStartX = scrollPos.x;
			float contentEndX = contentStartX + lastScrollViewVector2.x;
			if (Math.Max(cursorX - editingArea.Style.lineHeight, 0) < contentStartX)
			{
				scrollPos.x = Math.Max(cursorX - editingArea.Style.lineHeight, 0);
			}
			else if (cursorX + editingArea.Style.lineHeight > contentEndX)
			{
				scrollPos.x = cursorX - lastContentVector2.x + editingArea.Style.lineHeight;
			}
		}

		void AdjustScrollY()
		{
			//Debug.Log("Adjusting scroll y");
			float cursorY = editingArea.CursorY();
			//Debug.Log("CursorY " + cursorY);
			float diff = lastContentVector2.y - lastScrollViewVector2.y;
			//Debug.Log("diff " + diff);
			float contentStartY = scrollPos.y;
			//Debug.Log("contentStartY " + contentStartY);
			float contentEndY = contentStartY + lastScrollViewVector2.y;
			//Debug.Log("contentEndY " + contentEndY);
			if (cursorY - editingArea.Style.lineHeight < contentStartY)
			{
				scrollPos.y = cursorY - editingArea.Style.lineHeight;
				//Debug.Log("reducing to " + scrollPos.y);
			}
			else if (cursorY + editingArea.Style.lineHeight > contentEndY)
			{
				scrollPos.y = cursorY - lastContentVector2.y + editingArea.Style.lineHeight;
				//Debug.Log("expanding to " + scrollPos.y);
			}
		}
	}
}

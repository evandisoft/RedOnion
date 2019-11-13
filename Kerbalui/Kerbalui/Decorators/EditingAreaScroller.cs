using System;
using Kerbalui.EventHandling;
using Kerbalui.Types;
using Kerbalui.Util;
using UnityEngine;

namespace Kerbalui.Decorators
{
	public class EditingAreaScroller:Decorator
	{
		public EditingArea editingArea;

		public KeyBindings keybindings= new KeyBindings();

		public EditingAreaScroller(EditingArea editingArea)
		{
			this.editingArea=editingArea;
		}

		public bool HasFocus() => editingArea.HasFocus();
		public void GrabFocus() => editingArea.GrabFocus();

		public Vector2 scrollPos = new Vector2();
		bool resetScroll;
		protected Vector2 lastScrollViewVector2 = new Vector2();
		protected Vector2 lastContentVector2 = new Vector2();

		public bool HorizontalScrollBarPresent => rect.width<editingArea.rect.width;

		public bool VerticalScrollBarPresent => rect.height<editingArea.rect.height;

		public const int ScrollbarWidth=20;

		public virtual void ResetScroll()
		{
			resetScroll = true;
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
			bool receivedInput=false;
			if (HasFocus())
			{
				keybindings.ExecuteAndConsumeIfMatched(Event.current);
			}

			scrollPos = GUI.BeginScrollView(rect, scrollPos, editingArea.rect);
			{
				// Without this, it takes two clicks to update the cursor when the editingArea is not focused.
				if ((Event.current.type==EventType.MouseDown || Event.current.type==EventType.ScrollWheel) && !HasFocus() && GUILibUtil.MouseInRect(rect))
				{
					//Debug.Log("edit area scroller mouse check");
					GrabFocus();
				}

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
			if (Math.Max(cursorX - editingArea.editableText.style.lineHeight, 0) < contentStartX)
			{
				scrollPos.x = Math.Max(cursorX - editingArea.editableText.style.lineHeight, 0);
			}
			else if (cursorX + editingArea.editableText.style.lineHeight > contentEndX)
			{
				scrollPos.x = cursorX - lastContentVector2.x + editingArea.editableText.style.lineHeight;
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
			if (cursorY - editingArea.editableText.style.lineHeight < contentStartY)
			{
				scrollPos.y = cursorY - editingArea.editableText.style.lineHeight;
				//Debug.Log("reducing to " + scrollPos.y);
			}
			else if (cursorY + editingArea.editableText.style.lineHeight > contentEndY)
			{
				scrollPos.y = cursorY - lastContentVector2.y + editingArea.editableText.style.lineHeight;
				//Debug.Log("expanding to " + scrollPos.y);
			}
		}
	}
}

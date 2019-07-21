using System;
using UnityEngine;

namespace Kerbalui.Gui
{
	public class ScrollableTextArea : TextArea
	{
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;
		protected Vector2 lastScrollViewVector2 = new Vector2();
		protected Vector2 lastContentVector2 = new Vector2();

		protected override void ProtectedUpdate(Rect rect)
		{

			GUI.BeginGroup(rect);
			{
				rect.x = 0;
				rect.y = 0;
				HandleScrolling(rect);

				if (style == null)
				{
					style = new GUIStyle(GUI.skin.textArea);
				}

				Vector2 contentSize = style.CalcSize(content);
				Rect contentRect = new Rect(0, 0,
					Math.Max(contentSize.x, rect.width),
					Math.Max(contentSize.y, rect.height)
					);
				if (!InputLockManager.IsLocked(ControlTypes.KEYBOARDINPUT) && Event.current.isScrollWheel)
				{

				}
				else
				{
					scrollPos = GUI.BeginScrollView(rect, scrollPos, contentRect);
					{


						base.ProtectedUpdate(contentRect);

						if (resetScroll)
						{
							scrollPos.y = contentRect.height;
							resetScroll = false;
							//selectIndex = cursorIndex = content.text.Length;
						}

						lastScrollViewVector2 = new Vector2(rect.width, rect.height);
						lastContentVector2 = new Vector2(rect.width, rect.height);

					}
					GUI.EndScrollView();
				}
			}
			GUI.EndGroup();
		}

		//protected override void ProtectedUpdate()
		//{
		//	throw new NotSupportedException();
		//}

		// Manually handling scrolling because there is a coordinate issue
		// with the default method. The system measures the mouse vertical
		// position from the bottom instead of the top of the screen for scrolling
		// purposes for whatever bizarre reason.
		void HandleScrolling(Rect rect)
		{
			if (InputLockManager.IsLocked(ControlTypes.KEYBOARDINPUT))
			{
				if (Event.current.isScrollWheel)
				{
					Event.current.Use();
				}
			}
			//Input.mouseScrollDelta
			//if (Event.current.isScrollWheel && GUIUtil.MouseInRect(rect))
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (Input.GetAxis("Mouse ScrollWheel") != 0 && GUILibUtil.MouseInRect(rect))
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
			{
				float delta = 0;
				if (Input.GetAxis("Mouse ScrollWheel") > 0)
				{
					delta = -1;
				}
				else
				{
					delta = 1;
				}

				scrollPos.y += delta * 20;
				//Event.current.Use();
			}
		}

		public virtual void ResetScroll()
		{
			resetScroll = true;
		}
	}
}

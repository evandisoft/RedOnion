using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Decorators
{
	/// <summary>
	/// Contains a ContentControl, which it manages scrolling for.
	/// </summary>
	public class ContentScroller:Decorator
	{
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;
		protected Vector2 lastScrollViewVector2 = new Vector2();
		protected Vector2 lastContentVector2 = new Vector2();

		public bool HorizontalScrollBarPresent => rect.width<contentControl.rect.width;
		public bool VerticalScrollBarPresent => rect.height<contentControl.rect.height;
		public const int ScrollbarWidth=20;

		public ContentControl contentControl;

		public ContentScroller(ContentControl contentControl)
		{
			this.contentControl=contentControl;
		}

		public virtual void ResetScroll()
		{
			resetScroll = true;
		}

		protected override void SetChildRect()
		{
			Vector2 contentSize = contentControl.StyleOrDefault.CalcSize(contentControl.content);
			contentControl.rect = new Rect(0, 0,
				Math.Max(contentSize.x, rect.width),
				Math.Max(contentSize.y, rect.height)
				);
			
		}

		protected override void DecoratorUpdate()
		{
			scrollPos = GUI.BeginScrollView(rect, scrollPos, contentControl.rect);
			{
				contentControl.Update();

				if (resetScroll)
				{
					scrollPos.y = contentControl.rect.height;
					resetScroll = false;
				}

				lastScrollViewVector2 = new Vector2(rect.width, rect.height);
				lastContentVector2 = new Vector2(rect.width, rect.height);

			}
			GUI.EndScrollView();
		}
	}
}

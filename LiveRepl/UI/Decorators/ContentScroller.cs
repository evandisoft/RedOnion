using System;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Widgets
{
	/// <summary>
	/// Contains a ContentControl, which it manages scrolling for.
	/// </summary>
	public class ContentScroller:Widget
	{
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;
		protected Vector2 lastScrollViewVector2 = new Vector2();
		protected Vector2 lastContentVector2 = new Vector2();

		public ContentControl contentControl;

		public ContentScroller(ContentControl contentControl)
		{
			this.contentControl=contentControl;
		}

		public override void Update()
		{
			if (contentControl.style == null)
			{
				contentControl.style = new GUIStyle(GUI.skin.textArea);
			}

			Vector2 contentSize = contentControl.style.CalcSize(contentControl.content);
			contentControl.rect = new Rect(0, 0,
				Math.Max(contentSize.x, rect.width),
				Math.Max(contentSize.y, rect.height)
				);

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

		public virtual void ResetScroll()
		{
			resetScroll = true;
		}

		public override void SetRect(Rect rect)
		{
			throw new NotImplementedException();
		}
	}
}

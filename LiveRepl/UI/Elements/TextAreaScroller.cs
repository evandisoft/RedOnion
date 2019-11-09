using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public class TextAreaScroller:Element
	{
		public Vector2 scrollPos = new Vector2();
		bool resetScroll;
		protected Vector2 lastScrollViewVector2 = new Vector2();
		protected Vector2 lastContentVector2 = new Vector2();

		public TextArea textArea;

		public TextAreaScroller(TextArea textArea)
		{
			this.textArea=textArea;
		}

		public override void Update()
		{
			if (textArea.style == null)
			{
				textArea.style = new GUIStyle(GUI.skin.textArea);
			}

			Vector2 contentSize = textArea.style.CalcSize(textArea.content);
			textArea.rect = new Rect(0, 0,
				Math.Max(contentSize.x, rect.width),
				Math.Max(contentSize.y, rect.height)
				);

			scrollPos = GUI.BeginScrollView(rect, scrollPos, textArea.rect);
			{
				textArea.Update();

				if (resetScroll)
				{
					scrollPos.y = textArea.rect.height;
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
	}
}

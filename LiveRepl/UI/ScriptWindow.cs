using Kerbalui.Types;
using LiveRepl.UI.Layout;
using UnityEngine;

namespace LiveRepl.UI
{
	public class ScriptWindow:Window
	{
		public const float windowHeight=600;
		public const float editorGroupWidth = 500;
		public const float centerGroupWidth = 100;
		public const float replGroupWidth = 400;
		public const float completionGroupWidth = 150;

		public const float startingX = 100;
		public const float startingY = 100;

		public ContentGroup contentGroup;

		public ScriptWindow(string title) : base(title)
		{
			AssignContent(contentGroup=new ContentGroup(this));

			rect.x=startingX;
			rect.y=startingY;
			rect.width=WindowWidth();
			rect.height=windowHeight;
		}

		public override void SetRect(Rect rect)
		{
			rect.width=WindowWidth();
			rect.height=windowHeight;

			base.SetRect(rect);
		}

		protected override void WindowsUpdate()
		{
			base.WindowsUpdate();
		}

		float WindowWidth()
		{
			float width=centerGroupWidth;
			if (EditorVisible || ReplVisible)
			{
				width+=completionGroupWidth;
				if (EditorVisible)
				{
					width+=editorGroupWidth;
				}
				if (ReplVisible)
				{
					width+=replGroupWidth;
				}
			}
			return width;
		}

		private bool editorVisible = true;
		public bool EditorVisible
		{
			get => editorVisible;
			set
			{
				if (value)
				{
					if (!editorVisible)
					{
						rect.x-=editorGroupWidth;
						rect.width+=editorGroupWidth;
					}
				}
				else
				{
					if (editorVisible)
					{
						rect.x+=editorGroupWidth;
						rect.width-=editorGroupWidth;
					}
				}
				editorVisible=value;
			}
		}

		private bool replVisible = true;
		public bool ReplVisible
		{
			get => replVisible;
			set
			{
				if (value)
				{
					if (!replVisible)
					{
						rect.width+=replGroupWidth;
					}
				}
				else
				{
					if (replVisible)
					{
						rect.width-=replGroupWidth;
					}
				}
				replVisible=value;
			}
		}
	}
}

using Kerbalui.Types;
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

		public void ToggleEditor()
		{
			EditorVisible=!editorVisible;
		}

		public void ToggleRepl()
		{
			ReplVisible=!replVisible;
		}

		private bool editorVisible = true;
		public bool EditorVisible
		{
			get => editorVisible;
			set
			{
				if (value!=editorVisible) 
				{
					if (editorVisible)
					{
						rect.x+=editorGroupWidth;
						rect.width-=editorGroupWidth;
					}
					else
					{
						rect.x-=editorGroupWidth;
						rect.width+=editorGroupWidth;
					}

					contentGroup.editorGroup.active=editorVisible=value;
					contentGroup.completionGroup.active=editorVisible || replVisible;
					needsResize=true;
				}
			}
		}

		private bool replVisible = true;
		public bool ReplVisible
		{
			get => replVisible;
			set
			{
				if (value!=replVisible)
				{
					if (replVisible)
					{
						rect.width-=replGroupWidth;
					}
					else
					{
						rect.width+=replGroupWidth;
					}


					contentGroup.replGroup.active=replVisible=value;
					contentGroup.completionGroup.active=editorVisible || replVisible;
					needsResize=true;
				}
			}
		}
	}
}

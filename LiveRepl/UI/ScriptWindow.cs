using System;
using LiveRepl.UI.ElementTypes;
using LiveRepl.UI.Layout;
using UnityEngine;

namespace LiveRepl.UI
{
	public class ScriptWindow:Window
	{
		public bool editorVisible = true;
		public bool replVisible = true;

		public const float windowHeight=600;
		public const float editorGroupWidth = 500;
		public const float centerGroupWidth = 100;
		public const float replGroupWidth = 400;
		public const float completionGroupWidth = 150;

		float WindowWidth()
		{
			float width=centerGroupWidth;
			if (editorVisible || replVisible)
			{
				width+=completionGroupWidth;
				if (editorVisible)
				{
					width+=editorGroupWidth;
				}
				if (replVisible)
				{
					width+=replGroupWidth;
				}
			}
			return width;
		}

		float WindowX()
		{
			float x=baseWindowPos.x;
			if (!editorVisible)
			{
				x+=editorGroupWidth;
			}
			return x;
		}

		Vector2 baseWindowPos=new Vector2(100,100);

		public EditorGroup editorGroup;
		public CenterGroup centerGroup;
		public ReplGroup replGroup;
		public CompletionGroup completionGroup;

		public ScriptWindow(string title):base(title)
		{
			RegisterForUpdate(editorGroup=new EditorGroup(this));
			RegisterForUpdate(centerGroup=new CenterGroup(this));
			RegisterForUpdate(replGroup=new ReplGroup(this));
			RegisterForUpdate(completionGroup=new CompletionGroup(this));
		}

		public override void SetRect(Rect rect)
		{
			base.SetRect(rect);
		}

		protected override void SetChildRects()
		{
			editorGroup.SetRect(new Rect(0, 0, editorGroupWidth, rect.height));

			float x=0;
			if (editorVisible)
			{
				x+=editorGroup.rect.width;
			}
			centerGroup.SetRect(new Rect(x, 0, centerGroupWidth, rect.height));

			Rect centerGroupRect=centerGroup.rect;
			x=centerGroupRect.x+centerGroupRect.width;
			replGroup.SetRect(new Rect(x, 0, replGroupWidth, rect.height));

			x=centerGroupRect.x+centerGroupRect.width;
			if (replVisible)
			{
				x+=replGroup.rect.width;
			}
			completionGroup.SetRect(new Rect(x, 0, completionGroupWidth, rect.height));
		}

		protected override void WindowsUpdate()
		{

			base.WindowsUpdate();
		}


	}
}

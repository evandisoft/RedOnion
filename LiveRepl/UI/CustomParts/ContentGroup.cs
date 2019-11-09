using System;
using Kerbalui.Gui;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.CustomParts
{
	/// <summary>
	/// The Group that holds the Editor, Center, Repl, and CompletionArea Groups.
	/// </summary>
	public class ContentGroup:Group
	{
		public ScriptWindow scriptWindow;

		public EditorGroup editorGroup;
		public CenterGroup centerGroup;
		public ReplGroup replGroup;
		public CompletionGroup completionGroup;

		public ContentGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;

			RegisterForUpdate(editorGroup=new EditorGroup(this));
			RegisterForUpdate(centerGroup=new CenterGroup(this));
			RegisterForUpdate(replGroup=new ReplGroup(this));
			RegisterForUpdate(completionGroup=new CompletionGroup(this));
		}

		public override void SetRect(Rect rect)
		{
			this.rect=rect;
			editorGroup.SetRect(new Rect(0, 0, ScriptWindow.editorGroupWidth, rect.height));

			float x=0;
			if (scriptWindow.editorVisible)
			{
				x+=editorGroup.rect.width;
			}
			centerGroup.SetRect(new Rect(x, 0, ScriptWindow.centerGroupWidth, rect.height));

			Rect centerGroupRect=centerGroup.rect;
			x=centerGroupRect.x+centerGroupRect.width;
			replGroup.SetRect(new Rect(x, 0, ScriptWindow.replGroupWidth, rect.height));

			x=centerGroupRect.x+centerGroupRect.width;
			if (scriptWindow.replVisible)
			{
				x+=replGroup.rect.width;
			}
			completionGroup.SetRect(new Rect(x, 0, ScriptWindow.completionGroupWidth, rect.height));
		}
	}
}

using Kerbalui.Types;
using LiveRepl.UI.Center;
using LiveRepl.UI.Completion;
using LiveRepl.UI.Editor;
using LiveRepl.UI.Repl;
using UnityEngine;

namespace LiveRepl.UI
{
	/// <summary>
	/// This Group represents the content area of the ScriptWindow
	/// </summary>
	public class ContentGroup:Group
	{
		public ScriptWindow scriptWindow;

		private EditorGroup editorGroup;
		private CenterGroup centerGroup;
		private ReplGroup replGroup;
		private CompletionGroup completionGroup;

		public ContentGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;

			RegisterForUpdate(editorGroup=new EditorGroup(this));
			RegisterForUpdate(centerGroup=new CenterGroup(this));
			RegisterForUpdate(replGroup=new ReplGroup(this));
			RegisterForUpdate(completionGroup=new CompletionGroup(this));
		}

		protected override void SetChildRects()
		{
			editorGroup.SetRect(new Rect(0, 0, ScriptWindow.editorGroupWidth, rect.height));

			float x=0;
			if (scriptWindow.EditorVisible)
			{
				x+=editorGroup.rect.width;
			}
			centerGroup.SetRect(new Rect(x, 0, ScriptWindow.centerGroupWidth, rect.height));

			Rect centerGroupRect=centerGroup.rect;
			x=centerGroupRect.x+centerGroupRect.width;
			replGroup.SetRect(new Rect(x, 0, ScriptWindow.replGroupWidth, rect.height));

			x=centerGroupRect.x+centerGroupRect.width;
			if (scriptWindow.ReplVisible)
			{
				x+=replGroup.rect.width;
			}
			completionGroup.SetRect(new Rect(x, 0, ScriptWindow.completionGroupWidth, rect.height));
		}
	}
}

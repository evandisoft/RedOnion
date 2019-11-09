using System;
using Kerbalui.Gui;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	/// <summary>
	/// The pane that holds the Editor, Center, Repl, and CompletionArea Panes.
	/// </summary>
	public class ContentPane:Pane
	{
		public ScriptWindow scriptWindow;

		public EditorPane editorPane;
		public CenterPane centerPane;
		public ReplPane replPane;
		public CompletionPane completionPane;

		public ContentPane(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;

			RegisterForUpdate(editorPane=new EditorPane(this));
			RegisterForUpdate(centerPane=new CenterPane(this));
			RegisterForUpdate(replPane=new ReplPane(this));
			RegisterForUpdate(completionPane=new CompletionPane(this));
		}

		public override void SetRect(Rect rect)
		{
			this.rect=rect;
			editorPane.SetRect(new Rect(0, 0, ScriptWindow.editorPaneWidth, rect.height));

			float x=0;
			if (scriptWindow.editorVisible)
			{
				x+=editorPane.rect.width;
			}
			centerPane.SetRect(new Rect(x, 0, ScriptWindow.centerPaneWidth, rect.height));

			Rect centerPaneRect=centerPane.rect;
			x=centerPaneRect.x+centerPaneRect.width;
			replPane.SetRect(new Rect(x, 0, ScriptWindow.replPaneWidth, rect.height));

			x=centerPaneRect.x+centerPaneRect.width;
			if (scriptWindow.replVisible)
			{
				x+=replPane.rect.width;
			}
			completionPane.SetRect(new Rect(x, 0, ScriptWindow.completionPaneWidth, rect.height));
		}
	}
}

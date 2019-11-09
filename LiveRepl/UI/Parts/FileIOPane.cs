using System;
using LiveRepl.UI.Base;
using LiveRepl.UI.Controls;
using LiveRepl.UI.Panes;
using UnityEngine;

namespace LiveRepl.UI.Parts
{
	public class FileIOPane:Pane
	{
		public EditorPane editorPane;

		public HorizontalSpacer horizontalSpacer;

		public Label changesIndicator=new Label("*");

		public FileIOPane(EditorPane editorPane)
		{
			this.editorPane=editorPane;

			RegisterForUpdate(horizontalSpacer=new HorizontalSpacer());
			horizontalSpacer.Add(3, new TextField());
			horizontalSpacer.Add(0, changesIndicator);
			horizontalSpacer.Add(1, new Button("Save", () => throw new NotImplementedException("Save Button not Implemented")));
			horizontalSpacer.Add(1, new Button("Load", () => throw new NotImplementedException("Load Button not Implemented")));
			horizontalSpacer.Add(1, new Button("Run", () => throw new NotImplementedException("Run Button not Implemented")));
		}

		public override void SetRect(Rect rect)
		{
			this.rect=rect;

			horizontalSpacer.SetRect(FillPane());
		}
	}
}

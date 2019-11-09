using System;
using LiveRepl.UI.Base;
using LiveRepl.UI.Controls;
using LiveRepl.UI.Groups;
using UnityEngine;

namespace LiveRepl.UI.Parts
{
	public class FileIOGroup:Group
	{
		public EditorGroup editorGroup;

		public HorizontalSpacer horizontalSpacer;

		public Label changesIndicator=new Label("*");

		public FileIOGroup(EditorGroup editorGroup)
		{
			this.editorGroup=editorGroup;

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

			horizontalSpacer.SetRect(FillGroup());
		}
	}
}

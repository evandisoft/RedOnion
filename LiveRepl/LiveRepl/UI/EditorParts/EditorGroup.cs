using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Decorators;
using Kerbalui.Controls;

namespace LiveRepl.UI.EditorParts
{
	/// <summary>
	/// The Group that holds the Editor and related functionality.
	/// </summary>
	public class EditorGroup : Group
	{
		public ContentGroup contentGroup;

		public FileIOGroup fileIOGroup;
		public Editor editor;
		public EditorStatusLabel editorStatusLabel;

		public EditorGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			RegisterForUpdates(fileIOGroup=new FileIOGroup(this));
			RegisterForUpdates(editor=new Editor(this));
			RegisterForUpdates(editorStatusLabel=new EditorStatusLabel());
		}

		protected override void GroupUpdate()
		{
			//TODO: This isn't correct at the moment
			if (Event.current.type==EventType.KeyDown)
			{
				fileIOGroup.horizontalSpacer.needsRecalculation=true;
			}
		}

		protected override void SetChildRects()
		{
			fileIOGroup.SetRect(new Rect(0, 0, rect.width, fileIOGroup.horizontalSpacer.MinSize.y));
			//TODO: TabsGroup
			//y+=fileIOGroup.rect.height;
			//height-=fileIOGroup.rect.height;
			editorStatusLabel.SetRect(new Rect(0,rect.height-editorStatusLabel.MinSize.y, rect.width, editorStatusLabel.MinSize.y));

			editor.SetRect(new Rect(0, fileIOGroup.rect.height, rect.width, rect.height-fileIOGroup.rect.height-editorStatusLabel.rect.height));

		}
	}
}
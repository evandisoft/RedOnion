using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.Groups;

namespace LiveRepl.UI.EditorParts
{
	/// <summary>
	/// The Group that holds the Editor and related functionality.
	/// </summary>
	public class EditorGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public FileIOGroup fileIOGroup;
		public Editor editor;
		public EditorStatusLabel editorStatusLabel;

		public EditorGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddMinSized(fileIOGroup=new FileIOGroup(this));
			AddWeighted(1,editor=new Editor(this));
			AddMinSized(editorStatusLabel=new EditorStatusLabel());
		}

		protected override void GroupUpdate()
		{
			if (Event.current.type==EventType.KeyDown)
			{
				fileIOGroup.needsResize=true;
			}
		}

		//protected override void SetChildRects()
		//{
		//	fileIOGroup.SetRect(new Rect(0, 0, rect.width, fileIOGroup.MinSize.y));
		//	//TODO: TabsGroup
		//	//y+=fileIOGroup.rect.height;
		//	//height-=fileIOGroup.rect.height;
		//	editorStatusLabel.SetRect(new Rect(0,rect.height-editorStatusLabel.MinSize.y, rect.width, editorStatusLabel.MinSize.y));

		//	editor.SetRect(new Rect(0, fileIOGroup.rect.height, rect.width, rect.height-fileIOGroup.rect.height-editorStatusLabel.rect.height));

		//}
	}
}
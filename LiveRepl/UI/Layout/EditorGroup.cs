using LiveRepl.UI.ElementTypes;
using LiveRepl.UI.Controls;
using LiveRepl.UI.Decorators;
using UnityEngine;

namespace LiveRepl.UI.Layout
{
	/// <summary>
	/// The Group that holds the Editor and related functionality.
	/// </summary>
	public class EditorGroup : Group
	{
		public ScriptWindow scriptWindow;

		public FileIOGroup fileIOGroup;
		public EditingArea editingArea;

		public EditorGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;

			RegisterForUpdate(fileIOGroup=new FileIOGroup(this));
			RegisterForUpdate(editingArea=new EditingArea(new TextArea()));
		}

		protected override void GroupUpdate()
		{
			if (Event.current.type==EventType.KeyDown)
			{
				fileIOGroup.horizontalSpacer.needsRecalculation=true;
			}
		}

		protected override void SetChildRects()
		{
			fileIOGroup.SetRect(new Rect(0, 0, rect.width, 20));
			//TODO: TabsGroup
			editingArea.SetRect(new Rect(0, fileIOGroup.rect.height, rect.width, rect.height-fileIOGroup.rect.height));
			//TODO: EditorStatusGroup
		}
	}
}
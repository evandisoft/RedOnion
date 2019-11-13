using Kerbalui.Layout;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.Parts
{
	/// <summary>
	/// This Group represents the content area of the ScriptWindow
	/// </summary>
	public class ContentGroup:HorizontalSpacer
	{
		ScriptWindowParts uiparts;

		public ContentGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddFixed(ScriptWindow.replGroupWidth, uiparts.replGroup=new ReplGroup(uiparts));
			AddFixed(ScriptWindow.centerGroupWidth, uiparts.centerGroup=new CenterGroup(uiparts));
			AddFixed(ScriptWindow.completionGroupWidth, uiparts.completionGroup=new CompletionGroup(uiparts));
			AddFixed(ScriptWindow.editorGroupWidth, uiparts.editorGroup=new EditorGroup(uiparts));
		}
	}
}

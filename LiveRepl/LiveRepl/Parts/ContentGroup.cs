using Kerbalui.Layout;
using Kerbalui.Types;
using LiveRepl.Decorators;
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
			AddFixed(ScriptWindow.completionGroupWidth, new ScriptDisabledElement(uiparts,uiparts.completionGroup=new CompletionGroup(uiparts)));
			AddFixed(ScriptWindow.editorGroupWidth, new ScriptDisabledElement(uiparts, uiparts.editorGroup=new EditorGroup(uiparts)));
		}
	}
}

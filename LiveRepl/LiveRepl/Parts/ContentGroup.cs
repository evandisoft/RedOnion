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

			AddFixed(ScriptWindow.baseReplGroupWidth, uiparts.replGroup=new ReplGroup(uiparts));
			AddFixed(ScriptWindow.baseCenterGroupWidth, uiparts.centerGroup=new CenterGroup(uiparts));
			AddFixed(ScriptWindow.baseEditorGroupWidth, uiparts.scriptDisabledEditorGroup=new ScriptDisabledElement(uiparts, uiparts.editorGroup=new EditorGroup(uiparts)));
			AddFixed(ScriptWindow.baseCompletionGroupWidth, uiparts.ScriptDisabledCompletionGroup=new ScriptDisabledElement(uiparts, uiparts.completionGroup=new CompletionGroup(uiparts)));

		}
	}
}

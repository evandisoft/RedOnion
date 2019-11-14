using Kerbalui.Controls;
using Kerbalui.Layout;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.Parts
{
	/// <summary>
	/// The Group that contains the completion area.
	/// </summary>
	public class CompletionGroup : VerticalSpacer
	{
		ScriptWindowParts uiparts;

		public CompletionGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddMinSized(uiparts.scriptNameInputArea=new ScriptNameInputArea(uiparts));
			AddWeighted(1, uiparts.completionArea=new CompletionArea(uiparts));
		}
	}
}
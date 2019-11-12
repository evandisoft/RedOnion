using Kerbalui.Layout;
using Kerbalui.Types;
using LiveRepl.UI.CenterParts;
using LiveRepl.UI.CompletionParts;
using LiveRepl.UI.EditorParts;
using LiveRepl.UI.ReplParts;
using UnityEngine;

namespace LiveRepl
{
	/// <summary>
	/// This Group represents the content area of the ScriptWindow
	/// </summary>
	public class ContentGroup:HorizontalSpacer
	{
		public ScriptWindow scriptWindow;

		public EditorGroup editorGroup;
	    public CenterGroup centerGroup;
		public ReplGroup replGroup;
		public CompletionGroup completionGroup;

		public ContentGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;

			AddFixed(ScriptWindow.editorGroupWidth, editorGroup=new EditorGroup(this));
			AddFixed(ScriptWindow.centerGroupWidth, centerGroup=new CenterGroup(this));
			AddFixed(ScriptWindow.replGroupWidth, replGroup=new ReplGroup(this));
			AddFixed(ScriptWindow.completionGroupWidth, completionGroup=new CompletionGroup(this));
		}
	}
}

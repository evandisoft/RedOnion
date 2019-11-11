using System;
using Kerbalui.Controls;
using Kerbalui.Groups;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.UI.CenterParts
{
	/// <summary>
	/// The Center Group between the Editor and Repl.
	/// Will tend to contain functionality for the overall ScriptWindow
	/// </summary>
	public class CenterGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public CenterGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddMinSized(new Button("Stop", ()=>throw new NotImplementedException("Centergroup stop button")));
			AddMinSized(new Button("<<", contentGroup.scriptWindow.ToggleEditor));
			AddMinSized(new Button(">>", contentGroup.scriptWindow.ToggleRepl));
		}
	}
}
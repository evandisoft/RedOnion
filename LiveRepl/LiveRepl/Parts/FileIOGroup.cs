using System;
using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Layout;
using Kerbalui.Controls;

namespace LiveRepl.Parts
{
	/// <summary>
	/// This group holds the FileIO related functionality, including save, load, and also has the "run script" button.
	/// </summary>
	public class FileIOGroup:HorizontalSpacer
	{
		public ScriptWindowParts uiparts;

		public FileIOGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddMinSized(uiparts.editorChangesIndicator=new EditorChangesIndicator(uiparts));
			//AddWeighted(3, uiparts.scriptNameInputArea=new ScriptNameInputArea(uiparts));
		}
	}
}

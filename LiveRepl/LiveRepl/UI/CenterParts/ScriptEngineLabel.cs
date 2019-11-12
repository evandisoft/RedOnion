using System;
using Kerbalui.Controls;

namespace LiveRepl.UI.CenterParts
{
	public class ScriptEngineLabel:Label
	{
		public ScriptEngineLabel()
		{
			SetEngine("None");
		}

		public void SetEngine(string engineName)
		{
			content.text="Engine: "+engineName;
		}
	}
}

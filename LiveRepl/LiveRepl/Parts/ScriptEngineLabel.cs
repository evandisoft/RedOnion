using System;
using Kerbalui.Controls;
using UnityEngine;

namespace LiveRepl.Parts
{
	public class ScriptEngineLabel:Label
	{
		public ScriptEngineLabel()
		{
			SetEngine("None");
			Style.alignment=TextAnchor.MiddleCenter;
		}

		public void SetEngine(string engineName)
		{
			Content.text="Engine: "+engineName;
		}
	}
}

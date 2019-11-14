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
		}

		public void SetEngine(string engineName)
		{
			content.text="Engine: "+engineName;
		}

		protected override void ControlUpdate()
		{
			if (style==null)
			{
				style=new GUIStyle(DefaultStyle());
				style.alignment=TextAnchor.MiddleCenter;
			}

			base.ControlUpdate();
		}
	}
}

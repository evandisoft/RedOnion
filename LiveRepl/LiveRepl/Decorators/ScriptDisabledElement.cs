using System;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.Decorators
{
	public class ScriptDisabledElement:Decorator
	{
		ScriptWindowParts uiparts;
		Element element;

		public ScriptDisabledElement(ScriptWindowParts uiparts,Element element)
		{
			this.uiparts=uiparts;
			this.element=element;
		}

		protected override void DecoratorUpdate()
		{
			bool prevEnabled=GUI.enabled;
			GUI.enabled=!uiparts.scriptWindow.ScriptRunning;
			element.Update();
			GUI.enabled=prevEnabled;
		}

		protected override void SetChildRect()
		{
			element.SetRect(rect);
		}

		public override Vector2 MinSize => element.MinSize;
	}
}

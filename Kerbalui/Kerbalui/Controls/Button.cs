using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls {
	public class Button:ContentControl {
		protected Action action;

		public Button(string text, Action action)
		{
			content.text = text;
			this.action = action;
		}

		public virtual Action Action { get => action; set => action=value; }

		public override GUIStyle DefaultStyle()
		{
			return Window.defaultSkin.button;
		}

		protected override void ControlUpdate()
		{
			if (GUI.Button(rect, content, StyleOrDefault))
			{
				action.Invoke();
			}
		}
	}
}

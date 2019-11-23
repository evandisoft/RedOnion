using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Controls {
	public class Button:ContentControl {
		protected Action action;

		public Button(string text, Action action):base(GUI.skin.button)
		{
			Content.text = text;
			this.action = action;
		}

		public virtual Action Action { get => action; set => action=value; }

		protected override void ControlUpdate()
		{
			if (GUI.Button(rect, Content, Style))
			{
				action.Invoke();
			}
		}
	}
}

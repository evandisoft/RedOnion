using System;
using Kerbalui.Controls;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;

namespace LiveRepl.Parts
{
	public class QueueTagInputArea : EditingArea
	{
		public QueueTagInputArea() : base(new TextField())
		{
			keybindings.Clear();
		}
	}
}

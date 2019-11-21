using System;
using Kerbalui.Controls;
using Kerbalui.Layout;
using RedOnion.KSP.Debugging;

namespace LiveRepl.Parts
{
	public class PrintQueuesArea:HorizontalSpacer
	{
		ScriptWindowParts uiparts;

		QueueTagInputArea queueTagInputArea;

		public PrintQueuesArea(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddWeighted(1, new Button("Print Logs", () =>
			{
				uiparts.replOutoutArea.AddText(QueueLogger.GetContentsByTag(queueTagInputArea.Text));
			}));
			AddWeighted(1, queueTagInputArea=new QueueTagInputArea());
		}
	}
}

using System;
using Kerbalui.Controls;
using Kerbalui.Layout;
using MunOS.Debugging;
using RedOnion.KSP.Settings;

namespace LiveRepl.Parts
{
	public class PrintQueuesArea:HorizontalSpacer
	{
		ScriptWindowParts uiparts;

		public PrintQueuesArea(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddWeighted(1, new Button("Print", () =>
			{
				uiparts.scriptWindow.currentEngineProcess.outputBuffer.AddText(QueueLogger.GetContentsByTag(uiparts.queueTagInputArea.Text));
			}));
			AddWeighted(1, new Button("Clear", () =>
			{
				QueueLogger.ClearLoggersByTag(uiparts.queueTagInputArea.Text);
			}));
			AddWeighted(2, uiparts.queueTagInputArea=new QueueTagInputArea());
			//uiparts.FontChange+=uiparts.queueTagInputArea.editableText.FontChangeEventHandler;
			uiparts.queueTagInputArea.Text=SavedSettings.LoadSetting("lastQueueTag", "ui");
		}
	}
}

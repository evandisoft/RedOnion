using System.Text;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.Util;
using UnityEngine;

namespace LiveRepl.Parts
{
	public class ReplOutoutArea:EditingAreaScroller
	{
		public ScriptWindowParts uiparts;

		public ReplOutoutArea(ScriptWindowParts uiparts) : base(new EditingArea(new TextArea()))
		{
			this.uiparts=uiparts;
			editingArea.Style.alignment = TextAnchor.LowerLeft;
			editingArea.Style.font = GUILibUtil.GetMonoSpaceFont();

			uiparts.FontChange+=editingArea.FontChangeEventHandler;
		}

		//public StringBuilder outputBuffer=new StringBuilder();

		//public const int OUTPUT_LENGTH_LIMIT = 10000;

		//void CommonOutputProcessing()
		//{
		//	ResetScroll();
		//	if(outputBuffer.Length > OUTPUT_LENGTH_LIMIT)
		//	{
		//		editingArea.Text=outputBuffer.ToString();
		//		outputBuffer.Clear();
		//	}
		//}

		//protected void AppendString(string text)
		//{
		//	outputBuffer.Append(text);
		//	CommonOutputProcessing();
		//}

		//public void Clear()
		//{
		//	editingArea.Text = "";
		//	outputBuffer.Clear();
		//	CommonOutputProcessing();
		//	needsResize=true;
		//}

		//public void AddText(string str)
		//{
		//	AppendString("\n" + str);
		//}

		//public void AddReturnValue(string str)
		//{
		//	AppendString("\nr> " + str);
		//}

		//public void AddOutput(string str)
		//{
		//	AppendString("\no> " + str);
		//}

		//public void AddError(string str)
		//{
		//	AppendString("\ne> " + str);
		//}

		//public void AddSourceString(string str)
		//{
		//	AppendString("\ni> " + str);
		//}

		//public void AddFileContent(string str)
		//{
		//	AppendString("\nf> " + str);
		//}

		protected override void DecoratorUpdate()
		{
			InterceptMostInput();

			editingArea.Text=uiparts.scriptWindow.currentEngineProcess.outputBuffer.GetString(out bool newOutput);
			if (newOutput)
			{
				ResetScroll();
			}
			//bool hadKeyDownThisUpdate=Event.current.type==EventType.KeyDown;
			//if (outputBuffer.Length>0)
			//{
			//	if (outputBuffer.Length>OUTPUT_LENGTH_LIMIT)
			//	{
			//		editingArea.Text=outputBuffer.ToString();
			//		outputBuffer.Clear();
			//	}

			//	int diff=editingArea.Text.Length+outputBuffer.Length-OUTPUT_LENGTH_LIMIT;
			//	if (diff>=0 && diff<editingArea.Text.Length)
			//	{
			//		editingArea.Text=editingArea.Text.Substring(diff, editingArea.Text.Length-diff)+outputBuffer;
			//	}
			//	else
			//	{
			//		editingArea.Text=editingArea.Text+outputBuffer;
			//	}
			//	outputBuffer.Clear();
			//	SetChildRect();
			//}

			base.DecoratorUpdate();

			//if (hadKeyDownThisUpdate && Event.current.type==EventType.Used)
			//{
			//	if (editingArea.Text.Length > OUTPUT_LENGTH_LIMIT)
			//	{
			//		editingArea.Text = editingArea.Text.Substring(editingArea.Text.Length - OUTPUT_LENGTH_LIMIT, OUTPUT_LENGTH_LIMIT);
			//	}
			//	needsResize=true;
			//}
		}

		void InterceptMostInput()
		{
			if (HasFocus() && Event.current.type == EventType.KeyDown)
			{
				switch (Event.current.keyCode) {
				case KeyCode.Insert:
				case KeyCode.LeftControl:
				case KeyCode.RightControl:
				case KeyCode.C:
					if (!Event.current.control) {
						// Don't give focus for the followup char events.
						if(Event.current.keyCode!=KeyCode.None)
							uiparts.replInputArea.GrabFocus();
					}
					break;

				default:
					// Don't give focus for the followup char events.
					if (Event.current.keyCode != KeyCode.None)
						uiparts.replInputArea.GrabFocus();
					break;
				}
			}
		}
	}
}
using System.Collections.Generic;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.EditingChanges;
using Kerbalui.Util;
using LiveRepl.Completion;
using LiveRepl.Interfaces;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl.Parts
{
	public class ReplInputArea:EditingAreaScroller,ICompletableElement
	{
		public ScriptWindowParts uiparts;
		const float extraBottomSpace=3;

		public ReplInputArea(ScriptWindowParts uiparts):base(new EditingArea(new TextArea()))
		{
			this.uiparts=uiparts;

			uiparts.FontChange+=editingArea.FontChangeEventHandler;
		}

		public EditingState EditingState
		{
			get
			{
				return new EditingState(editingArea.Text, editingArea.CursorIndex, editingArea.SelectIndex);
			}
		}

		protected override void DecoratorUpdate()
		{
			//if (editingArea.TrySetFont(uiparts.fontSelector.CurrentFont))
			//{
			//	uiparts.scriptWindow.needsResize=true;
			//}

			bool receivedInput=false;
			if (HasFocus())
			{
				receivedInput=Event.current.type==EventType.KeyDown;
			}
			base.DecoratorUpdate();

			if (editingArea.ReceivedInput || receivedInput)
			{
				editingArea.ReceivedInput=true;
				uiparts.scriptWindow.needsResize=true;
			}
		}

		public void Complete(int index)
		{
			EditingState newEditingState=CompletionHelper.Complete(index,uiparts,EditingState);
			Text=newEditingState.text; CursorIndex=newEditingState.cursorIndex; SelectIndex=newEditingState.selectionIndex;
		}

		public IList<string> GetCompletionContent(out int replaceStart, out int replaceEnd)
		{
			//return uiparts.scriptWindow.currentReplEvaluator.GetDisplayableCompletions(Text, CursorIndex, out replaceStart, out replaceEnd);
			return uiparts.scriptWindow.CurrentEngine.GetDisplayableCompletions(Text, CursorIndex, out replaceStart, out replaceEnd);
		}

		public override bool HorizontalScrollBarPresent { get => rect.width<editingArea.MinSize.x; }
		public override bool VerticalScrollBarPresent { get => false; }


		bool firstUse=false;
		public override Vector2 MinSize
		{
			get
			{
				float minHeight=editingArea.MinSize.y+extraBottomSpace;
				if (HorizontalScrollBarPresent)
				{
					minHeight+=ScrollbarWidth;
				}
				//Debug.Log("minHeight "+minHeight);
				if (!firstUse)
				{
					uiparts.scriptWindow.needsResize=true;
					firstUse=true;
				}
				return new Vector2(0, minHeight);
			}
		}
	}
}

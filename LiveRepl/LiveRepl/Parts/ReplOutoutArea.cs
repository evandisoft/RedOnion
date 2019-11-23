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
		}

		public const int OUTPUT_LENGTH_LIMIT = 10000;

		void CommonOutputProcessing()
		{
			ResetScroll();
			uiparts.scriptWindow.needsResize=true;
		}

		public void Clear()
		{
			editingArea.Text = "";
			CommonOutputProcessing();
		}

		public void AddText(string str)
		{
			editingArea.Text += "\n" + str;
			CommonOutputProcessing();
		}

		public void AddReturnValue(string str)
		{
			editingArea.Text += "\nr> " + str;
			CommonOutputProcessing();
		}

		public void AddOutput(string str)
		{
			editingArea.Text += "\no> " + str;
			CommonOutputProcessing();
		}

		public void AddError(string str)
		{
			editingArea.Text += "\ne> " + str;
			CommonOutputProcessing();
		}

		public void AddSourceString(string str)
		{
			editingArea.Text += "\ni> " + str;
			CommonOutputProcessing();
		}

		public void AddFileContent(string str)
		{
			editingArea.Text += "\nf> " + str;
			CommonOutputProcessing();
		}

		protected override void DecoratorUpdate()
		{
			if (editingArea.editableText.Style==null)
			{
				editingArea.editableText.Style = new GUIStyle(GUI.skin.textArea)
				{
					alignment = TextAnchor.LowerLeft,
					font = GUILibUtil.GetMonoSpaceFont()
				};
			}

			//if (editingArea.TrySetFont(uiparts.fontSelector.CurrentFont))
			//{
			//	uiparts.scriptWindow.needsResize=true;
			//}

			InterceptMostInput();

			bool hadKeyDownThisUpdate=Event.current.type==EventType.KeyDown;

			base.DecoratorUpdate();

			if (hadKeyDownThisUpdate && Event.current.type==EventType.Used)
			{
				if (editingArea.Text.Length > OUTPUT_LENGTH_LIMIT)
				{
					editingArea.Text = editingArea.Text.Substring(editingArea.Text.Length - OUTPUT_LENGTH_LIMIT, OUTPUT_LENGTH_LIMIT);
				}
				needsResize=true;
			}
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
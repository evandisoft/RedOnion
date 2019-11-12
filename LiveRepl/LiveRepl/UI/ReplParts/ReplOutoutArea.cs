using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.Util;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	public class ReplOutoutArea:EditingAreaScroller
	{
		public Repl repl;

		public ReplOutoutArea(Repl repl) : base(new EditingArea(new TextArea()))
		{
			this.repl=repl;
		}

		public const int OUTPUT_LENGTH_LIMIT = 10000;

		void CommonOutputProcessing()
		{
			ResetScroll();
			repl.needsResize=true;
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
			if (editingArea.editableText.style==null)
			{
				editingArea.editableText.style = new GUIStyle(GUI.skin.textArea)
				{
					alignment = TextAnchor.LowerLeft,
					font = GUILibUtil.GetMonoSpaceFont()
				};
			}

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
							repl.replInputArea.GrabFocus();
					}
					break;

				default:
					// Don't give focus for the followup char events.
					if (Event.current.keyCode != KeyCode.None)
						repl.replInputArea.GrabFocus();
					break;
				}
			}
		}
	}
}
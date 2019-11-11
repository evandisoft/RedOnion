using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.Util;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	public class ReplOutoutArea:ContentScroller
	{
		public Repl repl;

		public ReplOutoutArea(Repl repl) : base(new TextArea())
		{
			this.repl=repl;
		}

		public const int OUTPUT_LENGTH_LIMIT = 10000;

		void CommonOutputProcessing()
		{
			ResetScroll();
		}

		public void AddReturnValue(string str)
		{
			contentControl.content.text += "\nr> " + str;
			CommonOutputProcessing();
		}

		public void AddOutput(string str)
		{
			contentControl.content.text += "\no> " + str;
			CommonOutputProcessing();
		}

		public void AddError(string str)
		{
			contentControl.content.text += "\ne> " + str;
			CommonOutputProcessing();
		}

		public void AddSourceString(string str)
		{
			contentControl.content.text += "\ni> " + str;
			CommonOutputProcessing();
		}

		public void AddFileContent(string str)
		{
			contentControl.content.text += "\nf> " + str;
			CommonOutputProcessing();
		}

		protected override void DecoratorUpdate()
		{
			if (contentControl.style==null)
			{
				contentControl.style = new GUIStyle(GUI.skin.textArea)
				{
					alignment = TextAnchor.LowerLeft,
					font = GUILibUtil.GetMonoSpaceFont()
				};
			}

			bool hadKeyDownThisUpdate=Event.current.type==EventType.KeyDown;

			base.DecoratorUpdate();

			if (hadKeyDownThisUpdate && Event.current.type==EventType.Used)
			{
				if (contentControl.content.text.Length > OUTPUT_LENGTH_LIMIT)
				{
					contentControl.content.text = contentControl.content.text.Substring(contentControl.content.text.Length - OUTPUT_LENGTH_LIMIT, OUTPUT_LENGTH_LIMIT);
				}
				needsResize=true;
			}
		}
	}
}
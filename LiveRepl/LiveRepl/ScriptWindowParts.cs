using System;
using LiveRepl.Decorators;
using LiveRepl.Parts;

namespace LiveRepl
{
	public class ScriptWindowParts
	{
		public ScriptWindowParts(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;
			contentGroup=new ContentGroup(this);
		}

		public ScriptWindow scriptWindow;
		public CenterGroup centerGroup;
		public CompletionArea completionArea;
		public CompletionGroup completionGroup;
		public ContentGroup contentGroup;
		public Editor editor;
		public EditorChangesIndicator editorChangesIndicator;
		public EditorGroup editorGroup;
		public EditorStatusLabel editorStatusLabel;
		public EditorStatusGroup editorStatusGroup;
		public Repl repl;
		public ReplGroup replGroup;
		public ReplInputArea replInputArea;
		public ReplOutoutArea replOutoutArea;
		public ScriptEngineLabel scriptEngineLabel;
		public ScriptEngineSelector scriptEngineSelector;
		public ScriptNameInputArea scriptNameInputArea;
	       internal ScriptDisabledElement scriptDisabledEditorGroup;
        internal ScriptDisabledElement ScriptDisabledCompletionGroup;
    }
}

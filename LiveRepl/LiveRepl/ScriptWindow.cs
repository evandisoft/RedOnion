using Kerbalui.Util;
using LiveRepl.Completion;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow
	{
		public CompletionManager completionManager;
		public bool inputIsLocked;

		public ScriptWindow(string title) : base(title)
		{
			InitLayout();
			InitEvaluation();
			InitCompletion();
			InitializeGlobalKeyBindings();

			LoadEditorText();

			InitFont();
		}

		public void InitFont()
		{
			var defaultFont=GUILibUtil.GetMonoSpaceFont();
			var fontname=SavedSettings.LoadSetting("fontname", "");
			var font=Font.CreateDynamicFontFromOSFont(fontname,14);
			if (font==null)
			{
				font=defaultFont;
			}
			uiparts.ChangeFont(font);
		}

		public void SetOrReleaseInputLock()
		{
			if (GUILibUtil.MouseInRect(uiparts.contentGroup.rect)) //ContentRect.Contains(Event.current.mousePosition))//GUILibUtil.MouseInRect(rect))
			{
				if (!inputIsLocked)
				{
					//Debug.Log("Input is now locked");
					inputIsLocked = true;
					InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
				}
			}
			else
			{
				if (inputIsLocked)
				{
					//Debug.Log("Input is no longer locked");
					inputIsLocked = false;
					InputLockManager.ClearControlLocks();
				}
			}
		}

		void InitCompletion()
		{
			completionManager=new CompletionManager(uiparts.completionArea);
			completionManager.AddCompletable(new EditingAreaCompletionAdapter(uiparts.editor.editingArea, this));
			completionManager.AddCompletable(new EditingAreaCompletionAdapter(uiparts.replInputArea.editingArea, this));
			completionManager.AddCompletable(uiparts.scriptNameInputArea);
			completionManager.AddCompletable(uiparts.scriptNameInputArea);
			completionManager.AddCompletable(uiparts.fontSelector);
		}

		bool hadMouseDownLastUpdate;
		protected override void WindowsUpdate()
		{
			SetOrReleaseInputLock();

			GUILibUtil.ConsumeMarkedCharEvent(Event.current);

			if (ScriptRunning) HandleInputWhenExecuting();

			GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			if (completionManager.Update(hadMouseDownLastUpdate))
			{
				uiparts.completionArea.needsResize=true;
			}
			hadMouseDownLastUpdate=Event.current.type==EventType.MouseDown;
			base.WindowsUpdate();

			if (inputIsLocked && Event.current.type == EventType.ScrollWheel)
			{
				Event.current.Use();
			}
		}

		private void HandleInputWhenExecuting()
		{
			if (Event.current.type == EventType.KeyDown
								&& Event.current.keyCode == KeyCode.C
								&& Event.current.control)
			{
				GUILibUtil.ConsumeAndMarkNextCharEvent(Event.current);
				Terminate();
			}

			EventType t = Event.current.type;
			if (t == EventType.KeyDown)
			{
				Event.current.Use();
			}
		}

		public void OnDestroy()
		{
			SavedSettings.SaveSetting("WindowPositionX", rect.x.ToString());
			SavedSettings.SaveSetting("WindowPositionY", rect.y.ToString());
			SavedSettings.SaveSetting("editorVisible", editorVisible.ToString());
			SavedSettings.SaveSetting("replVisible", replVisible.ToString());
			SavedSettings.SaveSetting("lastQueueTag", uiparts.queueTagInputArea.Text);
			SavedSettings.SaveToDisk();
		}
	}
}

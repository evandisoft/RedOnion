using System;
using System.Collections.Generic;
using Kerbalua.Other;
using Kerbalui.Controls;
using Kerbalui.Types;
using Kerbalui.Util;
using LiveRepl.Misc;
using LiveRepl.UI;
using LiveRepl.UI.ReplParts;
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

			contentGroup.editorGroup.LoadEditorText();
		}


		public void SetOrReleaseInputLock()
		{
			if (ContentRect.Contains(Event.current.mousePosition))//GUILibUtil.MouseInRect(rect))
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
			completionManager=new CompletionManager(contentGroup.completionGroup.completionArea);
			completionManager.AddCompletable(new EditingAreaCompletionAdapter(contentGroup.editorGroup.editor.editingArea, this));
			completionManager.AddCompletable(new EditingAreaCompletionAdapter(contentGroup.replGroup.repl.replInputArea.editingArea, this));
			completionManager.AddCompletable(contentGroup.editorGroup.fileIOGroup.scriptNameInputArea);
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
				contentGroup.completionGroup.completionArea.needsResize=true;
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
		}
	}
}

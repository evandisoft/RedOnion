using Kerbalui;
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
			//Debug.Log("UI scale is "+GameSettings.UI_SCALE);
			InitLayout();
			InitEvaluation();
			InitCompletion();
			InitializeGlobalKeyBindings();

			LoadEditorText();

			InitFont();
		}

		public void InitFont()
		{
			var defaultFontName=GUILibUtil.GetMonoSpaceFontName();
			var fontname=SavedSettings.LoadSetting("fontname", defaultFontName);

			uiparts.ChangeFont(fontname,KerbaluiSettings.DefaultFontsize);
		}

		public void SetOrReleaseInputLock()
		{
			if (GUILibUtil.MouseInRect(uiparts.contentGroup.rect)) //ContentRect.Contains(Event.current.mousePosition))//GUILibUtil.MouseInRect(rect))
			{
				if (!inputIsLocked)
				{
					//Debug.Log("Input is now locked");
					inputIsLocked = true;
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS, "kerbalua");
				}
			}
			else
			{
				if (inputIsLocked)
				{
					//Debug.Log("Input is no longer locked");
					inputIsLocked = false;
					InputLockManager.RemoveControlLock("kerbalua");
				}
			}
		}

		void InitCompletion()
		{
			completionManager=new CompletionManager(uiparts.completionArea);
			completionManager.AddCompletable(uiparts.editor);
			completionManager.AddCompletable(uiparts.replInputArea);
			completionManager.AddCompletable(uiparts.scriptNameInputArea);
			completionManager.AddCompletable(uiparts.scriptNameInputArea);
			completionManager.AddCompletable(uiparts.fontSelector);
		}

		bool hadMouseDownLastUpdate;
		protected override void WindowsUpdate()
		{
			SetOrReleaseInputLock();
			//if (inputIsLocked)
			//{
			//	GUI.FocusWindow(windowID);
			//}
			//else
			//{
			//	GUI.UnfocusWindow();
			//}

			if (ScriptRunning ) HandleInputWhenExecuting();
			GUILibUtil.ConsumeMarkedCharEvent(Event.current);
			//if (inputIsLocked)
			//{
			//	//GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			//}
			GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			if (completionManager.Update(hadMouseDownLastUpdate))
			{
				uiparts.completionArea.needsResize=true;
			}
			hadMouseDownLastUpdate=Event.current.type==EventType.MouseDown && rect.Contains(Event.current.mousePosition);

			base.WindowsUpdate();

			//if (inputIsLocked && Event.current.type == EventType.ScrollWheel)
			//{
			//	Event.current.Use();
			//}
			//if (Event.current.type == EventType.ScrollWheel && content.rect.Contains(Event.current.mousePosition))
			//{
			//	Event.current.Use();
			//}
			//if (Event.current.isKey || Event.current.isMouse || Event.current.isScrollWheel)
			//{

			//}
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
#if DEBUG
			SavedSettings.SaveSetting("lastQueueTag", uiparts.queueTagInputArea.Text);
#endif
			SavedSettings.SaveToDisk();
		}
	}
}

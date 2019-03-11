using UnityEngine;
using Kerbalua.Other;
using Kerbalua.Completion;
using System.Collections.Generic;
using System;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.Script;

namespace Kerbalua.Gui {
	public class ScriptWindow {
		const int maxOutputBytes = 80000;
		public string Title = "Live Code";

		public Editor editor = new Editor();
		public Repl repl = new Repl();

		public CompletionBox completionBox = new CompletionBox();
		public AutoLayoutBox widgetBar = new AutoLayoutBox();
		public RecentFilesList recentFiles;
		public ReplEvaluator currentReplEvaluator;
		public Dictionary<string,ReplEvaluator> replEvaluators = new Dictionary<string,ReplEvaluator>();

		const int windowID = 0;
		const int modalID = 1;
		Rect mainWindowRect;
		bool editorVisible = true;
		bool replVisible = true;

		Rect widgetBarRect;
		Rect replRect;
		Rect completionBoxRect;
		Rect editorRect;
		ScriptNameInputArea scriptIOTextArea=new ScriptNameInputArea();
		// Should be a label but I haven't made a label yet.
		TextArea replEvaluatorLabel = new TextArea();

		public string saveLoadFilename = "untitled.b";

		const float titleHeight = 20;

		bool inputIsLocked;

		public KeyBindings GlobalKeyBindings = new KeyBindings();

		/// <summary>
		/// This manages completion. Keeps track of what is was focused and
		/// manages the completion interaction between an control that can
		/// use completion, and the completion 
		/// </summary>
		CompletionManager completionManager;

		public void SetCurrentEvaluator(string evaluatorName)
		{
			currentReplEvaluator = replEvaluators[evaluatorName];
			replEvaluatorLabel.content.text = evaluatorName;
		}

		public ScriptWindow(Rect param_mainWindowRect)
		{
			replEvaluators["RedOnion"] = new RedOnionReplEvaluator();
			replEvaluators["MoonSharp"] = new MoonSharpReplEvaluator(CoreModules.Preset_Complete);
			SetCurrentEvaluator("RedOnion");
			recentFiles = new RecentFilesList((string filename) => {
				scriptIOTextArea.content.text = filename;
				editor.content.text = scriptIOTextArea.Load();
			});

			mainWindowRect = param_mainWindowRect;
			completionManager = new CompletionManager(completionBox);
			completionManager.AddCompletable(scriptIOTextArea);
			completionManager.AddCompletable(new EditingAreaCompletionAdapter(editor, this));
			completionManager.AddCompletable(new EditingAreaCompletionAdapter(repl.inputBox, this));

			scriptIOTextArea.content.text = saveLoadFilename;
			editor.content.text=scriptIOTextArea.Load();

			editorRect = new Rect(
				0,
				titleHeight,
				500,
				param_mainWindowRect.height - titleHeight
				);
			widgetBarRect = new Rect(
				0,
				titleHeight,
				100,
				(param_mainWindowRect.height - titleHeight)/2
				);
			replRect = new Rect(
				0,
				titleHeight,
				400,
				param_mainWindowRect.height - titleHeight
				);
			completionBoxRect = new Rect(
				0,
				titleHeight,
				150,
				param_mainWindowRect.height - titleHeight
				);
			widgetBarRect.x = editorRect.width;
			replRect.x = widgetBarRect.x + widgetBarRect.width;
			completionBoxRect.x = replRect.x + replRect.width;
			mainWindowRect.width = widgetBarRect.width + replRect.width + editorRect.width + completionBoxRect.width;

			widgetBar.renderables.Add(new Button("<<", () => editorVisible = !editorVisible));
			widgetBar.renderables.Add(new Button(">>", () => replVisible = !replVisible));
			widgetBar.renderables.Add(scriptIOTextArea);
			widgetBar.renderables.Add(new Button("Save", () => {
				scriptIOTextArea.Save(editor.content.text);
			}));
			widgetBar.renderables.Add(new Button("Load", () => {
				editor.content.text=scriptIOTextArea.Load();
			}));
			widgetBar.renderables.Add(new Button("Evaluate", () => {
				repl.outputBox.content.text+=currentReplEvaluator.Evaluate(editor.content.text);
			}));
			widgetBar.renderables.Add(new Button("Reset Engine", () => {
				currentReplEvaluator.ResetEngine();
			}));

			foreach(var evaluatorName in replEvaluators.Keys) {
				widgetBar.renderables.Add(new Button(evaluatorName, () => {
					SetCurrentEvaluator(evaluatorName);
				}));
			}
			widgetBar.renderables.Add(replEvaluatorLabel);

			InitializeKeyBindings();
		}

		void InitializeKeyBindings()
		{
			GlobalKeyBindings.Add(new EventKey(KeyCode.U, true), () => editor.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.I, true), () => scriptIOTextArea.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.O, true), () => repl.inputBox.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.P, true), () => completionBox.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.D, true), () => {
				editor.content.text = scriptIOTextArea.Load();
			});
			GlobalKeyBindings.Add(new EventKey(KeyCode.S, true), () => {
				scriptIOTextArea.Save(editor.content.text);
			});
			GlobalKeyBindings.Add(new EventKey(KeyCode.Space, false, true), completionManager.Complete);
			GlobalKeyBindings.Add(new EventKey(KeyCode.Return, true), completionManager.Complete);

			editor.KeyBindings.Add(new EventKey(KeyCode.E, true), () => {
				repl.outputBox.content.text += currentReplEvaluator.Evaluate(editor.content.text);
			});
			repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.E, true), () => {
				repl.outputBox.content.text += currentReplEvaluator.Evaluate(repl.inputBox.content.text);
			});
			repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.Return), () => {
				repl.outputBox.content.text += currentReplEvaluator.Evaluate(repl.inputBox.content.text);
				repl.inputBox.content.text = "";
				completionBox.content.text = "";
			});
			// For some reason having this event occur inside completionBox update
			// makes it so that the target for completion has its cursorIndex reset to 0

		}

		/// <summary>
		/// This updates Rects for boxes that are not inside the main window 
		/// that I want near the main window, so that when the main window is 
		/// moved these boxes stay in the specified position relative to the 
		/// window.
		/// </summary>
		/// <param name="rect">The Rect to be updated.</param>
		/// <param name="xOffset">The desired xOffset from the main window.</param>
		/// <param name="yOffset">The desired yOffset from the main window.</param>
		public Rect UpdateBoxPositionWithWindow(Rect rect,float xOffset,float yOffset=0)
		{
			rect.x = mainWindowRect.x + xOffset;
			rect.y = mainWindowRect.y + yOffset;
			return rect;
		}

		/// <summary>
		/// Returns the rectangle covering the entire area of the editor/repl/completion
		/// </summary>
		public Rect GetTotalArea()
		{
			return GetCurrentWindowRect();
		}

		public void SetOrReleaseInputLock()
		{
			if (GetTotalArea().Contains(Mouse.screenPos)) {
				if (!inputIsLocked) {
					inputIsLocked = true;
					InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
				}
			} else {
				if (inputIsLocked) {
					inputIsLocked = false;
					InputLockManager.ClearControlLocks();
				}
			}
		}

		public void Update()
		{
			SetOrReleaseInputLock();
			completionManager.Update();

			//if (replVisible) {
			//	mainWindowRect.width = buttonBarRect.width + replRect.width;
			//} else {
			//	mainWindowRect.width = buttonBarRect.width;
			//}

			Rect effectiveWindowRect = GetCurrentWindowRect();
			Rect modifiedEffectiveRect = GUI.Window(windowID, effectiveWindowRect, MainWindow, Title);
			mainWindowRect.x += modifiedEffectiveRect.x - effectiveWindowRect.x;
			mainWindowRect.y += modifiedEffectiveRect.y - effectiveWindowRect.y;
			//GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current);
		}

		Rect GetCurrentWindowRect()
		{
			Rect currentWindowRect = new Rect(mainWindowRect);
			if (!editorVisible) {
				currentWindowRect.x += editorRect.width;
				currentWindowRect.width -= editorRect.width;
			}
			if (!replVisible) {
				currentWindowRect.width -= completionBoxRect.width + replRect.width;
			}
			return currentWindowRect;
		}

		Rect GetCurrentWidgetBarRect()
		{
			Rect currentWidgetRect = new Rect(widgetBarRect);
			if (!editorVisible) {
				currentWidgetRect.x -= editorRect.width;
			}
			return currentWidgetRect;
		}

		Rect GetCurrentReplRect()
		{
			Rect currentReplRect = new Rect(replRect);
			if (!editorVisible) {
				currentReplRect.x -= editorRect.width;
			}
			return currentReplRect;
		}

		Rect GetCurrentCompletionBoxRect()
		{
			Rect currentCompletionBoxRect = new Rect(completionBoxRect);
			if (!editorVisible) {
				currentCompletionBoxRect.x -= editorRect.width;
			}
			return currentCompletionBoxRect;
		}

		/// <summary>
		/// To make input handling simpler, only the repl and button bar are in
		/// the main window. The other boxes are updated to always be beside this window.
		/// 
		/// This design decision may be reviewed in the future with a better understanding
		/// of IMGUI.
		/// </summary>
		/// <param name="id">Identifier.</param>
		void MainWindow(int id)
		{
			Rect effectiveWindowRect = GetCurrentWindowRect();
			GUI.DragWindow(new Rect(0, 0, effectiveWindowRect.width, titleHeight));
			GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current);

			Rect currentWidgetBarRect = GetCurrentWidgetBarRect();
			widgetBar.Update(currentWidgetBarRect);
			currentWidgetBarRect.y += widgetBarRect.height;
			recentFiles.Update(currentWidgetBarRect);

			if (replVisible) {
				if(repl.outputBox.HasFocus()) {
					repl.inputBox.GrabFocus();
				}

				repl.Update(GetCurrentReplRect());
			}

			if (editorVisible) {
				//editorRect = UpdateBoxPositionWithWindow(editorRect, -editorRect.width);
				editor.Update(editorRect);
			}

			// Lots of hacks here. I will eventually better understand how this
			// works and replace it.
			if (replVisible) {
				bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown;
				string lastControlname = GUI.GetNameOfFocusedControl();
				//completionBoxRect = UpdateBoxPositionWithWindow(completionBoxRect, mainWindowRect.width);

				completionBox.Update(GetCurrentCompletionBoxRect());
				if (lastEventWasMouseDown && Event.current.type == EventType.Used) {
					//Debug.Log("trying to complete");
					Rect rectMinusScrollBar = new Rect(completionBoxRect) {
						width = completionBoxRect.width - 20,
						height = completionBoxRect.height - 20
					};
					if (GUIUtil.MouseInRect(rectMinusScrollBar)) {
						completionManager.Complete();
						//GUI.FocusControl(lastControlname);
						completionManager.DisplayCurrentCompletions();
					}
				}
			}

		}
	}
}

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
		public ReplEvaluator currentReplEvaluator;
		public Dictionary<string,ReplEvaluator> replEvaluators = new Dictionary<string,ReplEvaluator>();

		const int windowID = 0;
		const int modalID = 1;
		Rect mainWindowRect;
		bool editorVisible = true;
		bool replVisible = true;

		Rect buttonBarRect;
		Rect replRect;
		Rect completionBoxRect;
		Rect editorRect;
		TextArea scriptNameInput=new TextArea();
		// Should be a label but I haven't made a label yet.
		TextArea replEvaluatorLabel = new TextArea();

		Rect SaveLoadRect;

		public string saveLoadFilename = "untitled.b";

		const float titleHeight = 20;

		bool inputIsLocked;

		string baseFolderPath = "scripts";

		bool editorChanged;
		bool inputBoxChanged;

		const string defaultScriptFilename = "untitled.b";

		KeyBindings KeyBindings = new KeyBindings();

		string CreateFullPath(string scriptName)
		{
			if (scriptName == "") {
				scriptName = defaultScriptFilename;
				scriptNameInput.content.text = scriptName;
				saveLoadFilename = defaultScriptFilename;
			}

			string fullPath=baseFolderPath + "/" + scriptName;

			if (!File.Exists(fullPath)) {
				File.WriteAllText(fullPath, "");
			}

			return fullPath;
		}

		void SaveScript(string scriptName)
		{
			Directory.CreateDirectory(baseFolderPath);
			File.WriteAllText(CreateFullPath(scriptName), editor.content.text);
			saveLoadFilename = scriptName;
		}

		void LoadScript(string scriptName)
		{
			Directory.CreateDirectory(baseFolderPath);
			editor.content.text=File.ReadAllText(CreateFullPath(scriptName));
			saveLoadFilename = scriptName;
		}

		public void SetCurrentEvaluator(string evaluatorName)
		{
			currentReplEvaluator = replEvaluators[evaluatorName];
			replEvaluatorLabel.content.text = evaluatorName;
		}

		public ScriptWindow(Rect mainWindowRect)
		{
			replEvaluators["RedOnion"] = new RedOnionReplEvaluator(new Engine());
			replEvaluators["MoonSharp"] = new MoonSharpReplEvaluator(CoreModules.Preset_Complete);
			SetCurrentEvaluator("RedOnion");

			this.mainWindowRect = mainWindowRect;

			scriptNameInput.content.text = saveLoadFilename;
			LoadScript(scriptNameInput.content.text);

			buttonBarRect = new Rect(0, titleHeight, 100, mainWindowRect.height-titleHeight);
			replRect = new Rect(buttonBarRect.width, titleHeight, mainWindowRect.width - buttonBarRect.width, mainWindowRect.height-titleHeight);
			editorRect = new Rect(0, 0, replRect.width, mainWindowRect.height);
			completionBoxRect = new Rect(0, 0, 150, mainWindowRect.height);

			widgetBar.renderables.Add(new Button("<<", () => editorVisible = !editorVisible));
			widgetBar.renderables.Add(new Button(">>", () => replVisible = !replVisible));
			widgetBar.renderables.Add(scriptNameInput);
			widgetBar.renderables.Add(new Button("Save", () => {
				SaveScript(scriptNameInput.content.text);
			}));
			widgetBar.renderables.Add(new Button("Load", () => {
				LoadScript(scriptNameInput.content.text);
			}));
			widgetBar.renderables.Add(new Button("Evaluate", () => {
				repl.outputBox.content.text+=currentReplEvaluator.Evaluate(editor.content.text);
			}));

			foreach(var evaluatorName in replEvaluators.Keys) {
				widgetBar.renderables.Add(new Button(evaluatorName, () => {
					SetCurrentEvaluator(evaluatorName);
				}));
			}
			widgetBar.renderables.Add(replEvaluatorLabel);

			KeyBindings.Add(new EventKey(KeyCode.U, true), () => editor.GrabFocus());
			KeyBindings.Add(new EventKey(KeyCode.I, true), () => scriptNameInput.GrabFocus());
			KeyBindings.Add(new EventKey(KeyCode.O, true), () => repl.inputBox.GrabFocus());
			KeyBindings.Add(new EventKey(KeyCode.P, true), () => completionBox.GrabFocus());

			InitializeEditorAndReplKeyBindings();
		}

		void InitializeEditorAndReplKeyBindings()
		{
			editor.KeyBindings.Add(new EventKey(KeyCode.Space, false, true), () => {
				//AllCompletion.Complete(replEvaluator.Globals, editor, completionBox.content, true);
			});
			editor.KeyBindings.Add(new EventKey(KeyCode.E, true), () => {
				repl.outputBox.content.text += currentReplEvaluator.Evaluate(editor.content.text);
			});
			editor.KeyBindings.Add(new EventKey(KeyCode.D, true), () => {
				LoadScript(scriptNameInput.content.text);
			});
			editor.KeyBindings.Add(new EventKey(KeyCode.S, true), () => {
				SaveScript(scriptNameInput.content.text);
			});
			repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.Space, false, true), () => {
				//AllCompletion.Complete(replEvaluator.Globals, editor, completionBox.content, true);
			});
			repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.E, true), () => {
				repl.outputBox.content.text += currentReplEvaluator.Evaluate(repl.inputBox.content.text);
			});
			repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.Return), () => {
				repl.outputBox.content.text += currentReplEvaluator.Evaluate(repl.inputBox.content.text);
				repl.inputBox.content.text = "";
				completionBox.content.text = "";
			});
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
			Rect totalArea = new Rect();
			totalArea.x = mainWindowRect.x;
			totalArea.y = mainWindowRect.y;
			totalArea.height = mainWindowRect.height;
			totalArea.width = mainWindowRect.width;

			if (editorVisible) {
				totalArea.width += editorRect.width;
				totalArea.x -= editorRect.width;
			}
			if (replVisible) {
				totalArea.width += completionBoxRect.width;
			}

			return totalArea;
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

		public void Render()
		{
			SetOrReleaseInputLock();

			if (replVisible) {
				mainWindowRect.width = buttonBarRect.width + replRect.width;
			} else {
				mainWindowRect.width = buttonBarRect.width;
			}


			mainWindowRect = GUI.Window(windowID, mainWindowRect, MainWindow, Title);
			KeyBindings.ExecuteAndConsumeIfMatched(Event.current);

			if (editorVisible) {

				editorRect =UpdateBoxPositionWithWindow(editorRect, -editorRect.width);
				editor.Render(editorRect);

				if (editorChanged) {
					//AllCompletion.Complete(replEvaluator.Globals, editor, completionBox.content, false);
					editorChanged = false;
				}
			}

			if (replVisible) {
				completionBoxRect=UpdateBoxPositionWithWindow(completionBoxRect, mainWindowRect.width);
				completionBox.Render(completionBoxRect);
			}


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
			GUI.DragWindow(new Rect(0, 0, mainWindowRect.width, titleHeight));
			KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			widgetBar.Render(buttonBarRect);
			
			if (replVisible) {
				if(repl.outputBox.HasFocus()) {
					repl.inputBox.GrabFocus();
				}

				repl.Render(replRect);

				if (inputBoxChanged) {
					//AllCompletion.Complete(replEvaluator.Globals, repl.inputBox, completionBox.content, false);
					inputBoxChanged = false;
				}
			}
		}
	}
}

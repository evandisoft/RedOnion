using UnityEngine;
using Kerbalua.Other;
using System.Collections.Generic;
using System;
using System.IO;
using RedOnion.KSP.Autopilot;
using RedOnion.KSP.Settings;
using RedOnion.KSP.API;
using Kerbalui.Gui;
using LiveRepl.Other;
using LiveRepl.UI.Layout;
using LiveRepl.UI;

namespace LiveRepl.Main
{
	public partial class ReplMain
	{
		const int maxOutputBytes = 80000;
		public const string title="Live REPL";

		public Editor editor = new Editor();
		public Repl repl = new Repl();

		public CompletionBox completionBox = new CompletionBox();
		public AutoLayoutBox widgetBar = new AutoLayoutBox();
		public AutoLayoutBox editorControlBox = new AutoLayoutBox();

		public void Update()
		{

			scriptWindow.Update();
		}

		public ScriptWindow scriptWindow=new ScriptWindow(title);

		public ReplMain()
		{

		}

		public RecentFilesList recentFiles;


		const int modalID = 1;
		Rect mainWindowRect;


		Rect widgetBarRect;
		Rect replRect;
		Rect completionBoxRect;
		Rect editorRect;
		//Rect scriptNameRect;
		string testReplContent="";
		ScriptNameInputArea scriptIOTextArea;
		// Should be a label but I haven't made a label yet.
		TextArea replEvaluatorLabel = new TextArea();

		public string saveLoadFilename = "untitled.b";




		bool inputIsLocked;
		//bool evaluationNotFinished = false;

		

		internal void OnDestroy()
		{
			SavedSettings.SaveSetting("WindowPositionX", mainWindowRect.x.ToString());
			SavedSettings.SaveSetting("WindowPositionY", mainWindowRect.y.ToString());
			//Debug.Log("On Destroy was called");
		}

		/// <summary>
		/// This manages completion. Keeps track of what is was focused and
		/// manages the completion interaction between an control that can
		/// use completion, and the completion 
		/// </summary>
		CompletionManager completionManager;


		//public ScriptWindow(Rect param_mainWindowRect)
		//{
		//	InitLayout();

		//	return;

		//	scriptIOTextArea = new ScriptNameInputArea(editor);
		//	editorVisible = bool.Parse(SavedSettings.LoadSetting("editorVisible", "true"));

		//	replEvaluators["ROS Engine"] = new RedOnionReplEvaluator()
		//	{
		//		PrintAction = (str) =>
		//			repl.outputBox.AddOutput(str),
		//		PrintErrorAction = (str) =>
		//			repl.outputBox.AddError(str)
		//	};
		//	RunRosStartupScripts();
		//	replEvaluators["Lua Engine"] = new MoonSharpReplEvaluator()
		//	{
		//		PrintAction = (str) =>
		//			repl.outputBox.AddOutput(str),
		//		PrintErrorAction = (str) =>
		//			repl.outputBox.AddError(str)
		//	};
		//	RunLuaStartupScripts();

		//	string lastEngineName = SavedSettings.LoadSetting("lastEngine", "Lua Engine");
		//	if (replEvaluators.ContainsKey(lastEngineName))
		//	{
		//		currentReplEvaluator = replEvaluators[lastEngineName];
		//		replEvaluatorLabel.content.text = lastEngineName;
		//	}
		//	else
		//	{
		//		foreach (var evaluatorName in replEvaluators.Keys)
		//		{
		//			currentReplEvaluator = replEvaluators[evaluatorName];
		//			replEvaluatorLabel.content.text = evaluatorName;
		//			SavedSettings.SaveSetting("lastEngine", evaluatorName);
		//			break;
		//		}
		//	}

		//	recentFiles = new RecentFilesList((string filename) =>
		//	{
		//		scriptIOTextArea.content.text = filename;
		//		editor.content.text = scriptIOTextArea.Load();
		//	});

		//	mainWindowRect = param_mainWindowRect;
		//	mainWindowRect.x = float.Parse(SavedSettings.LoadSetting("WindowPositionX", param_mainWindowRect.x.ToString()));
		//	mainWindowRect.y = float.Parse(SavedSettings.LoadSetting("WindowPositionY", param_mainWindowRect.y.ToString()));
		//	completionManager = new CompletionManager(completionBox);
		//	completionManager.AddCompletable(scriptIOTextArea);
		//	completionManager.AddCompletable(new EditingAreaCompletionAdapter(editor, this));
		//	completionManager.AddCompletable(new EditingAreaCompletionAdapter(repl.inputBox, this));

		//	editor.content.text = scriptIOTextArea.Load();

		//	editorRect = new Rect(
		//		0,
		//		titleHeight,
		//		500,
		//		param_mainWindowRect.height - titleHeight
		//		);
		//	widgetBarRect = new Rect(
		//		0,
		//		titleHeight,
		//		100,
		//		(param_mainWindowRect.height - titleHeight) / 2
		//		);
		//	replRect = new Rect(
		//		0,
		//		titleHeight,
		//		400,
		//		param_mainWindowRect.height - titleHeight
		//		);
		//	completionBoxRect = new Rect(
		//		0,
		//		titleHeight,
		//		150,
		//		param_mainWindowRect.height - titleHeight
		//		);

		//	widgetBarRect.x = editorRect.width;
		//	replRect.x = widgetBarRect.x + widgetBarRect.width;
		//	completionBoxRect.x = replRect.x + replRect.width;
		//	mainWindowRect.width = widgetBarRect.width + replRect.width + editorRect.width + completionBoxRect.width;

		//	widgetBar.renderables.Add(new Button("<<", () => {
		//		editorVisible = !editorVisible;
		//		SavedSettings.SaveSetting("editorVisible", editorVisible.ToString());
		//		}));
		//	widgetBar.renderables.Add(new Button(">>", () => {
		//		replVisible = !replVisible;
		//		SavedSettings.SaveSetting("editorVisible", editorVisible.ToString());
		//	}));
		//	//widgetBar.renderables.Add(scriptIOTextArea);
		//	widgetBar.renderables.Add(new Button("Save", () =>
		//	{
		//		scriptIOTextArea.Save(editor.content.text);
		//	}));
		//	widgetBar.renderables.Add(new Button("Load", () =>
		//	{
		//		editor.content.text = scriptIOTextArea.Load();
		//	}));
		//	widgetBar.renderables.Add(new Button("Run Script", () =>
		//	{
		//		scriptIOTextArea.Save(editor.content.text);
		//		repl.outputBox.AddFileContent(scriptIOTextArea.content.text);
		//		evaluationList.Add(new Evaluation(editor.content.text, scriptIOTextArea.content.text, currentReplEvaluator));
		//	}));
		//	widgetBar.renderables.Add(new Button("Reset Engine", () =>
		//	{
		//		currentReplEvaluator.ResetEngine();
		//		foreach(var evaluatorname in replEvaluators.Keys)
		//		{
		//			if (replEvaluators[evaluatorname] == currentReplEvaluator)
		//			{
		//				RunStartupScripts(evaluatorname);
		//			}
		//		}
		//	}));
		//	widgetBar.renderables.Add(new Button("Show Hotkeys", () =>
		//	{
		//		PrintKeyBindingsInOutputArea();
		//	}));

		//	foreach (var evaluatorName in replEvaluators.Keys)
		//	{
		//		widgetBar.renderables.Add(new Button(evaluatorName, () =>
		//		{
		//			SetCurrentEvaluator(evaluatorName);
		//			SavedSettings.SaveSetting("lastEngine", evaluatorName);
		//		}));
		//	}
		//	widgetBar.renderables.Add(replEvaluatorLabel);
		//	widgetBar.renderables.Add(new Button("Kill Ctrl", () => {
		//		FlightControl.GetInstance().Shutdown();
		//		Ship.DisableAutopilot();
		//	}));

		//	InitializeKeyBindings();
		//}

		/// <summary>
		/// This updates Rects for boxes that are not inside the main window 
		/// that I want near the main window, so that when the main window is 
		/// moved these boxes stay in the specified position relative to the 
		/// window.
		/// </summary>
		/// <param name="rect">The Rect to be updated.</param>
		/// <param name="xOffset">The desired xOffset from the main window.</param>
		/// <param name="yOffset">The desired yOffset from the main window.</param>
		public Rect UpdateBoxPositionWithWindow(Rect rect, float xOffset, float yOffset = 0)
		{
			rect.x = mainWindowRect.x + xOffset;
			rect.y = mainWindowRect.y + yOffset;
			return rect;
		}

		/// <summary>
		/// Returns the rectangle covering the entire area of the editor/repl/completion
		/// </summary>
		//public Rect GetTotalArea()
		//{
		//	return GetCurrentWindowRect();
		//}



		//bool hadMouseDownLastUpdate = false;
		//private Rect rect;



		//public void Update()
		//{
		//	if (Event.current.type==EventType.KeyDown)
		//	{
		//		Debug.Log(Event.current);
		//	}
		//	//GUI.FocusControl("Control-0");
		//	//UnityEngine.Debug.Log("blah");
		//	SetOrReleaseInputLock();

		//	completionManager.Update(hadMouseDownLastUpdate);
		//	hadMouseDownLastUpdate = false;


		//	//if (replVisible) {
		//	//	mainWindowRect.width = buttonBarRect.width + replRect.width;
		//	//} else {
		//	//	mainWindowRect.width = buttonBarRect.width;
		//	//}

		//	Rect effectiveWindowRect = GetCurrentWindowRect();
		//	Rect modifiedEffectiveRect = GUI.Window(windowID, effectiveWindowRect, MainWindow, Title);
		//	mainWindowRect.x += modifiedEffectiveRect.x - effectiveWindowRect.x;
		//	mainWindowRect.y += modifiedEffectiveRect.y - effectiveWindowRect.y;
		//	//GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current)

		//	foreach (var engineName in replEvaluators.Keys)
		//	{
		//		var repl = replEvaluators[engineName];
		//		try
		//		{
		//			repl.Update();
		//		}
		//		catch (Exception ex)
		//		{
		//			Debug.Log("Exception in REPL.Update: " + ex.Message);
		//			repl.ResetEngine();
		//			RunStartupScripts(engineName);
		//		}
		//	}
		//}



		/// <summary>
		/// To make input handling simpler, only the repl and button bar are in
		/// the main window. The other boxes are updated to always be beside this window.
		/// 
		/// This design decision may be reviewed in the future with a better understanding
		/// of IMGUI.
		/// </summary>
		/// <param name="id">Identifier.</param>
		//void MainWindow(int id)
		//{
		//	Rect effectiveWindowRect = GetCurrentWindowRect();
		//	GUI.DragWindow(new Rect(0, 0, effectiveWindowRect.width, titleHeight));

		//	if (evaluationList.Count!=0)
		//	{
		//		HandleInputWhenExecuting();
		//	}


		//	if (Event.current.type == EventType.MouseDown)
		//	{
		//		hadMouseDownLastUpdate = true;
		//	}


		//	GlobalKeyBindings.ExecuteAndConsumeIfMatched(Event.current);
		//	Rect currentWidgetBarRect = GetCurrentWidgetBarRect();
		//	widgetBar.Update(currentWidgetBarRect);
		//	GUILayout.BeginArea(new Rect(
		//		currentWidgetBarRect.x,
		//		currentWidgetBarRect.height,
		//		currentWidgetBarRect.width,
		//		25
		//		));
		//	{
		//		GUILayout.BeginHorizontal();
		//		{
		//			GUILayout.Label("Tabs");
		//			if (GUILayout.Button("+"))
		//			{
		//				List<string> recentFilesList = new List<string>(SavedSettings.LoadListSetting("recentFiles"));
		//				if (!recentFilesList.Contains(scriptIOTextArea.content.text))
		//				{
		//					recentFilesList.Add(scriptIOTextArea.content.text);
		//				}
		//				recentFilesList.RemoveAll((string filename) => !File.Exists(Path.Combine(SavedSettings.BaseScriptsPath, filename)));
		//				recentFilesList.Sort((string s1, string s2) =>
		//				{
		//					var t1 = Directory.GetLastWriteTime(Path.Combine(SavedSettings.BaseScriptsPath, s1));
		//					var t2 = Directory.GetLastWriteTime(Path.Combine(SavedSettings.BaseScriptsPath, s2));
		//					if (t1 < t2) return 1;
		//					if (t1 > t2) return -1;
		//					return 0;
		//				});
		//				if (recentFilesList.Count > 10)
		//				{
		//					recentFilesList.RemoveAt(recentFilesList.Count - 1);
		//				}
		//				SavedSettings.SaveListSetting("recentFiles", recentFilesList);
		//			}
		//			if (GUILayout.Button("-"))
		//			{
		//				List<string> recentFilesList = new List<string>(SavedSettings.LoadListSetting("recentFiles"));
		//				if (!recentFilesList.Contains(scriptIOTextArea.content.text))
		//				{
		//					recentFilesList.Add(scriptIOTextArea.content.text);
		//				}
		//				recentFilesList.RemoveAll((string filename) => !File.Exists(Path.Combine(SavedSettings.BaseScriptsPath, filename)));
		//				recentFilesList.Remove(scriptIOTextArea.content.text);
		//				if (recentFilesList.Count > 10)
		//				{
		//					recentFilesList.RemoveAt(recentFilesList.Count - 1);
		//				}
		//				SavedSettings.SaveListSetting("recentFiles", recentFilesList);
		//			}
		//		}
		//		GUILayout.EndHorizontal();
		//	}
		//	GUILayout.EndArea();
		//	currentWidgetBarRect.y += widgetBarRect.height + 20;
		//	currentWidgetBarRect.height -= 20;
		//	recentFiles.Update(currentWidgetBarRect);

		//	//testReplContent=GUI.TextArea(GetCurrentReplRect(),testReplContent);
		//	if (replVisible)
		//	{
		//		//if(repl.outputBox.HasFocus()) {
		//		//	repl.inputBox.GrabFocus();
		//		//}
		//		repl.Update(GetCurrentReplRect(), replVisible);
		//	}

		//	if (editorVisible)
		//	{
		//		//editorRect = UpdateBoxPositionWithWindow(editorRect, -editorRect.width);
		//		var scriptIORect = GetCurrentScriptNameRect();
		//		scriptIORect.width /= 2;
		//		scriptIOTextArea.Update(scriptIORect, true);
		//		var editorInfoRect = new Rect(scriptIORect);
		//		editorInfoRect.x = scriptIORect.x + scriptIORect.width;
		//		GUI.Label(editorInfoRect, "Line: " + editor.LineNumber+", Column: "+editor.ColumnNumber);
		//		editor.Update(GetCurrentEditorRect(), editorVisible);
		//	}

		//	// Lots of hacks here. I will eventually better understand how this
		//	// works and replace it.
		//	if (replVisible || editorVisible)
		//	{
		//		bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown;
		//		string lastControlname = GUI.GetNameOfFocusedControl();
		//		//completionBoxRect = UpdateBoxPositionWithWindow(completionBoxRect, mainWindowRect.width);
		//		//if (inputIsLocked && Event.current.type == EventType.ScrollWheel)
		//		//{
		//		//	Event.current.Use();
		//		//}
		//		completionBox.Update(GetCurrentCompletionBoxRect());
		//		if (lastEventWasMouseDown && Event.current.type == EventType.Used)
		//		{
		//			//Debug.Log("completionBox got a mouse down");
		//			//Debug.Log("trying to complete");
		//			Rect currentCompletionBoxRect = GetCurrentCompletionBoxRect();
		//			Rect rectMinusScrollBar = new Rect(currentCompletionBoxRect)
		//			{
		//				width = currentCompletionBoxRect.width - 20,
		//				height = currentCompletionBoxRect.height - 20
		//			};
		//			if (GUILibUtil.MouseInRect(rectMinusScrollBar))
		//			{
		//				Debug.Log("Mouse in completion Rect");
		//				completionManager.Complete();
		//				//GUI.FocusControl(lastControlname);
		//				completionManager.DisplayCurrentCompletions();
		//			}
		//		}
		//	}
		//}



		//Rect GetCurrentWindowRect()
		//{
		//	Rect currentWindowRect = new Rect(mainWindowRect);
		//	if (!editorVisible)
		//	{
		//		currentWindowRect.x += editorRect.width;
		//		currentWindowRect.width -= editorRect.width;
		//	}
		//	if (!replVisible)
		//	{
		//		currentWindowRect.width -= replRect.width;
		//	}
		//	if (!replVisible && !editorVisible)
		//	{
		//		currentWindowRect.width -= completionBoxRect.width;
		//	}
		//	return currentWindowRect;
		//}

		//Rect GetCurrentEditorRect()
		//{
		//	Rect currentEditorRect = new Rect(editorRect);
		//	float scriptNameHeight = GetCurrentScriptNameRect().height;
		//	currentEditorRect.y += scriptNameHeight;
		//	currentEditorRect.height = currentEditorRect.height - scriptNameHeight;

		//	return currentEditorRect;
		//}

		//Rect GetCurrentScriptNameRect()
		//{
		//	Rect currentScriptNameRect = new Rect();
		//	currentScriptNameRect.y = titleHeight;
		//	currentScriptNameRect.x = 0;
		//	currentScriptNameRect.width = editorRect.width;
		//	currentScriptNameRect.height = 25;
		//	return currentScriptNameRect;
		//}

		//Rect GetCurrentWidgetBarRect()
		//{
		//	Rect currentWidgetRect = new Rect(widgetBarRect);
		//	if (!editorVisible)
		//	{
		//		currentWidgetRect.x -= editorRect.width;
		//	}
		//	return currentWidgetRect;
		//}

		//Rect GetCurrentReplRect()
		//{
		//	Rect currentReplRect = new Rect(replRect);
		//	if (!editorVisible)
		//	{
		//		currentReplRect.x -= editorRect.width;
		//	}
		//	return currentReplRect;
		//}

		//Rect GetCurrentCompletionBoxRect()
		//{
		//	Rect currentCompletionBoxRect = new Rect(completionBoxRect);
		//	if (!editorVisible)
		//	{
		//		currentCompletionBoxRect.x -= editorRect.width;
		//	}
		//	if (!replVisible)
		//	{
		//		currentCompletionBoxRect.x -= replRect.width;
		//	}

		//	return currentCompletionBoxRect;
		//}
	}
}

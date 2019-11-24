using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.KSP.Settings;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.EventHandling;
using LiveRepl.Interfaces;
using Kerbalui.Util;

namespace LiveRepl.Parts {
	public class ScriptNameInputArea:EditingArea, ICompletableElement {
		string DefaultScriptFilename
		{
			get
			{
				var extension=".lua";
				if (uiparts.scriptWindow.currentReplEvaluator!=null)
				{
					extension=uiparts.scriptWindow.currentReplEvaluator.Extension;
				}

				return "untitled"+extension;
			}
		}
		public ScriptWindowParts uiparts;

		public ScriptNameInputArea(ScriptWindowParts uiparts):base(new TextField())
		{
			this.uiparts=uiparts;

			Text = SavedSettings.LoadSetting("lastScriptName",DefaultScriptFilename);
			if (!File.Exists(Path.Combine(SavedSettings.BaseScriptsPath, Text))) {
				IList<string> recentFiles = SavedSettings.LoadListSetting("recentFiles");
				if (recentFiles.Count > 0) {
					Text = recentFiles[0];
				} else {
					Text = DefaultScriptFilename;
				}
			};

			keybindings.Clear();

			//uiparts.FontChange+=editableText.FontChangeEventHandler;
		}

		protected override void DecoratorUpdate()
		{
			if (Event.current.type==EventType.MouseDown && GUILibUtil.MouseInRect(rect))
			{
				Text="";
				GrabFocus();
			}

			base.DecoratorUpdate();
			if (ReceivedInput)
			{
				uiparts.scriptWindow.needsResize=true;
			}
		}

		public void Complete(int index)
		{
			var completionContent = GetCompletionContent(out int replaceStart,out int replaceEnd);
			if (completionContent.Count > index) {
				Text = completionContent[index];
				SelectIndex=CursorIndex = Text.Length;
			}

			uiparts.scriptWindow.LoadEditorText();
			uiparts.editor.GrabFocus();
		}

		public string PartialCompletion()
		{
			return Text;
		}

		void CommonSaveLoadActions()
		{
			SavedSettings.SaveSetting("lastScriptName", Text);
		}

		public void SaveText(string text)
		{
			try {
				var previousText = ""; 
				try
				{
					previousText = RedOnion.KSP.ROS.RosProcessor.LoadScript(Text).Replace("\r", "");
				}
				catch (Exception)
				{
					previousText = "";
				}
				if (text != previousText)
				{
					//UnityEngine.Debug.Log(text + "\n!=\n" + previousText);
					File.WriteAllText(CreateFullPath(true), text.Replace("\n", Environment.NewLine));
				}
				CommonSaveLoadActions();
			}
			catch(Exception e) {
				UnityEngine.Debug.Log(e.StackTrace);
			}
		}

		public string LoadText()
		{
			string result = "";
			try { 
				CommonSaveLoadActions();
				CreateFullPath();
				result=RedOnion.KSP.ROS.RosProcessor.LoadScript(Text).Replace("\r","");
			}
			catch(Exception e) {
				UnityEngine.Debug.Log(e.StackTrace);
			}
			return result;
		}

		string CreateFullPath(bool forSave = false)
		{
			if (Text == "") {
				Text = DefaultScriptFilename;
			}

			Directory.CreateDirectory(SavedSettings.BaseScriptsPath);
			string fullPath = Path.Combine(SavedSettings.BaseScriptsPath, Text.Trim());

			if (forSave && !File.Exists(fullPath)) {
				File.WriteAllText(fullPath, "");
			}

			return fullPath;
		}


		public IList<string> GetCompletionContent(out int replaceStart,out int replaceEnd)
		{
			var newList = new List<string>();
			foreach (var scriptPathString in GetScriptList()) {
				string scriptFileName = Path.GetFileName(scriptPathString);
				if (scriptFileName.StartsWith(".")) {
					continue;
				}

				if (scriptFileName.StartsWith(Text)) {
					newList.Add(scriptFileName);
				}
			}
			replaceStart = 0;replaceEnd = Text.Length;
			return newList;
		}

		Stopwatch ioDelayWatch = new Stopwatch();
		List<string> scriptList;
		const int ioDelayMillis = 1000;

		public string ControlName => editableText.ControlName;

		/// <summary>
		/// This is used to update the script list at most every ioDelayMillis milliseconds.
		/// </summary>
		List<string> GetScriptList()
		{
			if (scriptList == null || ioDelayWatch.ElapsedMilliseconds > ioDelayMillis) {
				scriptList = RedOnion.KSP.ROS.RosProcessor.EnumerateScripts();
				ioDelayWatch.Reset();
				ioDelayWatch.Start();
			}
			return scriptList;
		}
	}
}

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.KSP.Settings;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.EventHandling;

namespace LiveRepl.UI.EditorParts {
	public class ScriptNameInputArea:EditingArea {
		static string defaultScriptFilename= "untitled.lua";
		public FileIOGroup fileIOGroup;

		public ScriptNameInputArea(FileIOGroup fileIOGroup):base(new TextField())
		{
			this.fileIOGroup=fileIOGroup;

			Text = SavedSettings.LoadSetting("lastScriptName",defaultScriptFilename);
			if (!File.Exists(Path.Combine(SavedSettings.BaseScriptsPath, Text))) {
				IList<string> recentFiles = SavedSettings.LoadListSetting("recentFiles");
				if (recentFiles.Count > 0) {
					Text = recentFiles[0];
				} else {
					Text = defaultScriptFilename;
				}
			}
			keybindings.Clear();
		}

		//public new KeyBindings KeyBindings = new KeyBindings();

		//protected override void ProtectedUpdate()
		//{
		//	if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
		//	base.ProtectedUpdate();
		//}
		protected override void DecoratorUpdate()
		{
			base.DecoratorUpdate();
			if (hadKeyDownThisUpdate)
			{
				fileIOGroup.editorGroup.needsResize=true;
			}
		}

		public void Complete(int index)
		{
			var completionContent = GetCompletionContent(out int replaceStart,out int replaceEnd);
			if (completionContent.Count > index) {
				Text = completionContent[index];
				SelectIndex=CursorIndex = Text.Length;
			}
			//editor.text=Load();
			Text=Load();
		}

		public string PartialCompletion()
		{
			return Text;
		}

		void CommonSaveLoadActions()
		{
			SavedSettings.SaveSetting("lastScriptName", Text);
		}

		public void Save(string text)
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

		public string Load()
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
				Text = defaultScriptFilename;
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

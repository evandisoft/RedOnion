using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Kerbalua.Utility;

namespace Kerbalua.Gui {
	public class ScriptNameInputArea:EditingArea, ICompletableElement {
		public bool receivedInput;


		static string defaultScriptFilename= "untitled.lua";

		public ScriptNameInputArea()
		{
			content.text = Settings.LoadSetting("lastScriptName",defaultScriptFilename);
			if (!File.Exists(Path.Combine(Settings.BaseScriptsPath, content.text))) {
				IList<string> recentFiles = Settings.LoadListSetting("recentFiles");
				if (recentFiles.Count > 0) {
					content.text = recentFiles[0];
				} else {
					content.text = defaultScriptFilename;
				}
			}
		}

		public new KeyBindings KeyBindings = new KeyBindings();

		protected override void ProtectedUpdate()
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.ProtectedUpdate();
		}

		public void Complete(int index)
		{
			var completionContent = GetCompletionContent(out int replaceStart,out int replaceEnd);
			if (completionContent.Count > index) {
				content.text = completionContent[index];
				selectIndex=cursorIndex = content.text.Length;
			}
		}

		public string PartialCompletion()
		{
			return content.text;
		}

		void CommonSaveLoadActions()
		{
			Settings.SaveSetting("lastScriptName", content.text);
		}

		public void Save(string text)
		{
			try {
				var previousText = ""; 
				try
				{
					previousText = RedOnion.KSP.ROS.KspRosEngine.LoadScript(content.text).Replace("\r", "");
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
				result=RedOnion.KSP.ROS.KspRosEngine.LoadScript(content.text).Replace("\r","");
			}
			catch(Exception e) {
				UnityEngine.Debug.Log(e.StackTrace);
			}
			return result;
		}

		string CreateFullPath(bool forSave = false)
		{
			if (content.text == "") {
				content.text = defaultScriptFilename;
			}

			Directory.CreateDirectory(Settings.BaseScriptsPath);
			string fullPath = Path.Combine(Settings.BaseScriptsPath, content.text);

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

				if (scriptFileName.StartsWith(content.text)) {
					newList.Add(scriptFileName);
				}
			}
			replaceStart = 0;replaceEnd = content.text.Length;
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
				scriptList = RedOnion.KSP.ROS.KspRosEngine.EnumerateScripts();
				ioDelayWatch.Reset();
				ioDelayWatch.Start();
			}
			return scriptList;
		}
	}
}

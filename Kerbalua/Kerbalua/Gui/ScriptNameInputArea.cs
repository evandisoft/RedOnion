using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kerbalua.Gui {
	public class ScriptNameInputArea:EditingArea, ICompletable {
		public bool receivedInput;

		static string baseFolderPath;
		static string settingsFile;
		static string defaultScriptFilename= "untitled.b";
		static ScriptNameInputArea()
		{
			baseFolderPath = Path.Combine(KSPUtil.ApplicationRootPath, "scripts");
			settingsFile = Path.Combine(baseFolderPath, ".settings");
		}

		public ScriptNameInputArea()
		{
			content.text = LoadConfig().GetValue("lastScriptName");
		}

		public new KeyBindings KeyBindings = new KeyBindings();

		ConfigNode LoadConfig()
		{
			ConfigNode configNode;
			if (!File.Exists(settingsFile)) {
				Directory.CreateDirectory(baseFolderPath);
				configNode = new ConfigNode();
				configNode.SetValue("lastScriptName", "", true);
				configNode.Save(settingsFile);
				return configNode;
			} 
			return ConfigNode.Load(settingsFile);
		}

		protected override void ProtectedUpdate()
		{
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.ProtectedUpdate();
		}

		public void Complete(int index)
		{
			var completionContent = GetCompletionContent();
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
			ConfigNode configNode = LoadConfig();
			configNode.SetValue("lastScriptName", content.text, true);
			configNode.Save(settingsFile);
		}

		public void Save(string text)
		{
			try {
				CommonSaveLoadActions();
				File.WriteAllText(CreateFullPath(), text);
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
				result=File.ReadAllText(CreateFullPath());
			}
			catch(Exception e) {
				UnityEngine.Debug.Log(e.StackTrace);
			}
			return result;
		}

		string CreateFullPath()
		{
			if (content.text == "") {
				content.text = defaultScriptFilename;
			}

			Directory.CreateDirectory(baseFolderPath);
			string fullPath = Path.Combine(baseFolderPath,content.text);

			if (!File.Exists(fullPath)) {
				File.WriteAllText(fullPath, "");
			}

			return fullPath;
		}


		public List<string> GetCompletionContent()
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
				scriptList = new List<string>(Directory.GetFiles(baseFolderPath));
				ioDelayWatch.Reset();
				ioDelayWatch.Start();
			}
			return scriptList;
		}
	}
}

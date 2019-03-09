using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kerbalua.Gui {
	public class ScriptNameInputArea:EditingArea, ICompletable {
		public bool receivedInput;
		public string baseFolderPath = "scripts"; 
		public string defaultScriptFilename = "untitled.b";
		public new KeyBindings KeyBindings = new KeyBindings();
		bool hadFocus;

		public override void Render()
		{
			receivedInput = HasFocus() && Event.current.type == EventType.KeyDown;
			KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.Render();
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

		public void Save(string text)
		{
			Directory.CreateDirectory(baseFolderPath);
			File.WriteAllText(CreateFullPath(), text);
		}

		public string Load()
		{
			Directory.CreateDirectory(baseFolderPath);
			return File.ReadAllText(CreateFullPath());
		}

		string CreateFullPath()
		{
			if (content.text == "") {
				content.text = defaultScriptFilename;
			}

			string fullPath = baseFolderPath + "/" + content.text;

			if (!File.Exists(fullPath)) {
				File.WriteAllText(fullPath, "");
			}

			return fullPath;
		}


		public List<string> GetCompletionContent()
		{
			var newList = new List<string>();
			foreach (var scriptName in GetScriptList()) {
				if (scriptName.StartsWith(baseFolderPath + "/" + content.text)) {
					newList.Add(scriptName.Split('/')[1]);
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

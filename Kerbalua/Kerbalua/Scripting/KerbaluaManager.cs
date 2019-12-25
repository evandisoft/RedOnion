using Kerbalua.Completion;
using MunOS;
using MunOS.Repl;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kerbalua.Scripting
{
	public class KerbaluaManager : ScriptManager
	{
		public override string Extension => ".lua";

		public override MunProcess CreateProcess(MunProcess shareWith = null)
			=> new KerbaluaProcess(this, shareWith as KerbaluaProcess);
		public override MunThread CreateThread(string source, string path,
			MunProcess process = null, MunPriority priority = MunPriority.Main, bool start = true)
		{
			if (source==null)
			{
				var basepath=Path.GetFileNameWithoutExtension(path);
				source=$"require(\"{basepath}\")";
			}
			return new KerbaluaThread((KerbaluaProcess)(process ?? Process), priority,
				source, path, start);
		}


		public override void Evaluate(string source, string path, bool withHistory = false)
		{
			var thread = new KerbaluaReplThread((KerbaluaProcess)Process, source, path, start: Initialized);
			if (!Initialized)
				waitingThreads.Add(thread);
			if (withHistory)
				History.Add(source);
		}

		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd, MunProcess process = null)
		{
			try
			{
				return MoonSharpIntellisense.GetCompletions(
					((KerbaluaProcess)(process ?? Process)).ScriptEngine.Globals,
					source, cursorPos, out replaceStart, out replaceEnd);
			}
			catch (Exception e)
			{
				// TODO: use MunCore.OnError or at least RedOnion.Common logging
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}
	}
}

using Kerbalua.Completion;
using MunOS;
using MunOS.Repl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kerbalua.Scripting
{
	public class KerbaluaManager : ScriptManager
	{
		public override string Extension => ".lua";

		public override MunProcess CreateProcess()
			=> new KerbaluaProcess(this);
		public override MunThread CreateThread(string source, string path,
			MunProcess process = null, MunPriority priority = MunPriority.Main, bool start = true)
			=> new KerbaluaThread((KerbaluaProcess)(process ?? Process), priority, source, path, start);

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

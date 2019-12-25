using MunOS;
using MunOS.Repl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOnion.KSP.ROS
{
	public class RosManager : ScriptManager
	{
		public override string Extension => ".ros";

		public override MunProcess CreateProcess()
			=> new RosProcess(this);
		public override MunThread CreateThread(string source, string path,
			MunProcess process = null, MunPriority priority = MunPriority.Main, bool start = true)
			=> new RosThread((RosProcess)(process ?? Process), priority,
				source ?? $"run \"{path}\"", path, start);

		RosSuggest suggest;
		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd, MunProcess process = null)
		{
			try
			{
				if (suggest == null)
					suggest = new RosSuggest();
				suggest.Processor = ((RosProcess)(process ?? Process)).Processor;
				return suggest.GetCompletions(source, cursorPos, out replaceStart, out replaceEnd);
			}
			catch (Exception ex)
			{
				// TODO: use MunCore.OnError or at least RedOnion.Common logging
				Debug.Log(ex);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}
	}
}

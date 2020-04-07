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

		public static MunProcess ProcessCreator(string path, object[] args)
		{
			var current = MunProcess.Current;
			var manager = current?.ScriptManager as RosManager;
			var process = manager != null ? new RosProcess(manager) : new RosProcess(current?.Core);
			process.Name = path;
			process.OutputBuffer = current?.OutputBuffer;
			new RosThread(process, MunPriority.Main, $"run \"{path}\"", path);
			return process;
		}

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

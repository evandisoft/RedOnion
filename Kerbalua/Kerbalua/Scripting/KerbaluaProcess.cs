using System;
using System.Collections.Generic;
using System.IO;
using Kerbalua.Completion;
using MoonSharp.Interpreter;
using MunOS.Core;
using MunOS.ProcessLayer;
using UnityEngine;

namespace Kerbalua.Scripting
{
	public class KerbaluaProcess:EngineProcess
	{
		public readonly KerbaluaScript scriptEngine;

		public KerbaluaProcess()
		{
			scriptEngine=new KerbaluaScript();
		}

		public override string Extension => ".lua";

		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			try
			{
				return MoonSharpIntellisense.GetCompletions(scriptEngine.Globals, source, cursorPos, out replaceStart, out replaceEnd);
			}
			catch (Exception e)
			{
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}



		public override IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			return GetCompletions(source, cursorPos, out replaceStart, out replaceEnd);
		}

		public override string GetImportString(string scriptname)
		{
			string basename = Path.GetFileNameWithoutExtension(scriptname);

			return "require(\""+basename+"\")";
		}

		public void ExecuteFunctionInThread(ExecPriority priority, Closure closure)
		{
			try
			{
				var thread=new KerbaluaThread(closure,this);
				ExecuteThread(priority, thread);
			}
			catch (Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}

		protected override void ExecuteSourceInThread(ExecPriority priority, string source, string path)
		{
			try
			{
				var thread=new KerbaluaThread(source,path,this);
				ExecuteThread(priority, thread);
			}
			catch(Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}
	}
}

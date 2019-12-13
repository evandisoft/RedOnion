using System;
using System.Collections.Generic;
using System.IO;
using Kerbalua.Completion;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using MunOS.Core;
using MunOS.ProcessLayer;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace Kerbalua.Scripting
{
	public class KerbaluaProcess:EngineProcess
	{
		public KerbaluaScript ScriptEngine { get; private set; }

		public KerbaluaProcess()
		{
			InternalResetEngine();
		}

		protected override MunThread CreateThread(string source, string path)
		{
			return new KerbaluaThread(source, path, this);
		}

		void InternalResetEngine()
		{
			ScriptEngine=new KerbaluaScript(this);
			ScriptEngine.Options.DebugPrint = outputBuffer.AddOutput;
			ScriptEngine.PrintErrorAction = outputBuffer.AddError;

			ScriptEngine.Options.ScriptLoader = new FileSystemScriptLoader();
			var slb=ScriptEngine.Options.ScriptLoader as ScriptLoaderBase;
			slb.IgnoreLuaPathGlobal = true;

			slb.ModulePaths = new string[] { SavedSettings.BaseScriptsPath+"/?.lua" };
		}

		public override string Extension => ".lua";

		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			try
			{
				return MoonSharpIntellisense.GetCompletions(ScriptEngine.Globals, source, cursorPos, out replaceStart, out replaceEnd);
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
				EnqueueThread(priority, thread);
			}
			catch (Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}

		protected override void ExecuteSourceInReplThread(ExecPriority priority, string source, string path)
		{
			try
			{
				var thread=new KerbaluaReplThread(source,path,this);
				EnqueueThread(priority, thread);
			}
			catch(Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}

		public override void ExecuteSourceInThread(ExecPriority priority, string source, string path)
		{
			try
			{
				var thread=new KerbaluaThread(source,path,this);
				EnqueueThread(priority, thread);
			}
			catch (Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}

		public override void ResetEngine()
		{
			base.ResetEngine();
			InternalResetEngine();
		}


	}
}

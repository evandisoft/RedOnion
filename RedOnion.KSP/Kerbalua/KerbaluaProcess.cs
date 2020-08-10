using System;
using System.Collections.Generic;
using System.IO;
using Kerbalua.Completion;
using Kerbalua.Scripting;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using MunOS;
using MunOS.Repl;
using RedOnion.KSP.API;
using RedOnion.KSP.MoonSharp.MoonSharpAPI;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace RedOnion.KSP.Kerbalua
{
	public class KerbaluaProcess:MunProcess
	{
		public KerbaluaScript ScriptEngine { get; private set; }

		//TODO: implement process memory/globals sharing
		public KerbaluaProcess(ScriptManager manager) : this(manager.Core)
			=> ScriptManager = manager;
		public KerbaluaProcess(MunCore core) : base(core)
			=> InternalResetEngine();

		void InternalResetEngine()
		{
			ScriptEngine=new KerbaluaScript(this);
			ScriptEngine.AddAPI(typeof(Globals));
			ScriptEngine.AddAPI(typeof(MoonSharpGlobals));

			ScriptEngine.Options.ScriptLoader = new FileSystemScriptLoader();
			var slb=ScriptEngine.Options.ScriptLoader as ScriptLoaderBase;
			slb.IgnoreLuaPathGlobal = true;

			slb.ModulePaths = new string[] { SavedSettings.BaseScriptsPath+"/?.lua" };
		}

		protected override void OnSetOutputBuffer(OutputBuffer value, OutputBuffer prev)
		{
			ScriptEngine.Options.DebugPrint = null;
			ScriptEngine.PrintErrorAction = null;
			if (value != null)
			{
				ScriptEngine.Options.DebugPrint = OutputBuffer.AddOutput;
				ScriptEngine.PrintErrorAction = OutputBuffer.AddError;
			}
		}

		protected override void OnError(MunEvent err)
		{
			if (!err.Printed && err.Exception != null && OutputBuffer != null)
			{
				Exception exception=err.Exception;
				if (exception is InterpreterException interExcept)
				{
					OutputBuffer.AddError(interExcept.DecoratedMessage);
				}
				else
				{
					OutputBuffer.AddError(exception.Message);
				}
				err.Printed = true;

				Debug.Log(exception);
			}
		}

		/*
		public override string GetImportString(string scriptname)
		{
			string basename = Path.GetFileNameWithoutExtension(scriptname);

			return "require(\""+basename+"\")";
		}

		public void Execute(ExecPriority priority, Closure closure)
		{
			try
			{
				var thread=new KerbaluaThread(closure,this);
				EnqueueThread(priority, thread);
			}
			catch (Exception e)
			{
				PrintException(e);
			}
		}

		public override void Execute(ExecPriority priority, string source, string path, bool inRepl)
		{
			try
			{
				MunThread thread=null;
				if (inRepl)
				{
					thread=new KerbaluaReplThread(source, path, this);
				}
				else
				{
					thread=new KerbaluaThread(source, path, this);
				}
				EnqueueThread(priority, thread);
			}
			catch(Exception e)
			{
				PrintException(e);
			}
		}

		public override void ResetEngine()
		{
			base.ResetEngine();
			InternalResetEngine();
		}

		protected override void ThreadExecutionComplete(MunThread thread, Exception e)
		{
			if (e!=null)
			{
				PrintException(e);
			}
		}
		*/
	}
}

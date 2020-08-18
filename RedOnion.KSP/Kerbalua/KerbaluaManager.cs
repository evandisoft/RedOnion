using Kerbalua.Completion;
using MunOS;
using MunOS.Repl;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Kerbalua.Scripting;
using MunSharp.Interpreter;
using RedOnion.KSP.Kerbalua.Events;
using RedOnion.KSP.API;
using RedOnion.UI;
using MunSharp.Interpreter.Interop;

namespace RedOnion.KSP.Kerbalua
{
	public class KerbaluaManager : ScriptManager
	{

		static KerbaluaManager()
		{
			Script.GlobalOptions.CustomConverters
				.SetClrToScriptCustomConversion(
					(Script script, Vector3d vector3d)
						=> DynValue.FromObject(script, new Vector(vector3d)) //DynValue.NewTable(new ModuleControlSurfaceProxyTable(this, m))
					);
			UserData.RegisterType<Button>(new LuaDescriptor(typeof(Button)));

			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

			Script.GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function, typeof(Action<Button>), (f) =>
				{
					return new Action<Button>((button) =>
					{
						var closure=f.Function;
						var kerbaluaScript=closure.OwnerScript as KerbaluaScript;
						if (kerbaluaScript==null)
						{
							throw new Exception("Ownerscript was not KerbaluaScript in KerbaluaStaticInit");
						}

						var process=kerbaluaScript?.kerbaluaProcess;
						if (process==null)
						{
							throw new Exception("Could not get current process in KerbaluaStaticInit");
						}

						var kerbaluaProcess=process as KerbaluaProcess;

						// I don't expect this to ever be the case.
						if (kerbaluaProcess==null)
						{
							throw new Exception("Process in LuaEventDescriptor was not a KerbaluaStaticInit");
						}

						new KerbaluaThread(kerbaluaProcess, MunPriority.Callback, closure);
					});
				});
		}

		public static MunProcess ProcessCreator(string path, object[] args)
		{
			var current = MunProcess.Current;
			var manager = current?.ScriptManager as KerbaluaManager;
			var process = manager != null ? new KerbaluaProcess(manager) : new KerbaluaProcess(current?.Core);
			process.Name = path;
			process.OutputBuffer = current?.OutputBuffer;
			var basepath=Path.GetFileNameWithoutExtension(path);
			new KerbaluaThread(process, MunPriority.Main, $"require(\"{basepath}\")", path);
			return process;
		}

		public override string Extension => ".lua";

		public override MunProcess CreateProcess() 
			=> new KerbaluaProcess(this);
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

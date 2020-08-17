using System;
using Kerbalua.Scripting;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MunOS;
using RedOnion.KSP.API;
using RedOnion.UI;
using RedOnion.KSP.Kerbalua.Events;

namespace RedOnion.KSP.Kerbalua
{
	/// <summary>
	/// Initializes the global settings for Kerbalua's use of MunSharp
	/// </summary>
	public static class KerbaluaStaticInit
	{
		static KerbaluaStaticInit() {
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
	}
}

using System;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.Lua.Proxies
{
	/// <summary>
	/// A proxy for KSP modules. Necessary because MoonSharp cannot handle
	/// duplicate names in subclasses that should just hide base class members.
	/// ModuleControlSurface redefines transformName.
	/// </summary>
	public class ModuleControlSurfaceProxy
	{

		public ModuleControlSurface module;

		[MoonSharpHidden]
		public ModuleControlSurfaceProxy(ModuleControlSurface module)
		{
			this.module = module;
		}

		//[MoonSharpUserDataMetamethod("__index")]
		static public DynValue Index(ModuleControlSurfaceProxy m,string obj)
		{
			return DynValue.NewString(obj);
		}

		public override string ToString()
		{
			return module.ToString();
		}
	}
}

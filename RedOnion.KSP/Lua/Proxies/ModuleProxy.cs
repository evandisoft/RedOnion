using System;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.Lua.Proxies
{
	/// <summary>
	/// A proxy for KSP modules. Necessary because MoonSharp cannot handle
	/// duplicate names in subclasses that should just hide base class members.
	/// </summary>
	public class ModuleProxy
	{

		PartModule partModule;

		[MoonSharpHidden]
		public ModuleProxy(PartModule partModule)
		{
			this.partModule = partModule;
			var a = new ModuleControlSurface();

		}

		public override string ToString()
		{
			return partModule.ToString();
		}
	}
}

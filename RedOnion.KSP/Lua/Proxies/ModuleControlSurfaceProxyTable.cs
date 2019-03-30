using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace RedOnion.KSP.Lua.Proxies
{
	public class ModuleControlSurfaceProxyTable : ProxyTable
	{
		public ModuleControlSurfaceProxyTable(MoonSharp.Interpreter.Script script, object proxied) : base(script, proxied)
		{
			this["GetPotentialTorque"] = new GetPotentialTorque(GetPotentialTorqueImpl);
		}

		delegate void GetPotentialTorque(out Vector3 pos, out Vector3 neg);

		void GetPotentialTorqueImpl(out Vector3 pos, out Vector3 neg)
		{
			((ModuleControlSurface)ProxiedObject).GetPotentialTorque(out pos, out neg);
		}
	}
}

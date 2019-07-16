using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public class Time : ICallable
	{
		[Browsable(false), MoonSharpHidden]
		public static Time Instance { get; } = new Time();
		protected Time() { }

		public double Now => Planetarium.GetUniversalTime();

		bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
		{
			result = Now;
			return true;
		}
		[MoonSharpUserDataMetamethod("__call"), Browsable(false)]
		public DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			=> DynValue.NewNumber(Now);
	}
}

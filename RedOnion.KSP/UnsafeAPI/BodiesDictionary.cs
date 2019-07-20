using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.CommonAPIHelpers;
using RedOnion.KSP.Completion;

namespace RedOnion.KSP.UnsafeAPI
{
	[Description("A dictionary mapping body names to CelestialBody instances. The bodies will be whatever"
	+" is returned by FlightGlobals.Bodies")]
	public class BodiesDictionary : ScriptStringKeyedConstDictionary<CelestialBody>
	{
		BodiesDictionary()
		{
		}

		static BodiesDictionary instance = null;
		public static BodiesDictionary Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new BodiesDictionary();
					var bodiesArray = FlightGlobals.Bodies;
					foreach(var body in bodiesArray)
					{
						instance.Add(body.bodyName.Replace(' ','_').ToLower(), body);
					}
				}
				return instance;
			}
		}
	}
}

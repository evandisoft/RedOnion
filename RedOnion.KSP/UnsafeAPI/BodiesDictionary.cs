using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Utilities;

namespace RedOnion.KSP.UnsafeAPI
{
	[Description(@"
A collection of space/celestial bodies.
Acess them by `bodies.bodyname`. 
For example bodies.mun will return a reference to the mun.

This version of `bodies` returns the native CelestialBody.
")]
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

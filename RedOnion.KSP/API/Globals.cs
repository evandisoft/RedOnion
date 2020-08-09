using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RedOnion.Attributes;
using RedOnion.KSP.Namespaces;
using RedOnion.KSP.ReflectionUtil;

namespace RedOnion.KSP.API
{
	public static class ApiMain
	{
		static bool done;
		/// <summary>
		/// To be called at least once before using <see cref="Globals"/>.
		/// </summary>
		/// <param name="core">Core to be used, <see cref="MunCore.Default"/> if null.</param>
		public static void Init(MunOS.MunCore core = null)
		{
			if (done)
				return;
			if (core == null)
				core = MunOS.MunCore.Default;
			core.BeforeExecute += Science.update;

			OperatingSystemInterface.RegisterProcessCreator(".ros", ROS.RosManager.ProcessCreator);
			done = true;
		}
		/// <summary>
		/// Reset API - disable autopiloat, clear all subscriptions.
		/// </summary>
		public static void Reset()
		{
			Ship.DisableAutopilot();
			Science.situationChanged.clear();
		}
	}

	[Description("Global variables, objects and functions common to all scripting languages.")]
	public static class Globals
	{
		#region Fields: Type references - Namespaces and singletons (static classes)

		[Description("An api for setting which scripts will be ran when an engine is reset.")]
		public static readonly Type autorun = typeof(AutoRun);

		// 'os' conflicts with lua's 'os' namespace so 'munos' is an alias for that.
		[Description("Operating System - interface to MunOS.")]
		public static readonly Type munos = typeof(OperatingSystem);
		[Description("Operating System - interface to MunOS.")]
		public static readonly Type os = typeof(OperatingSystem);

		[Description("User Interface.")]
		public static readonly Type ui = typeof(UI_Namespace);
		[Unsafe, Description("Shortcuts to KSP API + some tools.")]
		public static readonly Type ksp = typeof(KSP_Namespace);
		[Unsafe, Description("Shortcuts to Unity API.")]
		public static readonly Type unity = typeof(Unity_Namespace);
		[Description("Types to be used with ROS: `is` operator; Lua: `isa` function.")]
		public static readonly Type types = typeof(Types_Namespace);

		[Unsafe, Description("Namespace Mappings (import of native types by namespace). More info [here](../ReflectionUtil/NamespaceInstance.md)")]
		public static readonly NamespaceInstance native = NamespaceMappings.DefaultAssemblies.GetNamespace("");
		[Unsafe, Description("Assembly Mappings (import of native types by assembly). More info [here](../ReflectionUtil/GetMappings.md)")]
		public static readonly GetMappings assembly = new GetMappings();

		[Description("Staging logic.")]
		public static readonly Type stage = typeof(Stage);
		[Description("Current time and related functions.")]
		public static readonly Type time = typeof(Time);
		[Description("Maneuver node.")]
		public static readonly Type node = typeof(Node);
		[WorkInProgress, Description("User/player controls.")]
		public static readonly Type player = typeof(Player);
		[WorkInProgress, Description("User/player controls.")]
		public static readonly Type user = typeof(Player);
		[WorkInProgress, Description("Science tools.")]
		public static readonly Type science = typeof(Science);

		[Description("PID regulator (alias to `system.pid` in ROS).")]
		public static readonly Type PID = typeof(PID);
		[WorkInProgress, Description("Safe API for KSP Application Launcher (toolbar/buttons). WIP")]
		public static readonly Type app = typeof(App);

		#endregion

		#region Props: Main objects and singletons (that cannot be static classes)

		[Description("Function for creating 3D vector / coordinate.")]
		public static VectorCreator vector => VectorCreator.Instance;

		[Description("Active vessel (in flight only, null otherwise).")]
		public static Ship ship => Ship.Active;
		[Description("Autopilot for active vessel. (`null` if no ship)")]
		public static Autopilot autopilot => ship?.autopilot;

		[Description("A collection of space/celestial bodies. (Safe API)")]
		public static Bodies bodies => Bodies.Instance;

		//Not sure if I want to add this yet. It works, but not sure it will be
		// structured this way.
		//[Unsafe, Description("A map of kerbal names to kerbals for kerbals in the crew.")]
		//public static KerbalsDictionary kerbals => KerbalsDictionary.Instance;

		[WorkInProgress, Description("Target of active ship. Null if none.")]
		public static object target
		{
			get
			{
				if (!HighLogic.LoadedSceneIsFlight)
					return null;
				var target = FlightGlobals.fetch.VesselTarget;
				if (target is CelestialBody body)
					return bodies[body];
				if (target is Vessel vessel)
					return Ship.FromVessel(vessel);
				if (target is PartModule dock)
				{
					var part = dock.part;
					return Ship.FromVessel(part.vessel).parts[part];
				}
				return null;
			}
		}

		#endregion

		#region Props: shortcuts

		[Description("Alias to `ship.altitude`. (`NaN` if no ship.)")]
		public static double altitude => ship?.altitude ?? double.NaN;
		[Description("Alias to `ship.apoapsis`. (`NaN` if no ship.)")]
		public static double apoapsis => ship?.apoapsis ?? double.NaN;
		[Description("Alias to `ship.periapsis`. (`NaN` if no ship.)")]
		public static double periapsis => ship?.periapsis ?? double.NaN;
		[Description("Orbited body (redirects to `ship.body`, `null` if no ship).")]
		public static SpaceBody body => ship?.body;
		[Description("Atmosphere parameters of orbited body (redirects to `ship.body.atmosphere`, `atmosphere.none` if no ship).")]
		public static SpaceBody.Atmosphere atmosphere => body?.atmosphere ?? SpaceBody.Atmosphere.none;

		#endregion
	}
}

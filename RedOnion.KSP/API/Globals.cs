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
	[Description("Global variables, objects and functions common to all scripting languages.")]
	public static class Globals
	{
		#region Fields: Type references - Namespaces and singletons (static classes)

		[Description("An api for setting which scripts will be ran when an engine is reset.")]
		public static readonly Type autorun = typeof(AutoRun);

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
		[Description("User/player controls.")]
		public static Player player => Player.Instance;
		[Description("User/player controls.")]
		public static Player user => Player.Instance;

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

	public class RosGlobals : RedOnion.ROS.Objects.Globals
	{
		public override void Fill()
		{
			base.Fill();
			System.Add(typeof(PID));
		}

		class ReflectedGlobals : Reflected
		{
			public ReflectedGlobals() : base(typeof(Globals)) { }
			public int Count => prop.Count;
			public ref Prop this[int idx] => ref prop.items[idx];
			public IEnumerator<string> GetEnumerator()
			{
				for (int i = 0; i < prop.size; i++)
					yield return prop.items[i].name;
			}
		}
		const int mark = 0x7F000000;
		static ReflectedGlobals reflected = new ReflectedGlobals();

		public override int Find(string name)
		{
			int at = reflected.Find(null, name, false);
			if (at >= 0) return at + mark;
			return base.Find(name);
		}
		public override bool Get(ref Value self, int at)
		{
			if (at < mark)
				return base.Get(ref self, at);
			if ((at -= mark) >= reflected.Count)
				return false;
			ref var member = ref reflected[at];
			if (member.read == null)
				return false;
			self = member.read(self.obj);
			return true;
		}
		public override bool Set(ref Value self, int at, OpCode op, ref Value value)
		{
			if (at < mark)
				return base.Set(ref self, at, op, ref value);
			if ((at -= mark) >= reflected.Count)
				return false;
			ref var member = ref reflected[at];
			if (member.write == null)
				return false;
			if (op != OpCode.Assign)
				return false;
			member.write(self.obj, value);
			return true;
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			var seen = new HashSet<string>();
			foreach (var member in reflected)
			{
				seen.Add(member);
				yield return member;
			}
			foreach (var name in EnumerateProperties(self, seen))
				yield return name;
		}
	}

	public class LuaGlobals : Table, ICompletable
	{
		public static LuaGlobals Instance { get; } = new LuaGlobals();

		public IList<string> PossibleCompletions
		{
			get
			{
				IList<string> completions =
					typeof(Globals).GetProperties().Select(t => t.Name).Concat(
						typeof(Globals).GetFields().Select(t => t.Name)).ToList();
				return completions;
			}
		}

		public object CompletionProxy => UserData.CreateStatic(typeof(Globals));

		public bool TryGetCompletion(string completionName, out object completion)
		{
			completion = Get(this, DynValue.NewString(completionName));
			if (completion == null)
			{
				return false;
			}
			return true;
		}

		public LuaGlobals() : base(null)
		{
			this["__index"] = new Func<Table, DynValue, DynValue>(Get);
		}
		DynValue Get(Table table, DynValue index)
		{
			object obj=null;
			var name = index.String;
			var field = typeof(Globals).GetField(name, BindingFlags.Static|BindingFlags.Public);
			if (field !=null)
			{
				obj=field.GetValue(null);
			}
			var prop = typeof(Globals).GetProperty(name, BindingFlags.Static|BindingFlags.Public);
			if (prop != null)
			{
				obj=prop.GetValue(null, null);
			}

			if (obj.GetType().Name=="RuntimeType")
			{
				Type t=obj as Type;
				obj=UserData.CreateStatic(t);
			}


			return DynValue.FromObject(table.OwnerScript, obj);
		}
	}
}

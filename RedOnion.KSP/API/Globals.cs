using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;
using System.ComponentModel;
using System.Reflection;
using RedOnion.KSP.Autopilot;
using RedOnion.KSP.Namespaces;
using System.Linq;
using RedOnion.KSP.UnsafeAPI;
using RedOnion.KSP.ReflectionUtil;

namespace RedOnion.KSP.API
{
	[Description("Global variables, objects and functions.")]
	public static class Globals
	{
		[Description("An api for setting which scripts will be ran when an engine is reset.")]
		//public static AutoRun autorun => AutoRun.Instance;

		public static readonly Type autorun = typeof(AutoRun);
		[Description("Safe API for KSP Application Launcher (toolbar/buttons).")]
		public static readonly Type app = typeof(App);

		[Description("A collection of space/celestial bodies. (Safe API)")]
		public static Bodies bodies => Bodies.Instance;
		[Unsafe, Description("A map of planet names to planet bodies. (Unsafe API)")]
		public static BodiesDictionary getbody => BodiesDictionary.Instance;

		//Not sure if I want to add this yet. It works, but not sure it will be
		// structured this way.
		//[Unsafe, Description("A map of kerbal names to kerbals for kerbals in the crew.")]
		//public static KerbalsDictionary kerbals => KerbalsDictionary.Instance;

		[Unsafe, Description("All the reflection stuff and namespaces. (Lua also has `import`.)")]
		public static Reflect reflect => Reflect.Instance;
		[Unsafe, Description("Alias to `reflect` because of the namespaces.")]
		public static Reflect native => Reflect.Instance;
		[Unsafe, Description("Reflected/imported stuff by assembly name.")]
		public static readonly GetMappings assembly = new GetMappings();

		[Description("Function for creating 3D vector / coordinate.")]
		public static VectorCreator vector => VectorCreator.Instance;
		[DisplayName("V"), Description("Alias to Vector Function for creating 3D vector / coordinate.")]
		public static VectorCreator V => VectorCreator.Instance;
		[Description("Current time and related functions.")]
		public static Time time => Time.Instance;

		[Description("Active vessel (in flight only, null otherwise).")]
		public static Ship ship => Ship.Active;
		[Description("Staging logic.")]
		public static Stage stage => Stage.Instance;
		[Description("Autopilot for active vessel.")]
		public static Autopilot autopilot => ship.autopilot;
		[Description("User/player controls.")]
		public static Player player => Player.Instance;
		[Description("User/player controls.")]
		public static Player user => Player.Instance;

		[Description("Alias to `ship.altitude`")]
		public static double altitude => ship.altitude;
		[Description("Alias to `ship.apoapsis`.")]
		public static double apoapsis => ship.apoapsis;
		[Description("Alias to `ship.periapsis`.")]
		public static double periapsis => ship.periapsis;
		[Description("Orbited body (redirects to `ship.body`).")]
		public static SpaceBody body => ship.body;
		[Description("Atmosphere parameters of orbited body (redirects to `ship.body.atmosphere`).")]
		public static SpaceBody.Atmosphere atmosphere => ship.body.atmosphere;

		[Description("PID regulator (alias to `system.pid` in ROS).")]
		public static readonly Type pid = typeof(PID);
		[Description("PID regulator (alias to `pid`).")]
		public static readonly Type pidloop = typeof(PID);

		[Description("User Interface.")]
		public static readonly Type ui = typeof(UI_Namespace);
		[Unsafe, Description("Shortcuts to (unsafe) KSP API + some tools.")]
		public static readonly Type ksp = typeof(KSP_Namespace);
		[Unsafe, Description("Shortcuts to (unsafe) Unity API.")]
		public static readonly Type unity = typeof(Unity_Namespace);

		[Description("UI.Window")]
		public static readonly Type window = typeof(Window);
		[Description("UI.Anchors")]
		public static readonly Type anchors = typeof(UI.Anchors);
		[Description("UI.Padding")]
		public static readonly Type padding = typeof(UI.Padding);
		[Description("UI.LayoutPadding")]
		public static readonly Type layoutPadding = typeof(UI.LayoutPadding);
		[Description("UI.Layout")]
		public static readonly Type layout = typeof(UI.Layout);
		[Description("UI.Panel")]
		public static readonly Type panel = typeof(UI.Panel);
		[Description("UI.Label")]
		public static readonly Type label = typeof(UI.Label);
		[Description("UI.Button")]
		public static readonly Type button = typeof(UI.Button);
		[Description("UI.TextBox")]
		public static readonly Type textBox = typeof(UI.TextBox);

#if API_GLOBAL_ALIASES
		// TODO: move aliases to startup/setup script/library
		[Alias, Description("Alias to `Vector.dot` (or `V.dot`).")]
		public static readonly string vdot = "Vector.dot";
		[Alias, Description("Alias to `Vector.cross` (or `V.cross`).")]
		public static readonly string vcrs = "Vector.cross";
		[Alias, Description("Alias to `Vector.cross` (or `V.cross`).")]
		public static readonly string vcross = "Vector.cross";
		[Alias, Description("Alias to `Vector.angle` (or `V.angle`).")]
		public static readonly string vangle = "Vector.angle";
		[Alias, Description("Alias to `Vector.angle` (or `V.angle`).")]
		public static readonly string vang = "Vector.angle";
#endif
	}

	public class RosGlobals : RedOnion.ROS.Objects.Globals
	{
		public override void Fill()
		{
			base.Fill();
			System.Add(typeof(UnityEngine.Debug));
			System.Add(typeof(UnityEngine.Color));
			System.Add(typeof(UnityEngine.Rect));
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
#if !API_GLOBAL_ALIASES
			self = member.read(self.obj);
			return true;
#else
			if (member.kind != Reflected.Prop.Kind.Field || member.write != null)
			{
				self = member.read(self.obj);
				return true;
			}
			var fullPath = member.read(self.obj).ToStr();
			var path = fullPath.Split('.');
			at = Find(path[0]);
			if (at < 0)
			{
				Value.DebugLog("Globals: Could not find `{0}`", path[0]);
				return false;
			}
			var item = Value.Void;
			if (!Get(ref item, at))
			{
				Value.DebugLog("Globals: Could not get `{0}`", path[0]);
				return false;
			}
			for (int i = 1; i < path.Length; i++)
			{
				at = item.desc.Find(item.obj, path[i]);
				if (at < 0)
				{
					Value.DebugLog("Globals: Could not find `{0}` in {1}", path[i], fullPath);
					return false;
				}
				if (!item.desc.Get(ref item, at))
				{
					Value.DebugLog("Globals: Could not get `{0}`", path[i], fullPath);
					return false;
				}
			}
			self = item;
			return true;
#endif
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
			;
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
			var name = index.String;
			var prop = typeof(Globals).GetProperty(name, BindingFlags.Static|BindingFlags.Public);
			if (prop != null)
				return DynValue.FromObject(table.OwnerScript, prop.GetValue(null, null));
#if !API_GLOBAL_ALIASES
			return null;
#else
			var alias = typeof(Globals).GetField(name, BindingFlags.Static|BindingFlags.Public);
			if (alias == null || alias.FieldType != typeof(string))
			{
				/*
				if (name.Length == 0 || !char.IsLetter(name, 0) || name == "v")
					return null;
				name = (char.IsLower(name[0])
					? char.ToUpperInvariant(name[0])
					: char.ToLowerInvariant(name[0]))
					+ name.Substring(1);
				prop = typeof(Globals).GetProperty(name, BindingFlags.Static|BindingFlags.Public);
				if (prop != null)
					return DynValue.FromObject(table.OwnerScript, prop.GetValue(null, null));
				*/
				return null;
			}
			var fullPath = (string)alias.GetValue(null);
			var path = fullPath.Split('.');
			var item = Get(table, DynValue.NewString(path[0]));
			for (int i = 1; i < path.Length; i++)
			{
				var data = item?.UserData;
				if (data == null)
					return null;
				item = data.Descriptor.Index(table.OwnerScript, data.Object, DynValue.NewString(path[i]), false);
			}
			return item;
#endif
		}


	}
}

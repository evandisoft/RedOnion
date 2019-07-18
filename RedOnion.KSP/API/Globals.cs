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

namespace RedOnion.KSP.API
{
	[Description("Global variables, objects and functions.")]
	public static class Globals
	{
		[Description("An api for setting which scripts will be ran when an engine is reset.")]

		public static AutoRun autorun => AutoRun.Instance;

		[Description("A map of planet names to planet bodies")]
		public static BodiesDictionary bodies => BodiesDictionary.Instance;

		//Not sure if I want to add this yet. It works, but not sure it will be
		// structured this way.
		//[Description("A map of kerbal names to kerbals for kerbals in the crew.")]
		//public static KerbalsDictionary kerbals => KerbalsDictionary.Instance;

		[Description("All the reflection stuff and namespaces.")]
		public static Reflect reflect => Reflect.Instance;
		[Description("Alias to `reflect` because of the namespaces.")]
		public static Reflect native => Reflect.Instance;

		[Description("Current time")]
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

		[Description("Function for creating 3D vector / coordinate.")]
		public static VectorCreator vector => VectorCreator.Instance;
		[DisplayName("V"), Description("Alias to Vector Function for creating 3D vector / coordinate.")]
		public static VectorCreator V => VectorCreator.Instance;

		[Description("Alias to `ship.Altitude`")]
		public static double altitude => ship.altitude;
		[Description("Alias to `ship.Apoapsis`.")]
		public static double apoapsis => ship.apoapsis;
		[Description("Alias to `ship.Periapsis`.")]
		public static double periapsis => ship.periapsis;

		// TODO: move aliases to startup/setup script/library
		[Alias, Description("Alias to `Vector.dot` (or `v.dot`).")]
		public static readonly string vdot = "Vector.dot";
		[Alias, Description("Alias to `Vector.cross` (or `v.cross`).")]
		public static readonly string vcrs = "Vector.cross";
		[Alias, Description("Alias to `Vector.cross` (or `v.cross`).")]
		public static readonly string vcross = "Vector.cross";
		[Alias, Description("Alias to `Vector.angle` (or `v.angle`).")]
		public static readonly string vangle = "Vector.angle";
		[Alias, Description("Alias to `Vector.angle` (or `v.angle`).")]
		public static readonly string vang = "Vector.angle";
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
			System.Add("PIDloop", typeof(PID));

			System.Add("UI", typeof(UI_Namespace));
			Add(typeof(Window));
			Add(typeof(UI.Anchors));
			Add(typeof(UI.Padding));
			Add(typeof(UI.Layout));
			Add(typeof(UI.Panel));
			Add(typeof(UI.Label));
			Add(typeof(UI.Button));
			Add(typeof(UI.TextBox));

			Add("KSP", typeof(KSP_Namespace));
			Add("Unity", typeof(Unity_Namespace));
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
			var alias = typeof(Globals).GetField(name, BindingFlags.Static|BindingFlags.Public);
			if (alias == null || alias.FieldType != typeof(string))
			{
				if (name.Length == 0 || !char.IsLetter(name, 0) || name == "v")
					return null;
				name = (char.IsLower(name[0])
					? char.ToUpperInvariant(name[0])
					: char.ToLowerInvariant(name[0]))
					+ name.Substring(1);
				prop = typeof(Globals).GetProperty(name, BindingFlags.Static|BindingFlags.Public);
				if (prop != null)
					return DynValue.FromObject(table.OwnerScript, prop.GetValue(null, null));
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
		}


	}
}

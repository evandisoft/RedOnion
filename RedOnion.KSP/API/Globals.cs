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

namespace RedOnion.KSP.API
{
	[Description("Global variables, objects and functions.")]
	public static class Globals
	{
		[Description("An api for setting which scripts will be ran when an engine is reset.")]
#pragma warning disable IDE1006 // Naming Styles
		public static AutoRun autorun => AutoRun.Instance;

		[Description("A map of planet names to planet bodies")]
		public static BodiesDictionary bodies => BodiesDictionary.Instance;

		[Description("All the reflection stuff and namespaces.")]
		public static Reflect Reflect => Reflect.Instance;
		[Description("Alias to `reflect` because of the namespaces.")]
		public static Reflect Native => Reflect.Instance;

		[Description("Active vessel (in flight only, null otherwise).")]
		public static Ship Ship => Ship.Active;
		[Description("Staging logic.")]
		public static Stage Stage => Stage.Instance;

		[Description("Function for creating 3D vector / coordinate.")]
		public static VectorCreator Vector => VectorCreator.Instance;
		[DisplayName("V"), Description("Alias to Vector Function for creating 3D vector / coordinate.")]
		public static VectorCreator V => VectorCreator.Instance;

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

#pragma warning restore IDE1006 // Naming Styles
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

			foreach (var prop in typeof(Globals).GetProperties(BindingFlags.Public|BindingFlags.Static))
				Add(prop.Name, prop.GetValue(null, null));
			foreach (var alias in typeof(Globals).GetFields(BindingFlags.Public|BindingFlags.Static))
			{
				if (alias.FieldType != typeof(string))
					continue;
				var fullPath = (string)alias.GetValue(null);
				var path = fullPath.Split('.');
				int at = Find(path[0]);
				if (at < 0)
				{
					Value.DebugLog("Globals: Could not find `{0}`", path[0]);
					continue;
				}
				Value.DebugLog("Globals: `{0}` at {1}", path[0], at);
				var item = Value.Void;
				if (!Get(ref item, at))
				{
					Value.DebugLog("Globals: Could not get `{0}`", path[0]);
					continue;
				}
				Value.DebugLog("Globals: Got `{0}` at {1}", item.ToString(), at);
				for (int i = 1; i < path.Length; i++)
				{
					at = item.desc.Find(item.obj, path[i]);
					if (at < 0)
					{
						Value.DebugLog("Globals: Could not find `{0}` in {1}", path[i], fullPath);
						goto skip;
					}
					if (!item.desc.Get(ref item, at))
					{
						Value.DebugLog("Globals: Could not get `{0}`", path[i], fullPath);
						goto skip;
					}
				}
				Add(alias.Name, item);
			skip:;
			}
		}
	}

	public class LuaGlobals : Table, IHasCompletionProxy
	{
		public static LuaGlobals Instance { get; } = new LuaGlobals();

		public object CompletionProxy => UserData.CreateStatic(typeof(Globals));

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
				if (name.Length == 0 || !char.IsLower(name, 0) || name == "v")
					return null;
				name = char.ToUpperInvariant(name[0]) + name.Substring(1);
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

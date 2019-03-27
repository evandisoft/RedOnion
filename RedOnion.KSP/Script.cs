using System;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.Completion;
using RedOnion.Script.ReflectedObjects;
using UE = UnityEngine;
using UUI = UnityEngine.UI;
using KUI = KSP.UI;
using ROC = RedOnion.UI.Components;
using RedOnion.KSP.Autopilot;

namespace RedOnion.KSP
{
	public class RuntimeRoot : BasicRoot
	{
		public bool IsRepl { get; }
		public RuntimeRoot(IEngine engine, bool repl)
			: base(engine, fill: false)
		{
			IsRepl = repl;
			Fill();
		}
		// note: for now we ignore IsRepl
		protected override void Fill()
		{
			base.Fill();
			var hard = BaseProps; // will resist overwrite and shadowing in global scope
			var soft = MoreProps; // can be easily overwritten or shadowed

			// core types (added to both "System" namespace and "soft" global)
			var system = Get("System").Object;
			var sys = system?.BaseProps ?? new Properties();
			sys.Set("Delegate", AddType(typeof(Delegate)));
			sys.Set("Debug", AddType(typeof(UE.Debug)));
			sys.Set("Color", AddType(typeof(UE.Color)));
			sys.Set("Rect", AddType(typeof(UE.Rect)));
			sys.Set("Vector2", AddType(typeof(UE.Vector2)));
			var vector = AddType(typeof(UE.Vector3));
			soft.Set("Vector", vector);
			sys.Set("Vector", vector);
			sys.Set("Vector3", vector);
			sys.Set("Vector4", AddType(typeof(UE.Vector4)));
			if (system == null)
				BaseProps.Set("System", new SimpleObject(Engine, sys));

			// soft properties that will later also be added into "KSP" namespace
			var ship = Value.ReadOnly(dummy => ReflectedType.Convert(Engine,
				HighLogic.LoadedSceneIsFlight ? (object)FlightGlobals.ActiveVessel :
				HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null));
			soft.Set("ship", ship);

			// UI namespace
			hard.Set("UI", new Value(new SimpleObject(Engine, new Properties()
			{
				{ "Window",         new Value(e => _window.Get(e)) },
				{ "Anchors",        this[typeof(UI.Anchors)] },
				{ "Padding",        this[typeof(UI.Padding)] },
				{ "Layout",         this[typeof(UI.Layout)] },
				{ "LayoutPadding",  this[typeof(UI.LayoutPadding)] },
				{ "SizeConstraint", this[typeof(UI.SizeConstraint)] },
				{ "SizeConstraints",this[typeof(UI.SizeConstraints)] },
				{ "Constraint",     this[typeof(UI.SizeConstraint)] },
				{ "Constraints",    this[typeof(UI.SizeConstraints)] },
				{ "Element",        this[typeof(UI.Element)] },
				{ "Panel",          this[typeof(UI.Panel)] },
				{ "Label",          this[typeof(UI.Label)] },
				{ "Button",         this[typeof(UI.Button)] },
				{ "TextBox",        this[typeof(UI.TextBox)] },
			})));
			soft.Set("Window",		new Value(e => _window.Get(e)));
			soft.Set("Anchors",		this[typeof(UI.Anchors)]);
			soft.Set("Padding",		this[typeof(UI.Padding)]);
			soft.Set("Layout",		this[typeof(UI.Layout)]);
			soft.Set("Panel",		this[typeof(UI.Panel)]);
			soft.Set("Label",		this[typeof(UI.Label)]);
			soft.Set("Button",		this[typeof(UI.Button)]);
			soft.Set("TextBox",		this[typeof(UI.TextBox)]);

			// KSP stuff
			hard.Set("KSP", new Value(engine =>
			new SimpleObject(engine, new Properties()
			{
				{ "ship", ship },
				{ "Vessel",				this[typeof(Vessel)] },
				{ "FlightGlobals",		this[typeof(FlightGlobals)] },
				{ "FlightCtrlState",	this[typeof(FlightCtrlState)] },
				{ "FlightControl",		this[typeof(FlightControl)]},
				{ "HighLogic",			this[typeof(HighLogic)] },
				{ "InputLockManager",	this[typeof(InputLockManager)] },
				{ "InputLock",			this[typeof(InputLockManager)] },
				{ "StageManager",		this[typeof(KUI.Screens.StageManager)] },
				{ "EditorLogic",		this[typeof(EditorLogic)] },
				{ "ShipConstruction",	this[typeof(ShipConstruction)]},
				{ "FlightDriver",		this[typeof(FlightDriver)] },
				{ "GameScenes",         this[typeof(GameScenes)] },
				{ "PartLoader",         this[typeof(PartLoader)] },
			})));

			// Unity and UI-related stuff
			hard.Set("Unity", new Value(engine =>
			new SimpleObject(engine, new Properties()
			{
				{ "Object",				this[typeof(UE.Object)] },
				{ "GameObject",			this[typeof(UE.GameObject)] },
				{ "Canvas",				this[typeof(UE.Canvas)] },
				{ "CanvasGroup",		this[typeof(UE.CanvasGroup)] },
				{ "RectTransform",		this[typeof(UE.RectTransform)] },
				{ "LayerMask",			this[typeof(UE.LayerMask)] },

				{ "DefaultControls",	this[typeof(UUI.DefaultControls)] },
				{ "GridLayout",			this[typeof(UUI.GridLayoutGroup)] },
				{ "HorizontalLayout",	this[typeof(UUI.HorizontalLayoutGroup)] },
				{ "VerticalLayout",		this[typeof(UUI.VerticalLayoutGroup)] },
				{ "LayoutRebuilder",	this[typeof(UUI.LayoutRebuilder)] },
				{ "LayoutUtility",		this[typeof(UUI.LayoutUtility)] },
				{ "ContentSizeFitter",	this[typeof(UUI.ContentSizeFitter)] },
				{ "AspectRatioFitter",	this[typeof(UUI.AspectRatioFitter)] },
				{ "SizeFitter",			this[typeof(UUI.ContentSizeFitter)] },
				{ "RatioFitter",		this[typeof(UUI.AspectRatioFitter)] },

				{ "Text",				this[typeof(UUI.Text)] },
				{ "Button",				this[typeof(UUI.Button)] },
				{ "Image",				this[typeof(UUI.Image)] },
				{ "RawImage",			this[typeof(UUI.RawImage)] },
				{ "BackgroundImage",	this[typeof(ROC.BackgroundImage)] },
				{ "DragHandler",		this[typeof(ROC.DragHandler)] },
				{ "LayoutComponent",	this[typeof(ROC.LayoutComponent)] },

				{ "Screen",				this[typeof(UE.Screen)] },
				{ "Sprite",				this[typeof(UE.Sprite)] },
				{ "Texture",			this[typeof(UE.Texture)] },
				{ "Texture2D",			this[typeof(UE.Texture2D)] },

				{ "UIMaster",			this[typeof(KUI.UIMasterController)] },
				{ "UISkinDef",			this[typeof(UISkinDef)] },
				{ "UISkinManager",		this[typeof(UISkinManager)] },
				{ "UIStyle",			this[typeof(UIStyle)] },
				{ "UIStyleState",		this[typeof(UIStyleState)] },
			})));

			// IMGUI (can only be used from onGUI!)
			hard.Set("IMGUI", new Value(engine =>
			this[typeof(UE.GUI)] = new ReflectedType(engine, typeof(UE.GUI), new Properties()
			{
				{ "GUISkin",			this[typeof(UE.GUISkin)] },
				{ "GUIStyle",			this[typeof(UE.GUIStyle)] },
				{ "GUIStyleState",		this[typeof(UE.GUIStyleState)] },
				{ "GUIContent",			this[typeof(UE.GUIContent)] },
				{ "GUIElement",			this[typeof(UE.GUIElement)] },
				{ "GUILayer",			this[typeof(UE.GUILayer)] },
				{ "GUILayout",			this[typeof(UE.GUILayout)] },
				{ "GUIText",			this[typeof(UE.GUIText)] },
				{ "GUIUtility",			this[typeof(UE.GUIUtility)] },
			})));
		}

		struct LazyGet
		{
			IObject it;
			CreateObject creator;
			public LazyGet(CreateObject creator)
			{
				it = null;
				this.creator = creator;
			}
			public IObject Get(IEngine e)
				=> it ?? (it = creator(e));
		}
		LazyGet _window = new LazyGet(e => new ROS_UI.WindowFun(e));
	}

	/// <summary>
	/// Runtime engine with all the features
	/// </summary>
	public class RuntimeEngine : Engine
	{
		public RuntimeEngine()
			: base(engine => new RuntimeRoot(engine, repl: false)) { }
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion] " + msg);


		private static Ionic.Zip.ZipFile ScriptsZip;
		public static string LoadScript(string path)
		{
			var data = UI.Element.ResourceFileData(System.Reflection.Assembly.GetCallingAssembly(),
				"Scripts", ref ScriptsZip, path);
			return data == null ? null : System.Text.Encoding.UTF8.GetString(data);
		}
	}
	/// <summary>
	/// Limited engine whith what is safe in REPL / Immediate Mode
	/// </summary>
	public class ImmediateEngine : Engine
	{
		public ImmediateEngine()
			: base(engine => new RuntimeRoot(engine, repl: true))
			=> Options |= EngineOption.Repl;
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion.REPL] " + msg);
	}

	/// <summary>
	/// Engine designed to provide hints and documentation for runtime engine
	/// </summary>
	public class DocumentingEngine : CompletionEngine
	{
		public DocumentingEngine(RuntimeEngine engine)
			: base(engine) { }
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion.DOC] " + msg);
	}
	/// <summary>
	/// Engine designed to provide hints and documentation for REPL / Immediate Mode
	/// </summary>
	public class ReplHintsEngine : CompletionEngine
	{
		public ReplHintsEngine(ImmediateEngine engine)
			: base(engine) { }
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion.ReplDOC] " + msg);
	}
}

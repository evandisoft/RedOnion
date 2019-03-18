using System;
using System.Collections.Generic;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.Completion;
using RedOnion.Script.ReflectedObjects;
using RedOnion.UI;
using UE = UnityEngine;
using UUI = UnityEngine.UI;
using KUI = KSP.UI;

namespace RedOnion.KSP
{
	/// <summary>
	/// Runtime engine with all the features
	/// </summary>
	public class RuntimeEngine : Engine
	{
		public RuntimeEngine()
			: base(engine => new RuntimeRoot(engine, root =>
			FillRoot(root, repl: false))) { }
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion] " + msg);

		public static void FillRoot(IEngineRoot root, bool repl)
		{
#if DEBUG
			repl = false; // FOR NOW!
#endif
			// neutral types first
			// aliases in global namespaces are in MoreProps = overwrittable,
			// while names under "System" namespace are read-only (BaseProps)
			var system = root.Get("System").Object;
			var sys = system?.BaseProps ?? new Properties();
			sys.Set("Delegate",	root.AddType(typeof(Delegate)));
			sys.Set("Debug",	root.AddType(typeof(UE.Debug)));
			sys.Set("Color",	root.AddType(typeof(UE.Color)));
			sys.Set("Rect",		root.AddType(typeof(UE.Rect)));
			sys.Set("Vector2",	root.AddType(typeof(UE.Vector2)));
			var vector = root.AddType(typeof(UE.Vector3));
			root.MoreProps.Set("Vector", vector);
			sys.Set("Vector",	vector);
			sys.Set("Vector3",	vector);
			sys.Set("Vector4",	root.AddType(typeof(UE.Vector4)));
			if (system == null)
				root.BaseProps.Set("System", new SimpleObject(root.Engine, sys));

			// safe types next (TODO: ref-count or bind to engine to dispose with engine reset)
			root.BaseProps.Set("UI", new Value(engine =>
			new SimpleObject(engine, new Properties()
			{
				{ "Anchors",        root[typeof(UI.Anchors)] },
				{ "Element",        root[typeof(UI.Element)] },
				{ "Panel",          root[typeof(UI.Panel)] },
				{ "Window",         root[typeof(UI.Window)] },
				{ "Label",          root[typeof(UI.Label)] },
				{ "Button",         root[typeof(UI.Button)] },
			})));
			root.Set("Window",	root[typeof(UI.Window)]);
			root.Set("Label",	root[typeof(UI.Label)]);
			root.Set("Button",	root[typeof(UI.Button)]);

			// things that are dangerous in immediate / REPL mode
			if (!repl)
			{
				// definitely dangerous, IMGUI is not for REPL
				root.BaseProps.Set("IMGUI", new Value(engine =>
				root[typeof(UE.GUI)] = new ReflectedType(engine, typeof(UE.GUI), new Properties()
				{
					{ "GUISkin",        root[typeof(UE.GUISkin)] },
					{ "GUIStyle",       root[typeof(UE.GUIStyle)] },
					{ "GUIStyleState",  root[typeof(UE.GUIStyleState)] },
					{ "GUIContent",     root[typeof(UE.GUIContent)] },
					{ "GUIElement",     root[typeof(UE.GUIElement)] },
					{ "GUILayer",       root[typeof(UE.GUILayer)] },
					{ "GUILayout",      root[typeof(UE.GUILayout)] },
					{ "GUIText",        root[typeof(UE.GUIText)] },
					{ "GUIUtility",     root[typeof(UE.GUIUtility)] },
				})));

				// potentionally dangerous (could stay without a way to destroy)
				root.BaseProps.Set("Unity", new Value(engine =>
				new SimpleObject(engine, new Properties()
				{
					{ "Object",         root[typeof(UE.Object)] },
					{ "GameObject",     root[typeof(UE.GameObject)] },
					{ "Canvas",         root[typeof(UE.Canvas)] },
					{ "CanvasGroup",    root[typeof(UE.CanvasGroup)] },
					{ "RectTransform",  root[typeof(UE.RectTransform)] },
					{ "LayerMask",      root[typeof(UE.LayerMask)] },

					{ "DefaultControls",root[typeof(UUI.DefaultControls)] },
					{ "GridLayout",		root[typeof(UUI.GridLayoutGroup)] },
					{ "HorizontalLayout",root[typeof(UUI.HorizontalLayoutGroup)] },
					{ "VerticalLayout",	root[typeof(UUI.VerticalLayoutGroup)] },
					{ "LayoutRebuilder",root[typeof(UUI.LayoutRebuilder)] },
					{ "LayoutUtility",	root[typeof(UUI.LayoutUtility)] },
					{ "ContentSizeFitter", root[typeof(UUI.ContentSizeFitter)] },
					{ "AspectRatioFitter", root[typeof(UUI.AspectRatioFitter)] },
					{ "SizeFitter",		root[typeof(UUI.ContentSizeFitter)] },
					{ "RatioFitter",	root[typeof(UUI.AspectRatioFitter)] },

					{ "Text",           root[typeof(UUI.Text)] },
					{ "Button",         root[typeof(UUI.Button)] },
					{ "Image",          root[typeof(UUI.Image)] },
					{ "RawImage",       root[typeof(UUI.RawImage)] },

					{ "Screen",			root[typeof(UE.Screen)] },
					{ "Sprite",         root[typeof(UE.Sprite)] },
					{ "Texture",        root[typeof(UE.Texture)] },
					{ "Texture2D",      root[typeof(UE.Texture2D)] },

					{ "UIMaster",       root[typeof(KUI.UIMasterController)] },
					{ "UISkinDef",      root[typeof(UISkinDef)] },
					{ "UISkinManager",  root[typeof(UISkinManager)] },
					{ "UIStyle",        root[typeof(UIStyle)] },
					{ "UIStyleState",   root[typeof(UIStyleState)] },
				})));

				// potentionally dangerous (who the heck knows, we need our own safe API)
				root.BaseProps.Set("KSP", new Value(engine =>
				new SimpleObject(engine, new Properties()
				{
					{ "Vessel",         root[typeof(Vessel)] },
					{ "FlightGlobals",  root[typeof(FlightGlobals)] },
					{ "FlightCtrlState", root[typeof(FlightCtrlState)] },
					{ "StageManager",   root[typeof(KUI.Screens.StageManager)] },
				})));
			}
		}
	}
	/// <summary>
	/// Limited engine whith what is safe in REPL / Immediate Mode
	/// </summary>
	public class ImmediateEngine : Engine
	{
		public ImmediateEngine()
			: base(engine => new RuntimeRoot(engine, root =>
			RuntimeEngine.FillRoot(root, repl: true))) { }
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion.REPL] " + msg);
	}

	public class RuntimeRoot : BasicRoot
	{
		protected Action<IEngineRoot> FillMe;
		protected override void Fill()
		{
			base.Fill();
			FillMe?.Invoke(this);
		}
		public RuntimeRoot(IEngine engine, Action<IEngineRoot> fill)
			: base(engine, fill: false)
		{
			FillMe = fill;
			Fill();
		}
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

using System;
using System.Collections.Generic;
using KSP.UI;
using KSP.UI.Screens;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.Completion;
using RedOnion.Script.ReflectedObjects;
using RedOnion.UI;
using UnityEngine;
using UnityEngine.UI;

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
			=> Debug.Log("[RedOnion] " + msg);

		public static void FillRoot(IEngineRoot root, bool repl)
		{
			// neutral types first
			root.AddType(typeof(System.Delegate));
			root.AddType(typeof(UnityEngine.Debug));
			root.AddType(typeof(UnityEngine.Color));
			root.AddType(typeof(UnityEngine.Rect));
			root.AddType(typeof(UnityEngine.Vector2));
			root.AddType(typeof(UnityEngine.Vector3));
			root.AddType(typeof(UnityEngine.Vector4));

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

			// things that are dangerous in immediate / REPL mode
			if (!repl)
			{
				// definitely dangerous, IMGUI is not for REPL
				root.BaseProps.Set("IMGUI", new Value(engine =>
				root[typeof(GUI)] = new ReflectedType(engine, typeof(UnityEngine.GUI), new Properties()
				{
					{ "GUISkin",        root[typeof(UnityEngine.GUISkin)] },
					{ "GUIStyle",       root[typeof(UnityEngine.GUIStyle)] },
					{ "GUIStyleState",  root[typeof(UnityEngine.GUIStyleState)] },
					{ "GUIContent",     root[typeof(UnityEngine.GUIContent)] },
					{ "GUIElement",     root[typeof(UnityEngine.GUIElement)] },
					{ "GUILayer",       root[typeof(UnityEngine.GUILayer)] },
					{ "GUILayout",      root[typeof(UnityEngine.GUILayout)] },
					{ "GUIText",        root[typeof(UnityEngine.GUIText)] },
					{ "GUIUtility",     root[typeof(UnityEngine.GUIUtility)] },
				})));

				// potentionally dangerous (could stay without a way to destroy)
				root.BaseProps.Set("Unity", new Value(engine =>
				new SimpleObject(engine, new Properties()
				{
					{ "DefaultControls", root[typeof(UnityEngine.UI.DefaultControls)] },
					{ "Object",         root[typeof(UnityEngine.Object)] },
					{ "GameObject",     root[typeof(UnityEngine.GameObject)] },
					{ "Canvas",         root[typeof(UnityEngine.Canvas)] },
					{ "CanvasGroup",    root[typeof(UnityEngine.CanvasGroup)] },
					{ "RectTransform",  root[typeof(UnityEngine.RectTransform)] },
					{ "LayerMask",      root[typeof(UnityEngine.LayerMask)] },
					{ "Text",           root[typeof(UnityEngine.UI.Text)] },
					{ "Button",         root[typeof(UnityEngine.UI.Button)] },
					{ "Image",          root[typeof(UnityEngine.UI.Image)] },
					{ "RawImage",       root[typeof(UnityEngine.UI.RawImage)] },
					{ "Sprite",         root[typeof(UnityEngine.Sprite)] },
					{ "Texture",        root[typeof(UnityEngine.Texture)] },
					{ "Texture2D",      root[typeof(UnityEngine.Texture2D)] },
					{ "Renderer",       root[typeof(UnityEngine.Renderer)] },

					{ "Master",         root[typeof(UIMasterController)] },
					{ "UIMasterController", root[typeof(UIMasterController)] },
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
					{ "StageManager",   root[typeof(StageManager)] },
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
			=> Debug.Log("[RedOnion.REPL] " + msg);
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
			=> Debug.Log("[RedOnion.DOC] " + msg);
	}
	/// <summary>
	/// Engine designed to provide hints and documentation for REPL / Immediate Mode
	/// </summary>
	public class ReplHintsEngine : CompletionEngine
	{
		public ReplHintsEngine(ImmediateEngine engine)
			: base(engine) { }
		public override void Log(string msg)
			=> Debug.Log("[RedOnion.ReplDOC] " + msg);
	}
}

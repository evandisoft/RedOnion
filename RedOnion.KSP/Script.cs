using System;
using System.Collections.Generic;
using KSP.UI;
using KSP.UI.Screens;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
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
			: base(engine => new EngineRoot(engine, EngineRoot.RootKind.Runtime)) { }
		public override void Log(string msg)
			=> Debug.Log("[RedOnion] " + msg);
	}
	/// <summary>
	/// Limited engine whith what is safe in REPL / Immediate Mode
	/// </summary>
	public class ImmediateEngine : Engine
	{
		public ImmediateEngine()
			: base(engine => new EngineRoot(engine, EngineRoot.RootKind.Immediate)) { }
		public override void Log(string msg)
			=> Debug.Log("[RedOnion.REPL] " + msg);
	}
	/// <summary>
	/// Engine designed to provide hints and documentation for runtime engine
	/// </summary>
	public class DocumentingEngine : Engine
	{
		public DocumentingEngine()
			: base(engine => new EngineRoot(engine, EngineRoot.RootKind.Completion))
			=> Options = Options | EngineOption.Silent;
		protected DocumentingEngine(Func<IEngine, IEngineRoot> createRoot)
			: base(createRoot)
			=> Options = Options | EngineOption.Silent;
		public override void Log(string msg)
			=> Debug.Log("[RedOnion.DOC] " + msg);
		public virtual IList<string> Complete(string source, int at)
			=> new string[0];
		public virtual string Documentation(string source, int at)
			=> "TODO";
	}
	/// <summary>
	/// Engine designed to provide hints and documentation for REPL / Immediate Mode
	/// </summary>
	public class ReplHintsEngine : DocumentingEngine
	{
		public ReplHintsEngine()
			: base(engine => new EngineRoot(engine, EngineRoot.RootKind.ReplCompletion)) { }
		public override void Log(string msg)
			=> Debug.Log("[RedOnion.ReplDOC] " + msg);
	}
	public class EngineRoot : Root
	{
		public enum RootKind
		{
			Runtime,
			Immediate,
			Completion,
			ReplCompletion
		}
		public RootKind Kind { get; }
		public EngineRoot(IEngine engine, RootKind kind)
			: base(engine, fill: false)
		{
			Kind = kind;
			Fill();
		}

		protected override void Fill()
		{
			// neutral types first
			AddType(typeof(System.Delegate));
			AddType(typeof(UnityEngine.Debug));
			AddType(typeof(UnityEngine.Color));
			AddType(typeof(UnityEngine.Rect));
			AddType(typeof(UnityEngine.Vector2));
			AddType(typeof(UnityEngine.Vector3));
			AddType(typeof(UnityEngine.Vector4));

			// safe types next (TODO: ref-count or bind to engine to dispose with engine reset)
			BaseProps.Set("UI", new Value(engine =>
			new SimpleObject(engine, new Properties()
			{
				{ "Anchors",        this[typeof(UI.Anchors)] },
				{ "Element",        this[typeof(UI.Element)] },
				{ "Panel",          this[typeof(UI.Panel)] },
				{ "Window",         this[typeof(UI.Window)] },
				{ "Label",          this[typeof(UI.Label)] },
				{ "Button",         this[typeof(UI.Button)] },
			})));

			// things that are dangerous in immediate / REPL mode
			if (Kind == RootKind.Runtime /* TODO: || Kind == RootKind.Completion */)
			{
				// definitely dangerous, IMGUI is not for REPL
				BaseProps.Set("IMGUI", new Value(engine =>
				this[typeof(GUI)] = new ReflectedType(engine, typeof(UnityEngine.GUI), new Properties()
				{
					{ "GUISkin",        this[typeof(UnityEngine.GUISkin)] },
					{ "GUIStyle",       this[typeof(UnityEngine.GUIStyle)] },
					{ "GUIStyleState",  this[typeof(UnityEngine.GUIStyleState)] },
					{ "GUIContent",     this[typeof(UnityEngine.GUIContent)] },
					{ "GUIElement",     this[typeof(UnityEngine.GUIElement)] },
					{ "GUILayer",       this[typeof(UnityEngine.GUILayer)] },
					{ "GUILayout",      this[typeof(UnityEngine.GUILayout)] },
					{ "GUIText",        this[typeof(UnityEngine.GUIText)] },
					{ "GUIUtility",     this[typeof(UnityEngine.GUIUtility)] },
				})));

				// potentionally dangerous (could stay without a way to destroy)
				BaseProps.Set("Unity", new Value(engine =>
				new SimpleObject(engine, new Properties()
				{
					{ "DefaultControls", this[typeof(UnityEngine.UI.DefaultControls)] },
					{ "Object",         this[typeof(UnityEngine.Object)] },
					{ "GameObject",     this[typeof(UnityEngine.GameObject)] },
					{ "Canvas",         this[typeof(UnityEngine.Canvas)] },
					{ "CanvasGroup",    this[typeof(UnityEngine.CanvasGroup)] },
					{ "RectTransform",  this[typeof(UnityEngine.RectTransform)] },
					{ "LayerMask",      this[typeof(UnityEngine.LayerMask)] },
					{ "Text",           this[typeof(UnityEngine.UI.Text)] },
					{ "Button",         this[typeof(UnityEngine.UI.Button)] },
					{ "Image",          this[typeof(UnityEngine.UI.Image)] },
					{ "RawImage",       this[typeof(UnityEngine.UI.RawImage)] },
					{ "Sprite",         this[typeof(UnityEngine.Sprite)] },
					{ "Texture",        this[typeof(UnityEngine.Texture)] },
					{ "Texture2D",      this[typeof(UnityEngine.Texture2D)] },
					{ "Renderer",       this[typeof(UnityEngine.Renderer)] },

					{ "Master",         this[typeof(UIMasterController)] },
					{ "UIMasterController", this[typeof(UIMasterController)] },
					{ "UISkinDef",      this[typeof(UISkinDef)] },
					{ "UISkinManager",  this[typeof(UISkinManager)] },
					{ "UIStyle",        this[typeof(UIStyle)] },
					{ "UIStyleState",   this[typeof(UIStyleState)] },
				})));

				// potentionally dangerous (who the heck knows, we need our own safe API)
				BaseProps.Set("KSP", new Value(engine =>
				new SimpleObject(engine, new Properties()
				{
					{ "Vessel",         this[typeof(Vessel)] },
					{ "FlightGlobals",  this[typeof(FlightGlobals)] },
					{ "FlightCtrlState", this[typeof(FlightCtrlState)] },
					{ "StageManager",   this[typeof(StageManager)] },
				})));
			}
		}
	}
}

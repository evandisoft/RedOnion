using System;
using KSP.UI;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using UnityEngine;
using UnityEngine.UI;

namespace RedOnion
{
	public class ScriptEngine : Engine
	{
		public ScriptEngine() : base(engine => new EngineRoot(engine)) { }
		public override void Log(string msg)
			=> Debug.Log("[RedOnion] " + msg);
	}
	public class EngineRoot : Root
	{
		public EngineRoot(Engine engine) : base(engine) { }

		protected override void Fill()
		{
			AddType(typeof(Debug));
			AddType(typeof(Delegate));
			AddType(typeof(Color));
			AddType(typeof(Rect));
			AddType(typeof(Vector2));
			AddType(typeof(Vector3));
			AddType(typeof(Vector4));

			BaseProps.Set("IMGUI", new Value(engine =>
			{
				var gui = new ReflectedType(engine, typeof(GUI), new Properties()
				{
					{ "GUISkin",		this[typeof(GUISkin)] },
					{ "GUIStyle",		this[typeof(GUIStyle)] },
					{ "GUIStyleState",	this[typeof(GUIStyleState)] },
					{ "GUIContent",		this[typeof(GUIContent)] },
					{ "GUIElement",		this[typeof(GUIElement)] },
					{ "GUILayer",		this[typeof(GUILayer)] },
					{ "GUILayout",		this[typeof(GUILayout)] },
					{ "GUIText",		this[typeof(GUIText)] },
					{ "GUIUtility",		this[typeof(GUIUtility)] },
				});
				this[typeof(GUI)] = gui;
				return gui;
			}));

			BaseProps.Set("Unity", new Value(engine =>
			{
				return new SimpleObject(engine, new Properties()
				{
					{ "DefaultControls", this[typeof(DefaultControls)] },
					{ "UISkinDef",		this[typeof(UISkinDef)] },
					{ "UISkinManager",	this[typeof(UISkinManager)] },
					{ "UIStyle",		this[typeof(UIStyle)] },
					{ "UIStyleState",	this[typeof(UIStyleState)] },
					{ "Object",         this[typeof(UnityEngine.Object)] },
					{ "GameObject",		this[typeof(GameObject)] },
					{ "Canvas",			this[typeof(Canvas)] },
					{ "CanvasGroup",	this[typeof(CanvasGroup)] },
					{ "RectTransform",	this[typeof(RectTransform)] },
					{ "LayerMask",		this[typeof(LayerMask)] },
					{ "Text",			this[typeof(Text)] },
					{ "Button",			this[typeof(Button)] },
					{ "Image",			this[typeof(Image)] },
					{ "RawImage",		this[typeof(RawImage)] },
					{ "Sprite",			this[typeof(Sprite)] },
					{ "Texture",		this[typeof(Texture)] },
					{ "Texture2D",		this[typeof(Texture2D)] },
					{ "Renderer",		this[typeof(Renderer)] },
					{ "Master",         this[typeof(UIMasterController)] },
				});
			}));

			BaseProps.Set("UI", new Value(engine =>
			{
				return new SimpleObject(engine, new Properties()
				{
					{ "Element",		this[typeof(UI.Element)] },
					{ "Panel",			this[typeof(UI.Panel)] },
					{ "Window",         this[typeof(UI.Window)] },
					{ "Label",			this[typeof(UI.Label)] },
					{ "Button",         this[typeof(UI.Button)] },
				});
			}));

			BaseProps.Set("KSP", new Value(engine =>
			{
				var ksp = new SimpleObject(engine, new Properties()
				{
					{ "Vessel",			this[typeof(Vessel)] },
					{ "FlightGlobals",	this[typeof(FlightGlobals)] },
					{ "FlightCtrlState", this[typeof(FlightCtrlState)] },
				});
				return ksp;
			}));
		}
	}
}

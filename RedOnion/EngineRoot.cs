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
					{ "GUISkin",		new Value(this[typeof(GUISkin)]) },
					{ "GUIStyle",		new Value(this[typeof(GUIStyle)]) },
					{ "GUIStyleState",	new Value(this[typeof(GUIStyleState)]) },
					{ "GUIContent",		new Value(this[typeof(GUIContent)]) },
					{ "GUIElement",		new Value(this[typeof(GUIElement)]) },
					{ "GUILayer",		new Value(this[typeof(GUILayer)]) },
					{ "GUILayout",		new Value(this[typeof(GUILayout)]) },
					{ "GUIText",		new Value(this[typeof(GUIText)]) },
					{ "GUIUtility",		new Value(this[typeof(GUIUtility)]) },
				});
				this[typeof(GUI)] = gui;
				return gui;
			}));

			BaseProps.Set("Unity", new Value(engine =>
			{
				var unity = new SimpleObject(engine, new Properties()
				{
					{ "DefaultControls", new Value(this[typeof(DefaultControls)]) },
					{ "UISkinDef",		new Value(this[typeof(UISkinDef)]) },
					{ "UISkinManager",	new Value(this[typeof(UISkinManager)]) },
					{ "UIStyle",		new Value(this[typeof(UIStyle)]) },
					{ "UIStyleState",	new Value(this[typeof(UIStyleState)]) },
					{ "Object",         new Value(this[typeof(UnityEngine.Object)]) },
					{ "GameObject",		new Value(this[typeof(GameObject)]) },
					{ "Canvas",			new Value(this[typeof(Canvas)]) },
					{ "CanvasGroup",	new Value(this[typeof(CanvasGroup)]) },
					{ "RectTransform",	new Value(this[typeof(RectTransform)]) },
					{ "LayerMask",		new Value(this[typeof(LayerMask)]) },
					{ "Text",			new Value(this[typeof(Text)]) },
					{ "Button",			new Value(this[typeof(Button)]) },
					{ "Image",			new Value(this[typeof(Image)]) },
					{ "RawImage",		new Value(this[typeof(RawImage)]) },
					{ "Sprite",			new Value(this[typeof(Sprite)]) },
					{ "Texture",		new Value(this[typeof(Texture)]) },
					{ "Texture2D",		new Value(this[typeof(Texture2D)]) },
					{ "Renderer",		new Value(this[typeof(Renderer)]) },
					{ "Master",         new Value(this[typeof(UIMasterController)]) },
				});
				return unity;
			}));

			BaseProps.Set("KSP", new Value(engine =>
			{
				var ksp = new SimpleObject(engine, new Properties()
				{
					{ "Vessel",			new Value(this[typeof(Vessel)]) },
					{ "FlightGlobals",	new Value(this[typeof(FlightGlobals)]) },
					{ "FlightCtrlState", new Value(this[typeof(FlightCtrlState)]) },
				});
				return ksp;
			}));
		}
	}
}

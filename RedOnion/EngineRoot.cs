using System;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using UnityEngine;
using UnityEngine.UI;

namespace RedOnion
{
	public class EngineRoot : Root
	{
		public EngineRoot(Engine engine) : base(engine) { }

		protected override void Fill()
		{
			AddType("Color", typeof(Color));
			AddType("Rect", typeof(Rect));
			AddType("Vector2", typeof(Vector2));
			AddType("Vector3", typeof(Vector3));
			AddType("GUI", typeof(GUI));
			AddType("GUISkin", typeof(GUISkin));
			AddType("GUIStyle", typeof(GUIStyle));
			AddType("GUIStyleState", typeof(GUIStyleState));
			AddType("GUIContent", typeof(GUIContent));
			AddType("GUIElement", typeof(GUIElement));
			AddType("GUILayer", typeof(GUILayer));
			AddType("GUILayout", typeof(GUILayout));
			AddType("GUIText", typeof(GUIText));
			AddType("GUIUtility", typeof(GUIUtility));
		}
	}
}

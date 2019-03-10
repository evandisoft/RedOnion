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
			AddType(typeof(Debug));
			AddType(typeof(Color));
			AddType(typeof(Rect));
			AddType(typeof(Vector2));
			AddType(typeof(Vector3));
			AddType(typeof(GUI));
			AddType(typeof(GUISkin));
			AddType(typeof(GUIStyle));
			AddType(typeof(GUIStyleState));
			AddType(typeof(GUIContent));
			AddType(typeof(GUIElement));
			AddType(typeof(GUILayer));
			AddType(typeof(GUILayout));
			AddType(typeof(GUIText));
			AddType(typeof(GUIUtility));
		}
	}
}

using System;
using KSP.UI;
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
			AddType(typeof(Vector4));

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

			AddType(typeof(DefaultControls));
			AddType(typeof(UISkinDef));
			AddType(typeof(UISkinManager));
			AddType(typeof(UIStyle));
			AddType(typeof(UIStyleState));
			AddType("UnityObject", typeof(UnityEngine.Object));
			AddType(typeof(GameObject));
			AddType(typeof(GameObjectExtension));
			AddType(typeof(Canvas));
			AddType(typeof(CanvasGroup));
			AddType(typeof(RectTransform));
			AddType(typeof(LayerMask));
			AddType(typeof(Text));
			AddType(typeof(Button));
			AddType(typeof(Image));
			AddType(typeof(RawImage));
			AddType(typeof(Sprite));
			AddType(typeof(Texture));
			AddType(typeof(Texture2D));

			AddType(typeof(UIMasterController));
			AddType(typeof(FlightCtrlState));
			AddType(typeof(FlightGlobals));
			AddType(typeof(Vessel));
		}
	}
}

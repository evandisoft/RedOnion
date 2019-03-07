using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class GUIUtil {
		/// <summary>
		/// Checks whether the mouse is currently in the given local rect coordinates
		/// using the less buggy Mouse.screenPos rather than Event.current.mousePosition
		/// This is only relevant if called in Render
		/// </summary>
		/// <param name="rect">Rect.</param>
		public static bool MouseInRect(Rect rect)
		{
			Rect absoluteRect = new Rect();
			Vector2 absoluteRectStart = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
			absoluteRect.x = absoluteRectStart.x;
			absoluteRect.y = absoluteRectStart.y;
			absoluteRect.width = rect.width;
			absoluteRect.height = rect.height;

			//Debug.Log($"{absoluteRect.Contains(Mouse.screenPos)},{Mouse.screenPos},{absoluteRect}");
			return absoluteRect.Contains(Mouse.screenPos);
		}

		static Font monoSpaceFont = null;
		static public Font GetMonoSpaceFont()
		{
			if (monoSpaceFont == null) {
				string[] fonts = Font.GetOSInstalledFontNames();
				foreach (var fontName in fonts) {
					if (fontName.Contains("Mono")) {
						monoSpaceFont = Font.CreateDynamicFontFromOSFont(fontName, 12);
					}
				}
			}
			return monoSpaceFont;
		}
	}
}

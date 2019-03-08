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
					// Accept Courier New if available
					if (fontName=="Courier New") {
						monoSpaceFont = Font.CreateDynamicFontFromOSFont(fontName, 12);
						return monoSpaceFont;
					}
					// Accept the last listed Mono font if Courier New is not available
					else if (fontName.Contains("Mono"))
					{
						monoSpaceFont= Font.CreateDynamicFontFromOSFont(fontName, 12);
					}
				}
			}
			return monoSpaceFont;
		}

		static bool consumeNextCharEvent;
		/// <summary>
		/// Consumes the current event, assumed to be a keycode event
		/// and marks any followup character event to also be consumed.
		/// </summary>
		/// <param name="event1">Event1.</param>
		static public void ConsumeAndMarkNextCharEvent(Event event1)
		{
			if (event1.keyCode != KeyCode.None) {
				event1.Use();
				consumeNextCharEvent = true;
			}
		}
		static public void ConsumeMarkedCharEvent(Event event1)
		{
			if (consumeNextCharEvent && event1.keyCode == KeyCode.None) {
				event1.Use();
			}
			consumeNextCharEvent = false;
		}
	}
}

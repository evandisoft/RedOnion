using System;
using System.Collections.Generic;
using Kerbalui.Controls;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;
using Kerbalui.Types;
using Kerbalui.Util;
using LiveRepl.Interfaces;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl.Parts
{
	/// <summary>
	///  For windows, Consolas, and "Courier New"
	/// </summary>
	public class FontSelector : EditingArea, ICompletableElement
	{
		public Font CurrentFont { get; private set; } = null;

		ScriptWindowParts uiparts;

		public FontSelector(ScriptWindowParts uiparts) : base(new TextField())
		{
			this.uiparts=uiparts;

			var defaultFontName=GUILibUtil.GetMonoSpaceFontName();
			//Debug.Log("defaultFontName is "+defaultFontName);
			var currentFontName=SavedSettings.LoadSetting("fontname",defaultFontName);
			//Debug.Log("currentFontName is "+currentFontName);
			if (currentFontName=="")
			{
				CurrentFont=GUI.skin.font;
				currentFontName="Default Font";
			}
			else
			{
				CurrentFont=Font.CreateDynamicFontFromOSFont(currentFontName, 14);
			}

			Text=currentFontName;
			keybindings.Clear();
			onlyUseKeyBindings=true;

			//uiparts.FontChange+=editableText.FontChangeEventHandler;
		}

		public void Complete(int index)
		{
			var completionContent = GetCompletionContent(out int replaceStart,out int replaceEnd);
			if (completionContent.Count > index)
			{
				Text = completionContent[index];
				SelectIndex=CursorIndex = Text.Length;
			}
			var newFont=Font.CreateDynamicFontFromOSFont(Text,14);
			if (newFont!=null)
			{
				CurrentFont=newFont;
				uiparts.ChangeFont(CurrentFont);
			}
			SavedSettings.SaveSetting("fontname",Text);
		}

		public IList<string> GetCompletionContent(out int replaceStart, out int replaceEnd)
		{
			replaceStart=0;
			replaceEnd=Text.Length;
			return Font.GetOSInstalledFontNames();
		}

		protected override void DecoratorUpdate()
		{
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
			{
				//Text="";
				GrabFocus();
			}
			base.DecoratorUpdate();
		}
	}
}

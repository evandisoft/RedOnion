using System;
using System.Collections.Generic;
using Kerbalui;
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
		public string CurrentFontname { get; private set; } = "";

		ScriptWindowParts uiparts;

		public FontSelector(ScriptWindowParts uiparts) : base(new TextField())
		{
			this.uiparts=uiparts;

			var defaultFontName=GUILibUtil.GetMonoSpaceFontName();
			//Debug.Log("defaultFontName is "+defaultFontName);
			var currentFontName=SavedSettings.LoadSetting("fontname",defaultFontName);
			//Debug.Log("currentFontName is "+currentFontName);
			CurrentFontname=currentFontName;

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
			CurrentFontname=Text;

			uiparts.ChangeFont(CurrentFontname,KerbaluiSettings.DefaultFontsize);
			SavedSettings.SaveSetting("fontname", CurrentFontname);
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

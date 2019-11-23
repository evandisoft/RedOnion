using System;
using System.Collections.Generic;
using Kerbalui.Controls;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;
using Kerbalui.Types;
using Kerbalui.Util;
using LiveRepl.Interfaces;
using UnityEngine;

namespace LiveRepl.Parts
{
	/// <summary>
	///  For windows, Consolas, and "Courier New"
	/// </summary>
	public class FontSelector : EditingArea, ICompletableElement
	{
		public Font CurrentFont { get; private set; } = null;
		public event Action<Font> FontChange;

		public FontSelector() : base(new TextField())
		{
			var currentFontName=GUILibUtil.GetMonoSpaceFontName();
			if (currentFontName=="")
			{
				CurrentFont=Window.defaultSkin.font;
			}
			else
			{
				CurrentFont=Font.CreateDynamicFontFromOSFont(currentFontName, 14);
			}

			FontChange(CurrentFont);
			//Text=CurrentFont.
			keybindings.Clear();
			onlyUseKeyBindings=true;
		}

		public string ControlName => editableText.ControlName;

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
				FontChange(CurrentFont);
			}
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
				Text="";
				GrabFocus();
			}
			base.DecoratorUpdate();
		}
	}
}

using System;
using Kerbalui.Types;
using Kerbalui.Util;
using UnityEngine;

namespace Kerbalui.Controls.Abstract
{
	/// <summary>
	/// A Control that contains content.
	/// </summary>
	public abstract class ContentControl:Control
	{
		public GUIContent Content { get; set; }=new GUIContent("");
		public GUIStyle Style { get; set; }
		public string Fontname=KerbaluiSettings.DefaultFontname;
		private int Fontsize=KerbaluiSettings.DefaultFontsize;
		public int ScaledFontsize => (int)(KerbaluiSettings.UI_SCALE*Fontsize);

		void SettingsChangeHandler()
		{
			UpdateFont();
		}

		protected ContentControl(GUIStyle style)
		{
			Style=new GUIStyle(style);
			Style.fontSize=ScaledFontsize;
			//UpdateFont();
			KerbaluiSettings.SettingsChange+=SettingsChangeHandler;
		}

		void UpdateFont()
		{
			Style.fontSize=ScaledFontsize;
		}

		public void FontChangeEventHandler(string fontname,int fontsize)
		{
			Fontname=fontname;
			Fontsize=fontsize;
			Style.font=GUILibUtil.GetFont(fontname, fontsize);
			UpdateFont();
		}

		public override Vector2 MinSize
		{
			get
			{
				Vector2 minSize=Style.CalcSize(Content);
				if (Content.text=="")
				{
					Content.text="a";
					minSize.y=Style.CalcSize(Content).y;
					Content.text="";
				}
				//minSize.y+=5;
				return minSize;
			}
		}
	}
}

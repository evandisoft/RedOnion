using System;
using Kerbalui.Types;
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

		protected ContentControl(GUIStyle style)
		{
			Style=new GUIStyle(style);
		}

		public void FontChangeEventHandler(Font font)
		{
			Style.font=font;
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
				minSize.y+=5;
				return minSize;
			}
		}
	}
}

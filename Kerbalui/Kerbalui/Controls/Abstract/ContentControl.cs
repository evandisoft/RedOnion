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
		public GUIContent content=new GUIContent("");
		public GUIStyle style;

		public override Vector2 MinSize => StyleOrDefault.CalcSize(content);

		/// <summary>
		/// Returns default style if style is null
		/// </summary>
		/// <value>The style.</value>
		public GUIStyle StyleOrDefault
		{
			get
			{
				if (style==null)
				{
					return DefaultStyle();
				}
				return style;
			}
		}

		/// <summary>
		/// Subclasses implement this to return the default style they want to be returned from 
		/// StyleOrDefault when style is null.
		/// </summary>
		/// <returns>The style.</returns>
		public abstract GUIStyle DefaultStyle();

		public void InitStyle()
		{
			if (style==null)
			{
				DefaultStyle();
			}
		}


	}
}

using System;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Base
{
	/// <summary>
	/// A control that displays content.
	/// </summary>
	public abstract class ContentControl:Control
	{
		public GUIContent content=new GUIContent("");
		public GUIStyle style;

		public abstract void SetDefaultStyle();
		public void InitStyle()
		{
			if (style==null)
			{
				SetDefaultStyle();
			}
		}
	}
}

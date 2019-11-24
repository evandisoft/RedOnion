using System;
using UnityEngine;

namespace Kerbalui.Controls.Abstract
{
	/// <summary>
	/// A Control that contains text that is user-editable
	/// </summary>
	public abstract class EditableText:ContentControl
	{
		protected EditableText(GUIStyle style) : base(style)
		{
		}

		public virtual string Text { get => Content.text; set => Content.text=value; }
	}
}

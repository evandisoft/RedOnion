using System;
namespace Kerbalui.Controls.Abstract
{
	/// <summary>
	/// A Control that contains text that is user-editable
	/// </summary>
	public abstract class EditableText:ContentControl
	{
		public virtual string Text { get => content.text; set => content.text=value; }
	}
}

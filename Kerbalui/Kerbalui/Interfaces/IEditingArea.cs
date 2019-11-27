using System;
using Kerbalui.EditingChanges;

namespace Kerbalui.Interfaces
{
	public interface IEditingArea
	{
		string ControlName { get; }
		bool ReceivedInput { get; }

		string Text { get; set; }
		int CursorIndex { get; set; }

		void GrabFocus();
		bool HasFocus();
	}
}

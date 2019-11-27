using System;
namespace Kerbalui.Interfaces
{
	public interface IEditingArea
	{
		string ControlName { get; }
		bool ReceivedInput { get; }
		int CursorIndex { get; set; }
		string Text { get; set; }
		int SelectIndex { get; set; }

		void GrabFocus();
		bool HasFocus();
	}
}

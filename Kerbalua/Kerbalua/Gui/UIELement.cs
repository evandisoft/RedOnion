using System;
namespace Kerbalua.Gui {
	public class UIElement {
		static long NextID = 0;
		public readonly string ControlName = "Control-"+NextID++;
	}
}

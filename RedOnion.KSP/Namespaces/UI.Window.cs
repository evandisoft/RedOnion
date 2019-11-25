using System;
using RedOnion.KSP.API;
using RedOnion.KSP.OS;
using RedOnion.KSP.ROS;
using RedOnion.KSP.Attributes;
using RedOnion.ROS;

namespace RedOnion.KSP
{
	[DocBuild(AsType = typeof(UI.Window))]
	public class UI_Window : UI.Window
	{
		protected Process.ShutdownHook _hooks;

		public UI_Window()
			: this(null, UI.Layout.Vertical) {}
		public UI_Window(UI.Layout layout, string title = null)
			: this(title, layout) { }
		public UI_Window(string title, UI.Layout layout = UI.Layout.Vertical)
			: base(title, layout)
		{
			Value.DebugLog("Creating new window in process #{0}", Process.current.id);
			_hooks = new Process.ShutdownHook(this);
		}

		protected override void Dispose(bool disposing)
		{
			Value.DebugLog("Disposing window (dispose: {0}, process: {1})", disposing, _hooks?.process.id ?? 0);
			_hooks?.Dispose();
			_hooks = null;
			base.Dispose(disposing);
		}
	}
}

using System;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

namespace RedOnion.KSP.API_UI
{
	public class Window : UI.Window
	{
		WeakReference core;
		public Window(IProcessor core)
			: this(core, null, UI.Layout.Vertical) {}
		public Window(IProcessor core, UI.Layout layout)
			: this(core, null, layout) { }
		public Window(IProcessor core, string name = null, UI.Layout layout = UI.Layout.Vertical)
			: base(name, layout)
		{
			Value.DebugLog("Creating new window");
			if (core != null)
			{
				core.Shutdown += CoreShutdown;
				this.core = new WeakReference(core);
			}
		}
		bool CoreShutdown(IProcessor core)
		{
			Dispose();
			return false;
		}
		protected override void Dispose(bool disposing)
		{
			var core = this.core?.Target as IProcessor;
			Value.DebugLog("Disposing window (dispose: {0}, core: {1})", disposing, core != null);
			if (core != null)
				core.Shutdown -= CoreShutdown;
			if (disposing)
				this.core = null;
			base.Dispose(disposing);
		}
	}
}

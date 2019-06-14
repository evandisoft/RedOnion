using System;
using RedOnion.KSP.ROS;
using RedOnion.ROS;
using RedOnion.ROS.Objects;

namespace RedOnion.KSP.ROS_UI
{
	public class Window : UI.Window
	{
		WeakReference core;
		public Window(RosCore core, string name = null, UI.Layout layout = UI.Layout.Vertical)
			: base(name, layout)
		{
			core.Shutdown += CoreShutdown;
			this.core = new WeakReference(core);
		}
		bool CoreShutdown(Core core)
		{
			Dispose();
			return false;
		}
		protected override void Dispose(bool disposing)
		{
			var core = this.core.Target as RosCore;
			if (core != null)
				core.Shutdown -= CoreShutdown;
			if (disposing)
				this.core = null;
			base.Dispose(disposing);
		}
	}
}

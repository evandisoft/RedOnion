using System;
using RedOnion.Attributes;
using RedOnion.ROS;
using RedOnion.KSP.OS;

namespace RedOnion.KSP
{
	[DocBuild(AsType = typeof(UI.Window))]
	public class UI_Window : UI.Window
	{
		protected Process.ShutdownHook _hooks;

		/// <summary>
		/// Unique ID of the vector-draw.
		/// </summary>
		public ulong id { get; }
		protected static ulong id_counter;

		public UI_Window()
			: this(null, UI.Layout.Vertical) {}
		public UI_Window(UI.Layout layout, string title = null)
			: this(title, layout) { }
		public UI_Window(string title, UI.Layout layout = UI.Layout.Vertical)
			: base(title, layout)
		{
			id = ++id_counter;
			Value.DebugLog("Creating new UI.Window #{0} in process #{1}", id, Process.currentId);
			_hooks = new Process.ShutdownHook(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (_hooks != null)
			{
				Value.DebugLog("Disposing UI.Window #{0} in process #{1} (original: #{2}, dispose: {3})",
					id, Process.currentId, _hooks?.process.id ?? 0, disposing);
				var hooks = _hooks;
				_hooks = null;
				hooks.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}

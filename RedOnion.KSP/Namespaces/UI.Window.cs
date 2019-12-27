using System;
using RedOnion.Attributes;
using RedOnion.ROS;
using MunOS;

namespace RedOnion.KSP
{
	[DocBuild(AsType = typeof(UI.Window))]
	public class UI_Window : UI.Window
	{
		protected MunProcess.ShutdownHook _hooks;

		/// <summary>
		/// Unique ID of the vector-draw.
		/// </summary>
		public ulong id { get; }
		protected static ulong id_counter;

		public UI_Window() : this(null, UI.Layout.Vertical) {}
		public UI_Window(UI.Layout layout, string title = null, bool visible = true) : this(title, layout, visible) { }
		public UI_Window(string title, UI.Layout layout = UI.Layout.Vertical, bool visible = true) : base(layout, visible)
		{
			id = ++id_counter;
			Value.DebugLog("Creating new UI.Window #{0} in process #{1}", id, MunProcess.CurrentID);
			_hooks = new MunProcess.ShutdownHook(this);
			if (title != null)
				Title = title;
		}
		public UI_Window(bool visible, string title = null, UI.Layout layout = UI.Layout.Vertical) : this(title, layout, visible) { }
		public UI_Window(bool visible, UI.Layout layout, string title = null) : this(title, layout, visible) { }

		protected override void Dispose(bool disposing)
		{
			if (_hooks != null)
			{
				Value.DebugLog("Disposing UI.Window #{0} in process #{1} (original: #{2}, dispose: {3})",
					id, MunProcess.CurrentID, _hooks?.process.ID ?? MunID.Zero, disposing);
				var hooks = _hooks;
				_hooks = null;
				hooks.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}

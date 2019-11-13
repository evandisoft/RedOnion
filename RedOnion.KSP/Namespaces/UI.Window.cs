using System;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.KSP.Utilities;
using RedOnion.ROS;

namespace RedOnion.KSP
{
	[DocBuild(typeof(UI.Window))]
	public class Window : UI.Window
	{
		protected IProcessor _processor;
		public Window(IProcessor processor)
			: this(processor, null, UI.Layout.Vertical) {}
		public Window(IProcessor processor, UI.Layout layout, string title = null)
			: this(processor, title, layout) { }
		public Window(IProcessor processor, string title, UI.Layout layout = UI.Layout.Vertical)
			: base(title, layout)
		{
			Value.DebugLog("Creating new window");
			if (processor != null)
			{
				_processor = processor;
				_hooks = new Hooks(this);
			}
		}
		protected override void Dispose(bool disposing)
		{
			Value.DebugLog("Disposing window (dispose: {0}, processor: {1})", disposing, _processor != null);
			if (_hooks != null)
			{
				_hooks.Dispose();
				_hooks = null;
			}
			if (disposing)
				_processor = null;
			base.Dispose(disposing);
		}

		// this is to avoid direct hard-link from processor to window,
		// so that it can be garbage-collected when no direct link exists
		protected Hooks _hooks;
		protected class Hooks : IDisposable
		{
			protected WeakReference _window;
			protected IProcessor _processor;
			public Hooks(Window window)
			{
				_window = new WeakReference(window);
				_processor = window._processor;
				_processor.Shutdown += Shutdown;
			}
			~Hooks() => Dispose(false);
			public void Dispose()
			{
				GC.SuppressFinalize(this);
				Dispose(true);
			}
			protected virtual void Dispose(bool disposing)
			{
				if (_processor == null)
					return;
				_processor.Shutdown -= Shutdown;
				_processor = null;
				_window = null;
				
			}
			protected void Shutdown()
			{
				(_window?.Target as Window)?.Dispose();
			}
		}
	}
}

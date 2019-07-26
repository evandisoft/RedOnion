using System;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

namespace RedOnion.KSP
{
	public class Window : UI.Window
	{
		IProcessor _processor;
		public Window(IProcessor processor)
			: this(processor, null, UI.Layout.Vertical) {}
		public Window(IProcessor processor, UI.Layout layout)
			: this(processor, null, layout) { }
		public Window(IProcessor processor, string name = null, UI.Layout layout = UI.Layout.Vertical)
			: base(name, layout)
		{
			Value.DebugLog("Creating new window");
			if (processor != null)
			{
				_processor = processor;
				hooks = new Hooks(this);
			}
		}
		protected override void Dispose(bool disposing)
		{
			Value.DebugLog("Disposing window (dispose: {0}, processor: {1})", disposing, _processor != null);
			if (hooks != null)
			{
				hooks.Dispose();
				hooks = null;
			}
			if (disposing)
				_processor = null;
			base.Dispose(disposing);
		}

		// this is to avoid direct hard-link from processor to window,
		// so that it can be garbage-collected when no direct link exists
		protected Hooks hooks;
		protected class Hooks : IDisposable
		{
			protected WeakReference _window;
			public Hooks(Window window)
			{
				_window = new WeakReference(window);
				window._processor.Shutdown += Shutdown;
			}
			~Hooks() => Dispose(false);
			public void Dispose()
			{
				GC.SuppressFinalize(this);
				Dispose(true);
			}
			protected virtual void Dispose(bool disposing)
			{
				var window = _window?.Target as Window;
				if (window != null)
					return;
				_window = null;
				window._processor.Shutdown -= Shutdown;
			}
			protected bool Shutdown(IProcessor processor)
			{
				(_window?.Target as Window)?.Dispose();
				return false;
			}
		}
	}
}

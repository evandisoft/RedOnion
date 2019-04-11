using System;
using RedOnion.Script;
using RedOnion.Script.ReflectedObjects;

namespace RedOnion.KSP.ROS_UI
{
	public class Window : UI.Window
	{
		protected IEngine Engine { get; private set; }
		public Window(IEngine engine, string name = null, UI.Layout layout = UI.Layout.Vertical)
			: base(name, layout) => (Engine = engine).Resetting += EngineResetting;
		void EngineResetting(IEngine obj) => Dispose();
		protected override void Dispose(bool disposing)
		{
			if (Engine != null)
			{
				Engine.Resetting -= EngineResetting;
				Engine = null;
			}
			base.Dispose(disposing);
		}
	}
	public class WindowFun : ReflectedType
	{
		public WindowFun(IEngine engine) : base(engine, typeof(Window)) { }
		public override IObject Convert(object value)
			=> new WindowObj(Engine, (Window)value, this);
		public override IObject Create(Arguments args)
		{
			if (args.Length == 0)
				return new WindowObj(Engine, new Window(Engine), this);
			var arg = args[0];
			if (args.Length == 1)
			{
				if (arg.IsNumber)
					return new WindowObj(Engine, new Window(Engine, null, (UI.Layout)arg.Int), this);
				return new WindowObj(Engine, new Window(Engine, arg.String), this);
			}
			var arg2 = args[1];
			if (arg2.IsNumber)
				return new WindowObj(Engine, new Window(Engine, arg.String, (UI.Layout)arg2.Int), this);
			if (arg.IsNumber)
				return new WindowObj(Engine, new Window(Engine, arg2.String, (UI.Layout)arg.Int), this);
			return new WindowObj(Engine, new Window(Engine, arg.String), this);
		}
	}
	public class WindowObj : ReflectedObject<Window>
	{
		internal WindowObj(IEngine engine, Window window, WindowFun type)
			: base(engine, window, type) { }
		~WindowObj() => It.Dispose();
	}
}

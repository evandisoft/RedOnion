using System;
using RedOnion.Script;

namespace RedOnion.Script.BasicObjects
{
	public class SimpleMethod0<Obj> : BasicObject where Obj: IObject
	{
		Action<Obj> action;
		public SimpleMethod0(IEngine engine, Action<Obj> action)
			: base(engine) => this.action = action;
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
		{
			action((Obj)self);
			return new Value();
		}
	}
	public class SimpleFunction0<Obj> : BasicObject where Obj : IObject
	{
		Func<Obj, Value> func;
		public SimpleFunction0(IEngine engine, Func<Obj, Value> func)
			: base(engine) => this.func = func;
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
			=> func((Obj)self);
	}
	public class SimpleMethod1<Obj> : BasicObject where Obj : IObject
	{
		Action<Obj, Value> action;
		public SimpleMethod1(IEngine engine, Action<Obj, Value> action)
			: base(engine) => this.action = action;
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
		{
			action((Obj)self, Engine.GetArgument(argc));
			return new Value();
		}
	}
	public class SimpleFunction1<Obj> : BasicObject where Obj : IObject
	{
		Func<Obj, Value, Value> func;
		public SimpleFunction1(IEngine engine, Func<Obj, Value, Value> func)
			: base(engine) => this.func = func;
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
			=> func((Obj)self, Engine.GetArgument(argc));
	}
	public class SimpleMethod2<Obj> : BasicObject where Obj : IObject
	{
		Action<Obj, Value, Value> action;
		public SimpleMethod2(IEngine engine, Action<Obj, Value, Value> action)
			: base(engine) => this.action = action;
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
		{
			action((Obj)self, Engine.GetArgument(argc), Engine.GetArgument(argc, 1));
			return new Value();
		}
	}
	public class SimpleFunction2<Obj> : BasicObject where Obj : IObject
	{
		Func<Obj, Value, Value, Value> func;
		public SimpleFunction2(IEngine engine, Func<Obj, Value, Value, Value> func)
			: base(engine) => this.func = func;
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
			=> func((Obj)self, Engine.GetArgument(argc), Engine.GetArgument(argc, 1));
	}
}

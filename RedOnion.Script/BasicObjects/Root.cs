using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	public class Root : BasicObject, Engine.IRoot
	{
		public Value Undefined { get; }
		public Value Null { get; }
		public Value NaN { get; }
		public Value Infinity { get; }
		public FunctionFun Function { get; }
		public ObjectFun Object { get; }
		public StringFun String { get; }
		public NumberFun Number { get; }

		public Root(Engine engine)
			: base(engine, null, new Properties(), new Properties())
		{
			BaseProps.Set("undefined", Undefined = new Value());
			BaseProps.Set("null", Null = new Value(ValueKind.Object, null));
			BaseProps.Set("nan", NaN = new Value(double.NaN));
			BaseProps.Set("infinity", Infinity = new Value(double.PositiveInfinity));
			MoreProps.Set("inf", Infinity);
			var obj = new BasicObject(engine);
			var fun = new FunctionObj(engine, obj);
			var str = new StringObj(engine, obj);
			var num = new NumberObj(engine, obj);
			BaseProps.Set("function", new Value(Function = new FunctionFun(engine, fun, fun)));
			BaseProps.Set("object", new Value(Object = new ObjectFun(engine, fun, obj, this)));
			BaseProps.Set("string", new Value(String = new StringFun(engine, fun, str)));
			BaseProps.Set("number", new Value(Number = new NumberFun(engine, fun, num)));
		}

		public IObject Box(Value value)
		{
			for (;;)
			{
				switch (value.Type)
				{
				case ValueKind.Undefined:
					return new BasicObject(Engine, this.Object.Prototype);
				case ValueKind.Object:
					return (IObject)value.ptr;
				case ValueKind.Reference:
					value = ((IProperties)value.ptr).Get(value.str);
					continue;
				case ValueKind.String:
					return new StringObj(Engine, String.Prototype, value.str);
				default:
					if (value.IsNumber)
						return new NumberObj(Engine, Number.Prototype, value);
					throw new NotImplementedException();
				}
			}
		}

		public IObject Create(
			string[] strings, byte[] code, int codeAt, int codeSize, int typeAt,
			Engine.ArgInfo[] args, string body = null, IObject scope = null)
		{
			return new FunctionObj(Engine, Function.Prototype,
				strings, code, codeAt, codeSize, typeAt,
				args, body, scope == this ? null : scope);
		}

		public Value Get(OpCode op)
		{
			return op == OpCode.Null ? new Value(Object) : Get(op.Text());
		}

		public Value Get(OpCode op, Value value)
		{
			throw new NotImplementedException();
		}

		public Value Get(OpCode op, params Value[] par)
		{
			throw new NotImplementedException();
		}
	}
}

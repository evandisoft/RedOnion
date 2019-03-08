using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	public class Root : BasicObject, Engine.IRoot
	{
		public static Value Undefined { get; } = new Value();
		public static Value Null { get; } = new Value(ValueKind.Object, null);
		public static Value NaN { get; } = new Value(double.NaN);
		public static Value Infinity { get; } = new Value(double.PositiveInfinity);

		public FunctionFun Function { get; }
		public ObjectFun Object { get; }
		public StringFun String { get; }
		public NumberFun Number { get; }

		public Dictionary<Type, IObject>
			TypeMap { get; } = new Dictionary<Type, IObject>();
		public IObject this[Type type]
		{
			get
			{
				if (TypeMap.TryGetValue(type, out var value))
					return value;
				value = new ReflectedObjects.ReflectedType(Engine, type);
				TypeMap[type] = value;
				return value;
			}
			set => TypeMap[type] = value;
		}

		public Root(Engine engine)
			: base(engine, null, new Properties(), new Properties())
		{
			var obj = new BasicObject(engine);
			var fun = new FunctionObj(engine, obj);
			var str = new StringObj(engine, obj);
			var num = new NumberObj(engine, obj);
			Function = new FunctionFun(engine, fun, fun);
			Object = new ObjectFun(engine, fun, obj, this);
			String = new StringFun(engine, fun, str);
			Number = new NumberFun(engine, fun, num);
			Fill();
		}

		public override void Reset()
		{
			BaseProps.Reset();
			MoreProps.Reset();
			TypeMap.Clear();
		}

		protected virtual void Fill()
		{
			BaseProps.Set("undefined", Undefined);
			BaseProps.Set("null", Null);
			BaseProps.Set("nan", NaN);
			BaseProps.Set("infinity", Infinity);
			MoreProps.Set("inf", Infinity);
			BaseProps.Set("Function", new Value(Function));
			BaseProps.Set("Object", new Value(Object));
			BaseProps.Set("String", new Value(String));
			BaseProps.Set("Number", new Value(Number));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	public class Root : BasicObject, Engine.IRoot
	{
		public Dictionary<Type, IObject> TypeMap
		{ get; } = new Dictionary<Type, IObject>();

		public static Value Undefined { get; } = new Value();
		public static Value Null { get; } = new Value(ValueKind.Object, null);
		public static Value NaN { get; } = new Value(double.NaN);
		public static Value Infinity { get; } = new Value(double.PositiveInfinity);

		public FunctionFun Function { get; }
		public ObjectFun Object { get; }
		public StringFun String { get; }
		public NumberFun Number { get; }
		public NumberFun Float { get; }
		public NumberFun Double { get; }
		public NumberFun Long { get; }
		public NumberFun ULong { get; }
		public NumberFun Int { get; }
		public NumberFun UInt { get; }
		public NumberFun Short { get; }
		public NumberFun UShort { get; }
		public NumberFun SByte { get; }
		public NumberFun Byte { get; }
		public NumberFun Bool { get; }
		public NumberFun Char { get; }

		public Root(Engine engine)
			: this(engine, true) { }
		protected Root(Engine engine, bool fill)
			: base(engine, null, new Properties(), new Properties())
		{
			var obj = new BasicObject(engine);
			var fun = new FunctionObj(engine, obj);
			var str = new StringObj(engine, obj);
			var num = new NumberObj(engine, obj);
			Function = new FunctionFun(engine, fun, fun);
			Object = new ObjectFun(engine, fun, obj);
			String = new StringFun(engine, fun, str);
			Number = new NumberFun(engine, fun, num);
			Float = new NumberFun(engine, fun, num, typeof(float), ValueKind.Float);
			Double = new NumberFun(engine, fun, num, typeof(double), ValueKind.Double);
			Long = new NumberFun(engine, fun, num, typeof(long), ValueKind.Long);
			ULong = new NumberFun(engine, fun, num, typeof(ulong), ValueKind.ULong);
			Int = new NumberFun(engine, fun, num, typeof(int), ValueKind.Int);
			UInt = new NumberFun(engine, fun, num, typeof(uint), ValueKind.UInt);
			Short = new NumberFun(engine, fun, num, typeof(short), ValueKind.Short);
			UShort = new NumberFun(engine, fun, num, typeof(ushort), ValueKind.UShort);
			SByte = new NumberFun(engine, fun, num, typeof(sbyte), ValueKind.SByte);
			Byte = new NumberFun(engine, fun, num, typeof(byte), ValueKind.Byte);
			Bool = new NumberFun(engine, fun, num, typeof(bool), ValueKind.Bool);
			Char = new NumberFun(engine, fun, num, typeof(char), ValueKind.Char);
			if (fill)
				Fill();
		}

		public override void Reset()
		{
			BaseProps.Reset();
			MoreProps.Reset();
			TypeMap.Clear();
			Fill();
		}

		protected virtual void Fill()
		{
			BaseProps.Set("undefined",	Undefined);
			BaseProps.Set("null",		Null);
			BaseProps.Set("nan",		NaN);
			BaseProps.Set("infinity",	Infinity);
			MoreProps.Set("inf",		Infinity);
			BaseProps.Set("Function",	Function);
			BaseProps.Set("Object",		Object);
			BaseProps.Set("String",		String);
			BaseProps.Set("Number",		Number);
			BaseProps.Set("Float",		Float);
			MoreProps.Set("Single",		Float);
			BaseProps.Set("Double",		Double);
			BaseProps.Set("Long",		Long);
			MoreProps.Set("Int64",		Long);
			BaseProps.Set("ULong",		ULong);
			MoreProps.Set("UInt64",		ULong);
			BaseProps.Set("Int", new Value(Int));
			MoreProps.Set("Int32", new Value(Int));
			BaseProps.Set("UInt", new Value(UInt));
			MoreProps.Set("UInt32", new Value(UInt));
			BaseProps.Set("Short", new Value(Short));
			MoreProps.Set("Int16", new Value(Short));
			BaseProps.Set("UShort", new Value(UShort));
			MoreProps.Set("UInt16", new Value(UShort));
			BaseProps.Set("SByte", new Value(SByte));
			MoreProps.Set("Int8", new Value(SByte));
			BaseProps.Set("Byte", new Value(Byte));
			MoreProps.Set("UInt8", new Value(Byte));
			BaseProps.Set("Bool", new Value(Bool));
			MoreProps.Set("Boolean", new Value(Bool));
			BaseProps.Set("Char", new Value(Char));
			TypeMap[typeof(string)] = String;
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

		public IObject GetType(OpCode op)
		{
			switch(op)
			{
			case OpCode.Null:
			case OpCode.Object:
				return Object;
			case OpCode.String:
				return String;
			case OpCode.Byte:
				return Byte;
			case OpCode.UShort:
				return UShort;
			case OpCode.UInt:
				return UInt;
			case OpCode.ULong:
				return ULong;
			case OpCode.SByte:
				return SByte;
			case OpCode.Short:
				return Short;
			case OpCode.Int:
				return Int;
			case OpCode.Long:
				return Long;
			case OpCode.Float:
				return Float;
			case OpCode.Double:
				return Double;
			case OpCode.Bool:
				return Bool;
			case OpCode.Char:
				return Char;
			default:
				return Number;
			}
		}

		public IObject GetType(OpCode op, Value value)
		{
			//TODO: array object
			throw new NotImplementedException();
		}

		public IObject GetType(OpCode op, params Value[] par)
		{
			//TODO: multi-dimensional array
			throw new NotImplementedException();
		}

		public IObject this[Type type]
		{
			get
			{
				if (TypeMap.TryGetValue(type, out var value))
					return value;
				value = ReflectType(type);
				if (value != null)
					TypeMap[type] = value;
				return value;
			}
			set => TypeMap[type] = value;
		}
		protected virtual IObject ReflectType(Type type)
			=> new ReflectedObjects.ReflectedType(Engine, type);

		public void AddType(string name, Type type, IObject creator)
		{
			this[type] = creator;
			Set(name, new Value(creator));
		}
		public void AddType(string name, ReflectedObjects.ReflectedType type)
			=> AddType(name, type.Type, type);
		public void AddType(string name, Type type)
			=> AddType(name, type, ReflectType(type));
		public void AddType(Type type)
			=> AddType(type.Name, type, ReflectType(type));

	}
}

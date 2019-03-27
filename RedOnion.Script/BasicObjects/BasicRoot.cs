using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	public class BasicRoot : BasicObject, IEngineRoot
	{
		~BasicRoot() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing) { }

		public Dictionary<Type, IObject> TypeMap
		{ get; } = new Dictionary<Type, IObject>();

		public static Value Undefined { get; } = new Value();
		public static Value Null { get; } = new Value(ValueKind.Object, null);
		public static Value NaN { get; } = new Value(double.NaN);
		public static Value Infinity { get; } = new Value(double.PositiveInfinity);

		public FunctionFun Function { get; }
		public ObjectFun Object { get; }
		public StringFun String { get; }
		public ArrayFun  Array { get; }
		public ListFun   List { get; }
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

		public PrintFun Print { get; }

		public BasicRoot(IEngine engine)
			: this(engine, true) { }
		protected BasicRoot(IEngine engine, bool fill)
			: base(engine, null, new Properties(), new Properties())
		{
			var obj	= new BasicObject(engine);
			var fun	= new FunctionObj(engine, obj);
			var str	= new StringObj(engine, obj);
			var arr = new ArrayObj(engine, obj);
			var list= new ListObj(engine, obj);
			var num	= new NumberObj(engine, obj);
			Function= new FunctionFun(engine, fun, fun);
			Object	= new ObjectFun(engine, fun, obj);
			String	= new StringFun(engine, fun, str);
			Array   = new ArrayFun(engine, fun, arr);
			List    = new ListFun(engine, fun, list);
			Number	= new NumberFun(engine, fun, num);
			Float	= new NumberFun(engine, fun, num, typeof(float),	ValueKind.Float);
			Double	= new NumberFun(engine, fun, num, typeof(double),	ValueKind.Double);
			Long	= new NumberFun(engine, fun, num, typeof(long),		ValueKind.Long);
			ULong	= new NumberFun(engine, fun, num, typeof(ulong),	ValueKind.ULong);
			Int		= new NumberFun(engine, fun, num, typeof(int),		ValueKind.Int);
			UInt	= new NumberFun(engine, fun, num, typeof(uint),		ValueKind.UInt);
			Short	= new NumberFun(engine, fun, num, typeof(short),	ValueKind.Short);
			UShort	= new NumberFun(engine, fun, num, typeof(ushort),	ValueKind.UShort);
			SByte	= new NumberFun(engine, fun, num, typeof(sbyte),	ValueKind.SByte);
			Byte	= new NumberFun(engine, fun, num, typeof(byte),		ValueKind.Byte);
			Bool	= new NumberFun(engine, fun, num, typeof(bool),		ValueKind.Bool);
			Char	= new NumberFun(engine, fun, num, typeof(char),		ValueKind.Char);

			Print   = new PrintFun(engine);

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
			TypeMap[typeof(string)] = String;
			FillSystem(BaseProps, MoreProps);
			var sys = new Properties();
			FillSystem(sys, sys);
			BaseProps.Set("System", new SimpleObject(Engine, sys));
		}

		protected void FillSystem(IProperties core, IProperties more)
		{
			core.Set("global",		this);
			core.Set("undefined",	Undefined);
			core.Set("null",		Null);
			core.Set("nan",			NaN);
			core.Set("infinity",	Infinity);
			more.Set("inf",			Infinity);
			core.Set("Function",	Function);
			core.Set("Object",		Object);
			core.Set("String",		String);
			more.Set("Array",		Array);
			more.Set("List",		List);
			more.Set("Number",		Number);
			core.Set("Float",		Float);
			more.Set("Single",		Float);
			core.Set("Double",		Double);
			core.Set("Long",		Long);
			more.Set("Int64",		Long);
			core.Set("ULong",		ULong);
			more.Set("UInt64",		ULong);
			core.Set("Int",			Int);
			more.Set("Int32",		Int);
			core.Set("UInt",		UInt);
			more.Set("UInt32",		UInt);
			core.Set("Short",		Short);
			more.Set("Int16",		Short);
			core.Set("UShort",		UShort);
			more.Set("UInt16",		UShort);
			core.Set("SByte",		SByte);
			more.Set("Int8",		SByte);
			core.Set("Byte",		Byte);
			more.Set("UInt8",		Byte);
			core.Set("Bool",		Bool);
			more.Set("Boolean",		Bool);
			core.Set("Char",		Char);

			core.Set("print",		Print);
		}

		public IObject Box(Value value)
		{
			value = value.RValue;
			switch (value.Kind)
			{
			case ValueKind.Undefined:
				if (Engine.HasOption(EngineOption.Strict))
				{
					if (!Engine.HasOption(EngineOption.Silent))
						throw new InvalidOperationException("Undefined value cannot be boxed into object");
					return null;
				}
				return new BasicObject(Engine, this.Object.Prototype);
			case ValueKind.Object:
				return (IObject)value.ptr;
			case ValueKind.String:
				// TODO: ValueKind.BoxedString to delay the boxing (to speed things up)
				return new StringObj(Engine, String.Prototype, (string)value.ptr);
			default:
				if (value.IsNumber)
					return new NumberObj(Engine, Number.Prototype, value);
				throw new NotImplementedException("Boxing of " + value.Name);
			}
		}

		public IObject Create(string name,
			CompiledCode code, int codeAt, int codeSize, int typeAt,
			ArgumentInfo[] args, string body = null, IScope scope = null)
		{
			return new FunctionObj(Engine, name, Function.Prototype,
				code, codeAt, codeSize, typeAt,
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
			if (op == OpCode.Array)
				return Array;
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown type opcode: {0:04X} {1}", (ushort)op, op));
		}

		public IObject GetType(OpCode op, params Value[] par)
		{
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown type opcode: {0:04X} {1}", (ushort)op, op));
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

		public IObject AddType(string name, Type type, IObject creator = null)
			=> EngineRootExtensions.AddType(this, name, type, creator);
		public IObject AddType(string name, ReflectedObjects.ReflectedType type)
			=> AddType(name, type.Type, type);
		public IObject AddType(string name, Type type)
			=> AddType(name, type, ReflectType(type));
		public IObject AddType(Type type)
			=> AddType(type.Name, type, ReflectType(type));

	}
}

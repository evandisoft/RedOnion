using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Bee.Run.Ecma262
{
	public class Root: Obj, Engine.IRoot
	{
		private Value _undefined;
		public Value Undefined
		{
			get
			{
				return _undefined;
			}
		}//Undefined
		
		private Value _null;
		public Value Null
		{
			get
			{
				return _null;
			}
		}//Null
		
		private Value _nan;
		public Value Nan
		{
			get
			{
				return _nan;
			}
		}//Nan
		
		private Value _infinity;
		public Value Infinity
		{
			get
			{
				return _infinity;
			}
		}//Infinity
		
		private FunctionFun _function;
		public FunctionFun Function
		{
			get
			{
				return _function;
			}
		}//Function
		
		private ObjectFun _object;
		public ObjectFun Object
		{
			get
			{
				return _object;
			}
		}//Object
		
		private StringFun _string;
		public StringFun String
		{
			get
			{
				return _string;
			}
		}//String
		
		private NumberFun _number;
		public NumberFun Number
		{
			get
			{
				return _number;
			}
		}//Number
		
		public Root( Engine engine )
			: base( engine, null, new Props(), new Props() )
		{
			BaseProps.Set( "undefined", _undefined = new Value() );
			BaseProps.Set( "null", _null = new Value( Vtype.Object, null ) );
			BaseProps.Set( "nan", _nan = new Value( System.Double.NaN ) );
			BaseProps.Set( "infinity", _infinity = new Value( System.Double.PositiveInfinity ) );
			MoreProps.Set( "inf", _infinity );
			var obj = new Obj( engine );
			var fun = new FunctionObj( engine, obj );
			var str = new StringObj( engine, obj );
			var num = new NumberObj( engine, obj );
			BaseProps.Set( "function", new Value( _function = new FunctionFun( engine, fun, fun ) ) );
			BaseProps.Set( "object", new Value( _object = new ObjectFun( engine, fun, obj, this ) ) );
			BaseProps.Set( "string", new Value( _string = new StringFun( engine, fun, str ) ) );
			BaseProps.Set( "number", new Value( _number = new NumberFun( engine, fun, num ) ) );
		}//.ctor
		
		public IObject Box( Value value )
		{
			for( ; ;  )
			{
				switch( value.Type )
				{
				case Vtype.Undef:
					return new Obj( Engine, this.Object.Prototype );
				case Vtype.Object:
					return ((Obj)value.Ptr);
				case Vtype.Ident:
					value = ((IProps)value.Ptr).Get( value.Str );
					continue;
				case Vtype.String:
					return new StringObj( Engine, this.String.Prototype, value.Str );
				default:
					if( value.Isnum )
					{
						return new NumberObj( Engine, this.Number.Prototype, value );
					}
					throw new NotImplementedException();
				}
			}
		}//Box
		
		public IObject Create( byte[] code, int codeAt, int codeSize, int typeAt, Engine.ArgInfo[] args, string @string = null, IObject scope = null )
		{
			return new FunctionObj( Engine, Function.Prototype, code, codeAt, codeSize, typeAt, args, @string, scope == this ? null : scope );
		}//Create
		
		public Value Get( Opcode op )
		{
			return op == Opcode.Null ? new Value( this.Object ) : Get( op.Text() );
		}//Get
		
		public Value Get( Opcode op, Value value )
		{
			throw new NotImplementedException();
		}//Get
		
		public Value Get( Opcode op, params Value[] par )
		{
			throw new NotImplementedException();
		}//Get
	}//Root
}//Bee.Run.Ecma262

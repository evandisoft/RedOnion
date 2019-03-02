using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run
{
	public partial class Engine
	{
		protected virtual void Typeref( byte[] code, ref int at )
		{
			var op = ((Opcode)code[at++]).Extend();
			if( op.Kind() <= Opkind.Number )
			{
				if( (op == Opcode.Null) || (unchecked((byte)op) >= unchecked((byte)Opcode.String)) )
				{
					Value = Root.Get( op );
					return;
				}
				if( op == Opcode.Ident )
				{
					Value = Ctx.Get( Cident( code, ref at ) );
					return;
				}
				Value = new Value();
				return;
			}
			if( op == Opcode.Array )
			{
				if( code[at++] != 0 )
				{
					throw new NotImplementedException( "Fixed and multi-dimensional arrays not implemented" );
				}
				Typeref( code, ref at );
				Value = Root.Get( Opcode.Array, Value );
				return;
			}
			throw new NotImplementedException();
		}//Typeref
		
		protected override void Literal( Opcode op, byte[] code, ref int at )
		{
			int n;
			switch( op )
			{
			case Opcode.Undef:
				Value = new Value();
				return;
			case Opcode.Null:
				Value = new Value( Vtype.Object, null );
				return;
			case Opcode.False:
				Value = false;
				return;
			case Opcode.True:
				Value = true;
				return;
			case Opcode.This:
				Value = new Value( Ctx.Self );
				return;
			case Opcode.Base:
				Value = new Value( Ctx.Self?.BaseClass );
				return;
			case Opcode.Ident:
				var ident = Cident( code, ref at );
				Value = new Value( Vtype.Ident, Ctx.Which( ident ) ?? Ctx.Root, ident );
				return;
			case Opcode.String:
				n = code[at++];
				if( n >= (1 << 7) )
				{
					n = (n & 127) + (code[at++] << 7);
					if( n >= (1 << 14) )
					{
						n = (n & ((1 << 14) - 1)) + (code[at++] << 14);
						if( n >= (1 << 21) )
						{
							n = (n & ((1 << 21) - 1)) + (code[at++] << 21);
						}
					}
				}
				Value = Encoding.UTF8.GetString( code, at, n );
				at += n;
				return;
			case Opcode.Byte:
				Value = code[at++];
				return;
			case Opcode.Ushort:
				Value = Cushort( code, ref at );
				return;
			case Opcode.Uint:
				Value = Cuint( code, ref at );
				return;
			case Opcode.Ulong:
				Value = Culong( code, ref at );
				return;
			case Opcode.Sbyte:
				Value = ((sbyte)code[at++]);
				return;
			case Opcode.Short:
				Value = Cshort( code, ref at );
				return;
			case Opcode.Int:
				Value = Cint( code, ref at );
				return;
			case Opcode.Long:
				Value = Clong( code, ref at );
				return;
			case Opcode.Float:
				Value = Cfloat( code, ref at );
				return;
			case Opcode.Double:
				Value = Cdouble( code, ref at );
				return;
			}
			throw new NotImplementedException();
		}//Literal
		
		protected override void Special( Opcode op, byte[] code, ref int at )
		{
			var create = false;
		next:
			switch( op )
			{
			case Opcode.Create:
				create = true;
				op = ((Opcode)code[at]).Extend();
				if( (op.Kind() == Opkind.Special) && (unchecked((byte)op) < unchecked((byte)Opcode.Generic)) )
				{
					at++;
					goto next;
				}
				goto case Opcode.Ecall;
			case Opcode.Ecall:
				if( create )
				{
					Typeref( code, ref at );
				}
				else
				{
					Expression( code, ref at );
				}
				Obj self = null;
				if( Value.Type == Vtype.Ident )
				{
					self = Value.Ptr as Obj;
					Value = ((IProps)Value.Ptr).Get( Value.Str );
				}
				var fn = Box( Value );
				Value = create ? new Value( fn.Create( 0 ) ) : fn.Call( self, 0 );
				return;
			case Opcode.Call:
				if( create )
				{
					Typeref( code, ref at );
				}
				else
				{
					Expression( code, ref at );
				}
				self = null;
				if( Value.Type == Vtype.Ident )
				{
					self = Value.Ptr as Obj;
					Value = ((IProps)Value.Ptr).Get( Value.Str );
				}
				fn = Box( Value );
				Expression( code, ref at );
				Args.Add( Result );
				Value = create ? new Value( fn.Create( 1 ) ) : fn.Call( self, 1 );
				Args.Remove( 1 );
				return;
			case Opcode.Call2:
				if( create )
				{
					Typeref( code, ref at );
				}
				else
				{
					Expression( code, ref at );
				}
				self = null;
				if( Value.Type == Vtype.Ident )
				{
					self = Value.Ptr as Obj;
					Value = ((IProps)Value.Ptr).Get( Value.Str );
				}
				fn = Box( Value );
				Expression( code, ref at );
				Args.Add( Result );
				Expression( code, ref at );
				Args.Add( Result );
				Value = create ? new Value( fn.Create( 2 ) ) : fn.Call( self, 2 );
				Args.Remove( 2 );
				return;
			case Opcode.Mcall:
				int n = code[at++];
				if( create )
				{
					Typeref( code, ref at );
				}
				else
				{
					Expression( code, ref at );
				}
				self = null;
				if( Value.Type == Vtype.Ident )
				{
					self = Value.Ptr as Obj;
					Value = ((IProps)Value.Ptr).Get( Value.Str );
				}
				fn = Box( Value );
				var argc = n - 1;
				while( (--n) > 0 ){
					Expression( code, ref at );
					Args.Add( Result );
				}
				Value = create ? new Value( fn.Create( argc ) ) : fn.Call( self, argc );
				Args.Remove( argc );
				return;
			case Opcode.Index:
			case Opcode.Mindex:
				n = op == Opcode.Index ? 2 : code[at++];
				Expression( code, ref at );
				self = null;
				if( Value.Type == Vtype.Ident )
				{
					self = Value.Ptr as Obj;
					Value = ((IProps)Value.Ptr).Get( Value.Str );
				}
				fn = Box( Value );
				argc = n - 1;
				while( (--n) > 0 ){
					Expression( code, ref at );
					Args.Add( Result );
				}
				Value = fn.Index( self, argc );
				Args.Remove( argc );
				return;
			case Opcode.Var:
				var ident = Cident( code, ref at );
				Typeref( code, ref at );
				if( Value.Type == Vtype.Undef )
				{
					Expression( code, ref at );
					Ctx.Vars.Set( ident, Value );
					return;
				}
				fn = Box( Value );
				Expression( code, ref at );
				Args.Add( Result );
				Value = fn.Call( null, 1 );
				Args.Remove( 1 );
				Ctx.Vars.Set( ident, Value );
				return;
			case Opcode.Dot:
				Expression( code, ref at );
				fn = Box( Value );
				ident = Cident( code, ref at );
				Value = new Value( fn, ident );
				return;
			case Opcode.Ternary:
				Expression( code, ref at );
				if( Result.Bool )
				{
					at += 4;
					Expression( code, ref at );
					var fsz = Cint( code, ref at );
					at += fsz;
				}
				else
				{
					var tsz = Cint( code, ref at );
					at += tsz;
					at += 4;
					Expression( code, ref at );
				}
				return;
			}
			throw new NotImplementedException();
		}//Special
		
		protected override void Binary( Opcode op, byte[] code, ref int at )
		{
			Expression( code, ref at );
			if( (op == Opcode.LogicAnd) || (op == Opcode.LogicOr) )
			{
				if( (Value == true) == (op == Opcode.LogicOr) )
				{
					Value = op == Opcode.LogicOr;
					var n = Cint( code, ref at );
					at += n;
					return;
				}
				Expression( code, ref at );
				return;
			}
			var left = Value;
			Expression( code, ref at );
			switch( op )
			{
			case Opcode.Assign:
				left.Set( Value );
				return;
			case Opcode.OrAssign:
				left.Set( Value = (left | Value) );
				return;
			case Opcode.XorAssign:
				left.Set( Value = (left ^ Value) );
				return;
			case Opcode.AndAssign:
				left.Set( Value = (left & Value) );
				return;
			case Opcode.LshAssign:
				left.Set( Value = left.ShiftLeft( Value ) );
				return;
			case Opcode.RshAssign:
				left.Set( Value = left.ShiftRight( Value ) );
				return;
			case Opcode.AddAssign:
				left.Set( Value = (left + Value) );
				return;
			case Opcode.SubAssign:
				left.Set( Value = (left - Value) );
				return;
			case Opcode.MulAssign:
				left.Set( Value = (left * Value) );
				return;
			case Opcode.DivAssign:
				left.Set( Value = (left / Value) );
				return;
			case Opcode.ModAssign:
				left.Set( Value = (left % Value) );
				return;
			case Opcode.BitOr:
				Value = left | Value;
				return;
			case Opcode.BitXor:
				Value = left ^ Value;
				return;
			case Opcode.BitAnd:
				Value = left & Value;
				return;
			case Opcode.ShiftLeft:
				Value = left.ShiftLeft( Value );
				return;
			case Opcode.ShiftRight:
				Value = left.ShiftRight( Value );
				return;
			case Opcode.Add:
				Value = left + Value;
				return;
			case Opcode.Sub:
				Value = left - Value;
				return;
			case Opcode.Mul:
				Value = left * Value;
				return;
			case Opcode.Div:
				Value = left / Value;
				return;
			case Opcode.Mod:
				Value = left % Value;
				return;
			case Opcode.Equals:
				Value = new Value( left == Value );
				return;
			case Opcode.Differ:
				Value = new Value( left != Value );
				return;
			case Opcode.Less:
				Value = new Value( left < Value );
				return;
			case Opcode.More:
				Value = new Value( left > Value );
				return;
			case Opcode.Lesseq:
				Value = new Value( left <= Value );
				return;
			case Opcode.Moreeq:
				Value = new Value( left >= Value );
				return;
			}
			throw new NotImplementedException();
		}//Binary
		
		protected override void Unary( Opcode op, byte[] code, ref int at )
		{
			Expression( code, ref at );
			switch( op )
			{
			case Opcode.Plus:
				Value = -Value;
				return;
			case Opcode.Neg:
				Value = -Value;
				return;
			case Opcode.Flip:
				Value = ~Value;
				return;
			case Opcode.Not:
				Value = new Value( !Value.Bool );
				return;
			case Opcode.PostInc:
				if( Value.Type == Vtype.Ident )
				{
					Value = Value.Self++;
				}
				return;
			case Opcode.PostDec:
				if( Value.Type == Vtype.Ident )
				{
					Value = Value.Self--;
				}
				return;
			case Opcode.Inc:
				++Value.Self;
				return;
			case Opcode.Dec:
				++Value.Self;
				return;
			}
			throw new NotImplementedException();
		}//Unary
	}//Engine
}//Bee.Run

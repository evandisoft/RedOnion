using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class BsGenerator
	{
		protected override void Literal( Opcode op, byte[] code, ref int at )
		{
			if( op < Opcode.Ident )
			{
				Write( op.Text() );
				return;
			}
			if( (op == Opcode.Ident) || (op == Opcode.Number) )
			{
				var n = code[at++];
				Write( Encoding.UTF8.GetString( code, at, n ) );
				at += n;
				return;
			}
			if( op == Opcode.String )
			{
				var n = Cint( code, ref at );
				Write( Encoding.UTF8.GetString( code, at, n ) );
				at += n;
				return;
			}
			throw new InvalidOperationException();
		}//Literal
		
		protected override void Binary( Opcode op, byte[] code, ref int at )
		{
			var paren = Parens;
			if( paren )
			{
				Write( '(' );
			}
			if( op == Opcode.Cast )
			{
				Typeref( code, ref at );
				Write( "! " );
				Expression( code, ref at );
				if( paren )
				{
					Write( ')' );
				}
				return;
			}
			Expression( code, ref at );
			Write( ' ' );
			Write( op.Text() );
			Write( ' ' );
			if( (op == Opcode.LogicAnd) || (op == Opcode.LogicOr) )
			{
				at += 4;
			}
			Expression( code, ref at );
			if( paren )
			{
				Write( ')' );
			}
		}//Binary
		
		protected override void Unary( Opcode op, byte[] code, ref int at )
		{
			var paren = Parens;
			if( paren )
			{
				Write( '(' );
			}
			var text = op.Text();
			var post = op.Postfix();
			if( !post )
			{
				Write( text );
				if( text.Length > 2 )
				{
					Write( ' ' );
				}
			}
			Expression( code, ref at );
			if( post )
			{
				Write( text );
			}
			if( paren )
			{
				Write( ')' );
			}
		}//Unary
		
		protected override void Special( Opcode op, byte[] code, ref int at )
		{
			var paren = Parens;
			var create = false;
		next:
			switch( op )
			{
			case Opcode.Create:
				create = true;
				Write( "new " );
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
					if( code[at] == unchecked((byte)Opcode.Array) )
					{
						Typeref( code, ref at );
						return;
					}
					Typeref( code, ref at );
				}
				else
				{
					Expression( code, ref at );
				}
				Write( "()" );
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
				Write( paren ? '(' : ' ' );
				Expression( code, ref at );
				if( paren )
				{
					Write( ')' );
				}
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
				Write( paren ? '(' : ' ' );
				Expression( code, ref at );
				Write( ", " );
				Expression( code, ref at );
				if( paren )
				{
					Write( ')' );
				}
				return;
			case Opcode.Mcall:
				var mcn = ((int)(code[at++] - 1));
				if( mcn < 2 )
				{
					throw new InvalidOperationException();
				}
				if( create )
				{
					Typeref( code, ref at );
				}
				else
				{
					Expression( code, ref at );
				}
				Write( paren ? '(' : ' ' );
				Expression( code, ref at );
				while( (--mcn) > 0 ){
					Write( ", " );
					Expression( code, ref at );
				}
				if( paren )
				{
					Write( ')' );
				}
				return;
			case Opcode.Index:
				Expression( code, ref at );
				Write( '[' );
				Current = 0;
				Expression( code, ref at );
				Current = op;
				Write( ']' );
				return;
			case Opcode.Mindex:
				mcn = ((int)(code[at++] - 1));
				if( mcn < 2 )
				{
					throw new InvalidOperationException();
				}
				Expression( code, ref at );
				Write( '[' );
				Expression( code, ref at );
				while( (--mcn) > 0 ){
					Write( ", " );
					Expression( code, ref at );
				}
				Write( ']' );
				return;
			case Opcode.Var:
				var n = code[at++];
				var s = Encoding.UTF8.GetString( code, at, n );
				at += n;
				Write( "var " );
				Write( s );
				if( code[at] == 0 )
				{
					at++;
				}
				else
				{
					Write( ' ' );
					Typeref( code, ref at );
				}
				if( code[at] == 0 )
				{
					at++;
				}
				else
				{
					Write( " = " );
					Expression( code, ref at );
				}
				return;
			case Opcode.Dot:
				Expression( code, ref at );
				Write( '.' );
				n = code[at++];
				Write( Encoding.UTF8.GetString( code, at, n ) );
				at += n;
				return;
			case Opcode.Ternary:
				if( paren )
				{
					Write( '(' );
				}
				Expression( code, ref at );
				Write( " ? " );
				at += 4;
				Current = 0;
				Expression( code, ref at );
				Write( " : " );
				at += 4;
				Expression( code, ref at );
				Current = op;
				if( paren )
				{
					Write( ')' );
				}
				return;
			}
			throw new NotImplementedException();
		}//Special
		
		protected virtual bool Parens
		{
			get
			{
				var inside = this.Inside;
				if( unchecked((byte)inside) >= 0x80 )
				{
					return false;
				}
				var op = this.Current;
				if( inside.Prior() <= Opcode.Assign.Prior() )
				{
					return (inside.Kind() == Opkind.Special) && ((op.Prior() <= Opcode.Assign.Prior()) || (inside == Opcode.Dot));
				}
				return ((((((op.Prior() < inside.Prior()) || (op == Opcode.ShiftLeft)) || (op == Opcode.ShiftLeft)) || (inside == Opcode.ShiftLeft)) || (inside == Opcode.ShiftLeft)) || (inside == Opcode.BitOr)) || ((inside.Prior() == Opcode.Mul.Prior()) && ((op == Opcode.BitAnd) || (op == Opcode.BitXor)));
			}
		}//Parens
	}//BsGenerator
}//Bee

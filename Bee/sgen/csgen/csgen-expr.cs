using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class CsGenerator
	{
		public new CsGenerator Expression( byte[] code, ref int at )
		{
			this.Expression_( code, ref at );
			return this;
		}//Expression
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Expression_( byte[] code, ref int at )
		{
			var xpar = (Current == Opcode.Typeof) || (Current == Opcode.Nameof);
			if( xpar )
			{
				Write( '(' );
			}
			base.Expression_( code, ref at );
			if( xpar )
			{
				Write( ')' );
			}
		}//Expression_
		
		protected override bool Parens
		{
			get
			{
				var inside = this.Inside;
				if( unchecked((byte)inside) >= 0x80 )
				{
					return false;
				}
				var op = this.Current;
				if( ((op == Opcode.Call) || (op == Opcode.Call2)) || (op == Opcode.Mcall) )
				{
					return true;
				}
				if( inside.Cprior() <= Opcode.Assign.Cprior() )
				{
					return (inside == Opcode.Cast) || ((inside.Kind() == Opkind.Special) && ((op.Cprior() <= Opcode.Assign.Cprior()) || (inside == Opcode.Dot)));
				}
				return (((((op.Cprior() < inside.Cprior()) || (op == Opcode.ShiftLeft)) || (op == Opcode.ShiftLeft)) || (inside == Opcode.ShiftLeft)) || (inside == Opcode.ShiftLeft)) || (inside == Opcode.BitOr);
			}
		}//Parens
		
		protected override void Literal( Opcode op, byte[] code, ref int at )
		{
			if( op != Opcode.Ident )
			{
				base.Literal( op, code, ref at );
				return;
			}
			var n = code[at++];
			var s = Encoding.UTF8.GetString( code, at, n );
			at += n;
			if( Aliasing == Alias.None )
			{
				if( s[0] == '$' )
				{
					s = s.Substring( 1 );
				}
				if( HasBuiltin( s ) )
				{
					Write( '@' );
				}
				Write( s );
				return;
			}
			if( Inside == Opcode.Dot )
			{
				Name.Append( s );
				return;
			}
			Write( Unalias( s ) );
		}//Literal
		
		protected override void Special( Opcode op, byte[] code, ref int at )
		{
			var create = false;
		next:
			switch( op )
			{
			default:
				base.Special( op, code, ref at );
				return;
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
				Write( '(' );
				Expression( code, ref at );
				Write( ')' );
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
				Write( '(' );
				Expression( code, ref at );
				Write( ", " );
				Expression( code, ref at );
				Write( ')' );
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
				Write( '(' );
				Expression( code, ref at );
				while( (--mcn) > 0 ){
					Write( ", " );
					Expression( code, ref at );
				}
				Write( ')' );
				return;
			case Opcode.Dot:
				Debug.Assert( Name.Length == 0 );
				Expression( code, ref at );
				var s = Cident( code, ref at );
				if( Name.Length == 0 )
				{
					Write( '.' );
					Write( Unalias( s, true ) );
					return;
				}
				Name.Append( '.' );
				Name.Append( s );
				if( Inside != Opcode.Dot )
				{
					Write( Unalias() );
				}
				return;
			case Opcode.Var:
				s = Cident( code, ref at );
				if( s[0] == '$' )
				{
					s = s.Substring( 1 );
				}
				else if( Aliasing != Alias.None )
				{
					if( Aliasing == Alias.FirstUpper )
					{
						s = System.Char.ToUpper( s[0] ) + s.Substring( 1 );
					}
					else
					{
						AddLocal( s );
					}
				}
				if( code[at] == 0 )
				{
					at++;
					Write( "var " );
					Write( s );
				}
				else
				{
					Typeref( code, ref at );
					Write( ' ' );
					Write( s );
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
			}
		}//Special
		
		protected override void Binary( Opcode op, byte[] code, ref int at )
		{
			switch( op )
			{
			default:
				base.Binary( op, code, ref at );
				return;
			case Opcode.Cast:
				var paren = Parens;
				if( paren )
				{
					Write( '(' );
				}
				Write( '(' );
				Typeref( code, ref at );
				Write( ')' );
				Expression( code, ref at );
				if( paren )
				{
					Write( ')' );
				}
				return;
			case Opcode.Ascast:
				paren = Parens;
				if( paren )
				{
					Write( '(' );
				}
				Swap();
				Expression( code, ref at );
				Swap();
				Write( '(' );
				Typeref( code, ref at );
				Write( ')' );
				Paste();
				if( paren )
				{
					Write( ')' );
				}
				return;
			}
		}//Binary
	}//CsGenerator
}//Bee

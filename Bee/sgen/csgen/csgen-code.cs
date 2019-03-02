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
		protected bool WasBlock = false;
		protected new CsGenerator Block( byte[] code, ref int at )
		{
			this.Block_( code, ref at );
			return this;
		}//Block
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Block_( byte[] code, ref int at )
		{
			var n = Current.Kind() >= Opkind.Model ? 0 : Cint( code, ref at );
			var size = Cint( code, ref at );
			if( size == 0 )
			{
				Write( " {}" );
				WasBlock = true;
				return;
			}
			if( Aliasing >= Alias.ExceptLocal )
			{
				PushLocal();
			}
			var end = at + size;
			var elseif = false;
			if( ((n == 1) && (Current == Opcode.Else)) && (code[at] == unchecked((byte)Opcode.If)) )
			{
				Write( ' ' );
				elseif = true;
			}
			else
			{
				Line();
				Write( "{" );
				Indent++;
				Line();
			}
			for( ; ;  )
			{
				WasBlock = false;
				Process( code, ref at );
				if( WasBlock )
				{
					WasBlock = false;
				}
				else
				{
					Write( ';' );
				}
				if( at >= end )
				{
					break;
				}
				Line();
			}
			at = end;
			if( !elseif )
			{
				Indent--;
				Line();
				Write( "}" );
			}
			else
			{
				Line();
			}
			WasBlock = true;
			if( Aliasing >= Alias.ExceptLocal )
			{
				PopLocal();
			}
		}//Block_
		
		protected override void Statement( Opcode op, byte[] code, ref int at )
		{
			switch( op )
			{
			default:
				base.Statement( op, code, ref at );
				return;
			case Opcode.Raise:
				if( code[at] == 0 )
				{
					at++;
					Write( "throw" );
				}
				Write( "throw " );
				Expression( code, ref at );
				return;
			case Opcode.For:
				Write( "for (" );
				if( code[at] == 0 )
				{
					++at;
				}
				else
				{
					Expression( code, ref at );
				}
				if( code[at] == 0 )
				{
					++at;
					Write( ';' );
				}
				else
				{
					Write( "; " );
					Expression( code, ref at );
				}
				at += 4;
				if( 0 == Cint( code, ref at ) )
				{
					Write( ';' );
				}
				else
				{
					Write( "; " );
					Expression( code, ref at );
				}
				Write( ')' );
				Block( code, ref at );
				return;
			case Opcode.Foreach:
				Write( "foreach (" );
				if( code[at] != unchecked((byte)Opcode.Var) )
				{
					Write( "var " );
				}
				Expression( code, ref at );
				Write( " in " );
				Expression( code, ref at );
				Write( ')' );
				Block( code, ref at );
				return;
			case Opcode.While:
			case Opcode.Until:
				Write( op == Opcode.While ? "while (" : "until (" );
				Expression( code, ref at );
				Write( ')' );
				Block( code, ref at );
				return;
			case Opcode.Do:
			case Opcode.Dountil:
				Write( "do" );
				Block( code, ref at );
				Line();
				Write( op == Opcode.Do ? "while (" : "until (" );
				Expression( code, ref at );
				Write( ')' );
				WasBlock = false;
				return;
			case Opcode.If:
				Write( "if (" );
				Expression( code, ref at );
				Write( ')' );
				Block( code, ref at );
				if( (at >= code.Length) || (code[at] != unchecked((byte)Opcode.Else)) )
				{
					return;
				}
				Current = Opcode.Else;
				at++;
				Line();
				Write( "else" );
				Block( code, ref at );
				return;
			}
		}//Statement
	}//CsGenerator
}//Bee

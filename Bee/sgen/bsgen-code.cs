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
		protected new BsGenerator Block( byte[] code, ref int at )
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
				return;
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
				Indent++;
				Line();
			}
			for( ; ;  )
			{
				Process( code, ref at );
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
			}
		}//Block_
		
		protected override void Statement( Opcode op, byte[] code, ref int at )
		{
			switch( op )
			{
			default:
				throw new NotImplementedException();
			case Opcode.Return:
				if( code[at] == 0 )
				{
					at++;
					Write( "return" );
					return;
				}
				Write( "return " );
				Expression( code, ref at );
				return;
			case Opcode.Raise:
				if( code[at] == 0 )
				{
					at++;
					Write( "throw" );
				}
				Write( code[at] == unchecked((byte)Opcode.Create) ? "throw " : "raise " );
				Expression( code, ref at );
				return;
			case Opcode.Break:
				Write( "break" );
				return;
			case Opcode.Continue:
				Write( "continue" );
				return;
			case Opcode.For:
				if( code[at] == 0 )
				{
					++at;
					Write( "for" );
				}
				else
				{
					Write( "for " );
					Expression( code, ref at );
				}
				var notest = code[at] == 0;
				if( notest )
				{
					++at;
				}
				else
				{
					Write( "; " );
					Expression( code, ref at );
				}
				at += 4;
				var nolast = 0 == Cint( code, ref at );
				if( nolast )
				{
					if( !notest )
					{
						Write( "; " );
					}
				}
				else
				{
					Write( "; " );
					Expression( code, ref at );
				}
				Block( code, ref at );
				return;
			case Opcode.Foreach:
				Write( "for " );
				Expression( code, ref at );
				Write( " in " );
				Expression( code, ref at );
				Block( code, ref at );
				return;
			case Opcode.While:
			case Opcode.Until:
				Write( op == Opcode.While ? "while " : "until " );
				Expression( code, ref at );
				Block( code, ref at );
				return;
			case Opcode.Do:
			case Opcode.Dountil:
				Write( "do" );
				Block( code, ref at );
				Line();
				Write( op == Opcode.Do ? "while " : "until " );
				Expression( code, ref at );
				return;
			case Opcode.If:
				Write( "if " );
				Expression( code, ref at );
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
	}//BsGenerator
}//Bee

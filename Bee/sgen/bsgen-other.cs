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
		protected virtual void Typeref( byte[] code, ref int at )
		{
			var op = ((Opcode)code[at++]).Extend();
			switch( op )
			{
			default:
				if( op.Kind() <= Opkind.Number )
				{
					Debug.Assert( ((op == Opcode.String) || (op == Opcode.Char)) || (op.Kind() == Opkind.Number) );
					Write( op.Text() );
					return;
				}
				throw new NotImplementedException();
			case Opcode.Undef:
				return;
			case Opcode.Null:
				Write( "object" );
				return;
			case Opcode.Ident:
				Write( Cident( code, ref at ) );
				return;
			case Opcode.Dot:
				Typeref( code, ref at );
				Write( '.' );
				Write( Cident( code, ref at ) );
				return;
			case Opcode.Array:
				var n = code[at++];
				Typeref( code, ref at );
				if( n == 1 )
				{
					Write( "[]" );
					return;
				}
				Write( '[' );
				while( (--n) > 0 )
				{
					Expression( code, ref at );
				}
				Write( ']' );
				return;
			case Opcode.Generic:
				n = code[at++];
				Typeref( code, ref at );
				if( n == 1 )
				{
					Write( ".[]" );
					return;
				}
				Write( ".[" );
				while( (--n) > 0 )
				{
					Typeref( code, ref at );
				}
				Write( ']' );
				return;
			}
		}//Typeref
		
		protected override void Other( Opcode op, byte[] code, ref int at )
		{
			switch( op )
			{
			default:
				throw new NotImplementedException();
			case Opcode.Import:
				Write( "use " );
				Write( Cident( code, ref at ) );
				return;
			case Opcode.Space:
				if( code[at] == 0 )
				{
					Write( "pkg" );
					return;
				}
				Write( "pkg " );
				Write( Cident( code, ref at ) );
				return;
			case Opcode.Class:
			case Opcode.Struct:
			case Opcode.Enum:
			case Opcode.Face:
				var size = Cint( code, ref at );
				var body = at + size;
				var tflags = ((Tflag)Cushort( code, ref at ));
				var gtnum = code[at++];
				var bcnum = code[at++];
				Write( "def " );
				Write( Cident( code, ref at ) );
				if( gtnum != 0 )
				{
					throw new NotImplementedException();
				}
				if( op != Opcode.Class )
				{
					Write( ' ' );
					Write( op.Text() );
				}
				if( bcnum > 0 )
				{
					do
					{
						Write( ' ' );
						Typeref( code, ref at );
					}
					while( (--bcnum) > 0 );
				}
				var access = tflags.AccessText();
				if( access != null )
				{
					Write( ' ' );
					Write( access );
				}
				var scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( ' ' );
					Write( scope );
				}
				if( (tflags & Tflag.Partial) != 0 )
				{
					Write( " partial" );
				}
				if( (tflags & Tflag.Unsafe) != 0 )
				{
					Write( " unsafe" );
				}
				if( (tflags & Tflag.Hide) != 0 )
				{
					Write( " new" );
				}
				Debug.Assert( at == body );
				at = body;
				Block( code, ref at );
				return;
			case Opcode.Field:
			case Opcode.Event:
				size = Cint( code, ref at );
				tflags = ((Tflag)Cushort( code, ref at ));
				Write( op == Opcode.Event ? "event " : tflags.Scope() == Tflag.Const ? "const " : "var " );
				Write( Cident( code, ref at ) );
				if( code[at] == 0 )
				{
					at++;
				}
				else
				{
					Write( ' ' );
					Typeref( code, ref at );
				}
				access = tflags.AccessText();
				if( access != null )
				{
					Write( ' ' );
					Write( access );
				}
				if( tflags.Scope() != Tflag.Const )
				{
					scope = tflags.ScopeText();
					if( scope != null )
					{
						Write( ' ' );
						Write( scope );
					}
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
			case Opcode.Func:
				size = Cint( code, ref at );
				body = at + size;
				tflags = ((Tflag)Cushort( code, ref at ));
				Debug.Assert( code[at] == 0 );
				at++;
				var argc = code[at++];
				Write( Cident( code, ref at ) );
				size = Cint( code, ref at );
				var args = at + size;
				if( (size != 0) && (code[at] != 0) )
				{
					Write( ' ' );
					Typeref( code, ref at );
				}
				Debug.Assert( (at == args) || ((size == 1) && (code[at] == 0)) );
				at = args;
				while( argc > 0 )
				{
					size = Cint( code, ref at );
					var narg = at + size;
					Write( ", " );
					Write( Cident( code, ref at ) );
					size = Cint( code, ref at );
					var vat = at + size;
					if( (size != 0) && (code[at] != 0) )
					{
						Write( ' ' );
						Typeref( code, ref at );
					}
					Debug.Assert( (at == vat) || ((size == 1) && (code[at] == 0)) );
					at = vat;
					size = Cint( code, ref at );
					if( (size != 0) && (code[at] != 0) )
					{
						Write( " = " );
						Expression( code, ref at );
					}
					Debug.Assert( (at == narg) || ((size == 1) && (code[at] == 0)) );
					at = narg;
					argc--;
				}
				access = tflags.AccessText();
				if( access != null )
				{
					Write( ' ' );
					Write( access );
				}
				scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( ' ' );
					Write( scope );
				}
				if( (tflags & Tflag.Hide) != 0 )
				{
					Write( "new " );
				}
				if( (tflags & Tflag.Unsafe) != 0 )
				{
					Write( "unsafe " );
				}
				Debug.Assert( at == body );
				at = body;
				Block( code, ref at );
				return;
			case Opcode.Prop:
				size = Cint( code, ref at );
				body = at + size;
				tflags = ((Tflag)Cushort( code, ref at ));
				if( code[at] != 0 )
				{
					throw new NotImplementedException();
				}
				at++;
				argc = code[at++];
				Write( Cident( code, ref at ) );
				size = Cint( code, ref at );
				args = at + size;
				Debug.Assert( (size != 0) && (code[at] != 0) );
				Write( ' ' );
				Typeref( code, ref at );
				at = args;
				if( argc > 0 )
				{
					throw new NotImplementedException();
				}
				access = tflags.AccessText();
				if( access != null )
				{
					Write( ' ' );
					Write( access );
				}
				scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( ' ' );
					Write( scope );
				}
				if( (tflags & Tflag.Hide) != 0 )
				{
					Write( "new " );
				}
				if( (tflags & Tflag.Unsafe) != 0 )
				{
					Write( "unsafe " );
				}
				Debug.Assert( at == body );
				at = body;
				size = Cint( code, ref at );
				var end = at + size;
				Indent++;
				while( at < end )
				{
					switch( ((Opcode)((ushort)(code[at++] << 8))) | Opcode.Prop )
					{
					default:
						throw new NotImplementedException();
					case Opcode.Get:
						Line();
						Write( "get" );
						break;
					case Opcode.Set:
						Line();
						Write( "set" );
						break;
					}
					size = Cint( code, ref at );
					at += size;
					Block( code, ref at );
				}
				Indent--;
				return;
			}
		}//Other
	}//BsGenerator
}//Bee

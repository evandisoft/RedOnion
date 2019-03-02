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
		protected override void Typeref( byte[] code, ref int at )
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
				var s = Cident( code, ref at );
				if( Aliasing == Alias.None )
				{
					if( s[0] == '$' )
					{
						s = s.Substring( 1 );
					}
					Write( s );
					return;
				}
				if( Inside == Opcode.Dot )
				{
					Name.Append( s );
					return;
				}
				Write( Unalias( s, false, true ) );
				return;
			case Opcode.Dot:
				Debug.Assert( Name.Length == 0 );
				Typeref( code, ref at );
				s = Cident( code, ref at );
				if( Name.Length == 0 )
				{
					Write( '.' );
					Write( Unalias( s, true, true ) );
					return;
				}
				Name.Append( '.' );
				Name.Append( s );
				if( Inside != Opcode.Dot )
				{
					Write( Unalias( false, true ) );
				}
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
					Write( "<>" );
					return;
				}
				Write( '<' );
				while( (--n) > 0 )
				{
					Typeref( code, ref at );
				}
				Write( '>' );
				return;
			}
		}//Typeref
		
		protected string Space = null;
		protected override void Other( Opcode op, byte[] code, ref int at )
		{
			switch( op )
			{
			default:
				throw new NotImplementedException();
			case Opcode.Import:
				Write( "using " );
				Write( Unalias( Cident( code, ref at ), false, true ) );
				return;
			case Opcode.Space:
				if( Space != null )
				{
					Indent--;
					Line();
					Write( "}" );
				}
				if( code[at] == 0 )
				{
					at++;
					Space = null;
					return;
				}
				Write( "namespace " );
				Write( Space = Unalias( Cident( code, ref at ), false, true ) );
				Line();
				Write( "{" );
				Indent++;
				Line();
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
				var access = tflags.AccessText() ?? DefClassAccess.AccessText();
				if( access != null )
				{
					Write( access );
					Write( ' ' );
				}
				var scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( scope );
					Write( ' ' );
				}
				if( (tflags & Tflag.Hide) != 0 )
				{
					Write( "new " );
				}
				if( (tflags & Tflag.Unsafe) != 0 )
				{
					Write( "unsafe " );
				}
				if( (tflags & Tflag.Partial) != 0 )
				{
					Write( "partial " );
				}
				Write( op.Text() );
				Write( ' ' );
				Write( Unalias( Cident( code, ref at ), false, true ) );
				if( gtnum != 0 )
				{
					throw new NotImplementedException();
				}
				if( bcnum != 0 )
				{
					var first = true;
					do
					{
						Write( first ? ": " : ", " );
						first = false;
						Typeref( code, ref at );
					}
					while( (--bcnum) > 0 );
				}
				Debug.Assert( at == body );
				at = body;
				Block( code, ref at );
				return;
			case Opcode.Field:
			case Opcode.Event:
				size = Cint( code, ref at );
				tflags = ((Tflag)Cushort( code, ref at ));
				access = tflags.AccessText();
				if( access != null )
				{
					Write( access );
					Write( ' ' );
				}
				scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( scope );
					Write( ' ' );
				}
				if( op == Opcode.Event )
				{
					Write( "event " );
				}
				var s = Unalias( Cident( code, ref at ) );
				if( code[at] == 0 )
				{
					at++;
					Write( s );
				}
				else
				{
					Typeref( code, ref at );
				}
				Write( ' ' );
				Write( s );
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
				PushLocal();
				size = Cint( code, ref at );
				body = at + size;
				tflags = ((Tflag)Cushort( code, ref at ));
				access = tflags.AccessText() ?? DefMethodAccess.AccessText();
				if( access != null )
				{
					Write( access );
					Write( ' ' );
				}
				scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( scope );
					Write( ' ' );
				}
				if( (tflags & Tflag.Hide) != 0 )
				{
					Write( "new " );
				}
				if( (tflags & Tflag.Unsafe) != 0 )
				{
					Write( "unsafe " );
				}
				Debug.Assert( code[at] == 0 );
				at++;
				var argc = code[at++];
				s = Unalias( Cident( code, ref at ) );
				size = Cint( code, ref at );
				var args = at + size;
				if( (size == 0) || (code[at] == 0) )
				{
					Write( "void" );
				}
				else
				{
					Typeref( code, ref at );
				}
				Write( ' ' );
				Write( s );
				Write( '(' );
				Debug.Assert( (at == args) || ((size == 1) && (code[at] == 0)) );
				at = args;
				for( var i = 0; i < argc; i++ )
				{
					size = Cint( code, ref at );
					var narg = at + size;
					if( i > 0 )
					{
						Write( ", " );
					}
					s = Cident( code, ref at );
					AddLocal( s );
					size = Cint( code, ref at );
					var vat = at + size;
					if( (size == 0) || (code[at] == 0) )
					{
						Write( Unalias( s ) );
					}
					else
					{
						Typeref( code, ref at );
					}
					Write( ' ' );
					Write( s );
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
				}
				Write( ')' );
				Debug.Assert( at == body );
				at = body;
				Block( code, ref at );
				PopLocal();
				return;
			case Opcode.Prop:
				size = Cint( code, ref at );
				body = at + size;
				tflags = ((Tflag)Cushort( code, ref at ));
				access = tflags.AccessText() ?? DefPropertyAccess.AccessText();
				if( access != null )
				{
					Write( access );
					Write( ' ' );
				}
				scope = tflags.ScopeText();
				if( scope != null )
				{
					Write( scope );
					Write( ' ' );
				}
				if( (tflags & Tflag.Hide) != 0 )
				{
					Write( "new " );
				}
				if( (tflags & Tflag.Unsafe) != 0 )
				{
					Write( "unsafe " );
				}
				if( code[at] != 0 )
				{
					throw new NotImplementedException();
				}
				at++;
				argc = code[at++];
				s = Unalias( Cident( code, ref at ) );
				size = Cint( code, ref at );
				args = at + size;
				Debug.Assert( (size != 0) && (code[at] != 0) );
				Typeref( code, ref at );
				Write( ' ' );
				Write( s );
				at = args;
				if( argc > 0 )
				{
					throw new NotImplementedException();
				}
				Debug.Assert( at == body );
				at = body;
				size = Cint( code, ref at );
				var end = at + size;
				Line();
				Write( "{" );
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
						size = Cint( code, ref at );
						at += size;
						Block( code, ref at );
						continue;
					case Opcode.Set:
						Line();
						Write( "set" );
						PushLocal();
						AddLocal( "value" );
						size = Cint( code, ref at );
						at += size;
						Block( code, ref at );
						PopLocal();
						continue;
					}
				}
				Indent--;
				Line();
				Write( "}" );
				return;
			}
		}//Other
	}//CsGenerator
}//Bee

using Bee.Run;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class Parser
	{
		protected static bool Apply( ref Tflag flags, Opcode code )
		{
			if( code.Kind() == Opkind.Access )
			{
				if( code == Opcode.Internal )
				{
					flags |= Tflag.Internal;
				}
				else
				{
					flags = ((Tflag)(unchecked((byte)code) & 3)) | (flags & (~((Tflag)3)));
				}
				return true;
			}
			if( code.Kind() == Opkind.Scope )
			{
				if( ((code == Opcode.Readonly) && (flags.Scope() == Tflag.Static)) || ((code == Opcode.Static) && (flags.Scope() == Tflag.Readonly)) )
				{
					flags = Tflag.Rostatic | (flags & (~Tflag.Scope));
				}
				else
				{
					flags = ((Tflag)((unchecked((byte)code) & 7) << 4)) | (flags & (~Tflag.Scope));
				}
				return true;
			}
			if( code == Opcode.Partial )
			{
				flags |= Tflag.Partial;
				return true;
			}
			if( code == Opcode.Unsafe )
			{
				flags |= Tflag.Unsafe;
				return true;
			}
			return false;
		}//Apply
		
		protected int ClassMark;
		protected string ClassName;
		protected Opcode ClassType;
		protected Tflag ClassFlags;
		protected virtual bool Classes( Flag flags, Tflag lflags = Tflag.None )
		{
			if( (Word == null) || (Indent < Pindent) )
			{
				return false;
			}
			var was = false;
			var one = (flags & Flag.Single) != 0;
			flags &= ~Flag.Single;
			var prevMark = ClassMark;
			var prevName = ClassName;
			var prevType = ClassType;
			var prevFlags = ClassFlags;
			var saved = new SavedAt( this );
			do
			{
				saved.At = At;
				Pindent = Indent;
				var name = Word;
				var type = Opcode.Undef;
				var tflags = lflags;
				if( (((unchecked((byte)Opcode) == unchecked((byte)Opcode.Class)) || (Opcode == Opcode.Face)) || (Opcode == Opcode.Struct)) || (Opcode == Opcode.Enum) )
				{
					type = Opcode;
					name = null;
				}
				else if( ((Opcode != Opcode.Const) && (Opcode != Opcode.Readonly)) && Apply( ref tflags, Opcode ) )
				{
					name = null;
				}
				else if( Opcode != Opcode.Ident )
				{
					break;
				}
				while( (Next( name == null ).Curr == ',') || ((Curr == ':') && (type != 0)) )
				{
				}
				if( Word == null )
				{
					if( (saved.Pi >= 0) && (type != 0) )
					{
						throw new ParseError( this, "Expected word after '{0}'", name );
					}
					goto revert;
				}
				for( ; type == 0; Next( tflags != 0 ) )
				{
					if( Indent < Pindent )
					{
						throw new ParseError( this, "Wrong indentation" );
					}
					if( (Curr == ',') && (tflags != 0) )
					{
						continue;
					}
					if( (((Opcode == Opcode.Class) || (Opcode == Opcode.Face)) || (Opcode == Opcode.Struct)) || (Opcode == Opcode.Enum) )
					{
						type = Opcode;
						Next();
						break;
					}
					if( !Apply( ref tflags, Opcode ) )
					{
						if( tflags != 0 )
						{
							throw new ParseError( this, "Unexpected input in class declaration" );
						}
						goto revert;
					}
				}
				if( name == null )
				{
					while( (Eol || (Curr == ',')) || (Curr == ':') )
					{
						Next( true );
						if( Indent < Pindent )
						{
							throw new ParseError( this, "Wrong indentation" );
						}
					}
					if( Word == null )
					{
						throw new ParseError( this, "Unexpected input in class declaration" );
					}
					name = Word;
					Next();
				}
				var mark = ClassMark = Cgen.ClassStart( ClassName = name );
				var bcnum = 0;
				for( var line = false; ; Next( line ) )
				{
					if( Indent < Pindent )
					{
						throw new ParseError( this, "Wrong indentation" );
					}
					if( (Curr == ',') || (Curr == ':') )
					{
						if( Eol && (Curr == ':') )
						{
							break;
						}
						line = true;
						continue;
					}
					line = false;
					if( !Apply( ref tflags, Opcode ) )
					{
						if( Word == null )
						{
							break;
						}
						if( (((Opcode == Opcode.Class) || (Opcode == Opcode.Face)) || (Opcode == Opcode.Struct)) || (Opcode == Opcode.Enum) )
						{
							if( ((type != 0) && (type != Opcode.Def)) && (type != Opcode.Define) )
							{
								throw new ParseError( this, "More than one class type" );
							}
							type = Opcode;
						}
						else
						{
							FullType( flags );
							bcnum++;
						}
					}
				}
				if( (type == Opcode.Def) || (type == Opcode.Define) )
				{
					type = Opcode.Class;
				}
				ClassType = type;
				ClassFlags = tflags;
				ClassMark = mark = Cgen.ClassBody( mark, name, type, 0, bcnum, tflags );
				Members( name, flags );
				Cgen.ClassEnd( mark, name );
			}
			while( (Indent > saved.Pi) && (!one) );
			Pindent = saved.Pi;
		finish:
			ClassMark = prevMark;
			ClassName = prevName;
			ClassType = prevType;
			ClassFlags = prevFlags;
			return was;
		revert:
			Revert( saved );
			goto finish;
		}//Classes
		
		protected int MemberMark;
		protected string MemberName;
		protected Opcode MemberType;
		protected Tflag MemberFlags;
		protected virtual bool Members( string className, Flag flags )
		{
			if( !Eol )
			{
				throw new ParseError( this, "Unexpected input in class declaration" );
			}
			if( NextLine().Indent < Pindent )
			{
				return false;
			}
			if( ClassFlags.Scope() == Tflag.Static )
			{
				flags |= Flag.Static;
			}
			var was = false;
			var one = (flags & Flag.Single) != 0;
			flags &= ~Flag.Single;
			var lflags = Tflag.None;
			var prevMark = MemberMark;
			var prevName = MemberName;
			var prevType = MemberType;
			var prevFlags = MemberFlags;
			var saved = new SavedAt( this );
			do
			{
				saved.At = At;
				Pindent = Indent;
				var tflags = Tflag.None;
				var isvar = false;
				while( Apply( ref tflags, Opcode ) )
				{
					if( (Opcode == Opcode.Readonly) || (Opcode == Opcode.Const) )
					{
						isvar = true;
					}
					Next( isvar );
				}
				if( !isvar )
				{
					if( Curr == ':' )
					{
						if( tflags == Tflag.None )
						{
							throw new ParseError( this, "Unexpected ':' in class body" );
						}
						Next();
						lflags = tflags;
						continue;
					}
					if( (((unchecked((byte)Opcode) == unchecked((byte)Opcode.Class)) || (Opcode == Opcode.Face)) || (Opcode == Opcode.Struct)) || (Opcode == Opcode.Enum) )
					{
						Revert( saved );
						if( !Classes( flags, lflags ) )
						{
							throw new ParseError( this, "Expected class declaration" );
						}
						was = true;
						continue;
					}
				}
				if( tflags == Tflag.None )
				{
					tflags = lflags;
				}
				else
				{
					if( tflags.Access() == Tflag.None )
					{
						tflags = (tflags & (~Tflag.Access)) | lflags.Access();
					}
					if( tflags.Scope() == Tflag.None )
					{
						tflags = (tflags & (~Tflag.Scope)) | lflags.Scope();
					}
				}
				int mark;
				var name = Word;
				if( (isvar || (Opcode == Opcode.Var)) || (Opcode == Opcode.Event) )
				{
					var evt = (!isvar) && (Opcode == Opcode.Event);
					if( !isvar )
					{
						Next( true );
						while( Apply( ref tflags, Opcode ) )
						{
							Next( true );
						}
					}
					name = Word;
					if( name == null )
					{
						throw new ParseError( this, "Expected field name" );
					}
					MemberFlags = tflags;
					MemberMark = mark = Cgen.FieldStart( name );
					Next();
					while( Apply( ref tflags, Opcode ) )
					{
						Next();
					}
					OptType( flags );
					OptExpr( flags );
					while( Apply( ref tflags, Opcode ) )
					{
						Next();
					}
					Cgen.FieldEnd( mark, name, evt ? Opcode.Event : Opcode.Field, tflags );
					if( !Eof )
					{
						if( !Eol )
						{
							throw new ParseError( this, "Unexpected input in class body" );
						}
						NextLine();
					}
				}
				else
				{
					if( Curr == '.' )
					{
						if( (Next().Word == null) || (((((Word != "ctor") && (Word != "dtor")) && (Word != "new")) && (Word != "create")) && (Word != "init")) )
						{
							throw new ParseError( this, "Expected 'ctor' or 'new' after '.'" );
						}
						name = "." + Word;
					}
					if( name == null )
					{
						throw new ParseError( this, "Unexpected input in class body" );
					}
					MemberMark = Cgen.FuncStart( name );
					Next();
					while( Apply( ref tflags, Opcode ) )
					{
						Next();
					}
					OptType( flags );
					mark = Cgen.FuncTypeEnd( MemberMark, name );
					while( Apply( ref tflags, Opcode ) )
					{
						Next();
					}
					var argc = 0;
					var paren = Curr == '(';
					if( paren || (Curr == ',') )
					{
						Next( true );
					}
					while( (paren || (!Eol)) && (!Eof) )
					{
						if( paren && (Curr == ')') )
						{
							Next();
							break;
						}
						if( Word == null )
						{
							throw new ParseError( this, "Expected argument name" );
						}
						if( (!paren) && Apply( ref tflags, Opcode ) )
						{
							Next();
							while( Apply( ref tflags, Opcode ) )
							{
								Next();
							}
							break;
						}
						var argn = Word;
						var amrk = Cgen.FuncArg( mark, name, argc, argn );
						Next();
						OptType( flags );
						amrk = Cgen.FuncArgDef( mark, amrk, name, argc, argn );
						OptExpr( flags );
						Cgen.FuncArgEnd( mark, amrk, name, argc++, argn );
						if( Curr == ',' )
						{
							Next( true );
						}
					}
					while( Apply( ref tflags, Opcode ) )
					{
						Next();
					}
					MemberFlags = tflags;
					mark = Cgen.FuncBody( MemberMark, name, argc, tflags );
					Block( (flags | Flag.Nosize) | Flag.Member );
					if( MemberType == Opcode.Prop )
					{
						Cgen.PropEnd( mark, name );
					}
					else
					{
						Cgen.FuncEnd( mark, name );
					}
				}
			}
			while( ((!Eof) && (Indent > saved.Pi)) && (!one) );
			Pindent = saved.Pi;
			MemberMark = prevMark;
			MemberName = prevName;
			MemberType = prevType;
			MemberFlags = prevFlags;
			return was;
		}//Members
		
		protected virtual bool Property( Flag flags )
		{
			var code = Opcode;
			var saved = new SavedAt( this );
			var tflags = MemberFlags;
			if( ((!Next().Eol) && (code.Tflag() == 0)) && ((Curr != '.') || (Next().Opcode.Tflag() == 0)) )
			{
				Revert( saved );
				return false;
			}
			if( code.Code() != Opcode.Prop.Code() )
			{
				for( ; ;  )
				{
					if( !Apply( ref tflags, Opcode ) )
					{
						throw new ParseError( this, "Expected access modifier after property accessor" );
					}
					if( !Next().Eol )
					{
						break;
					}
					if( Curr == '.' )
					{
						Next();
					}
				}
			}
			if( !Eol )
			{
				if( Opcode.Code() != Opcode.Prop.Code() )
				{
					throw new ParseError( this, "Expected property accessor after access modifier" );
				}
				code = Opcode;
				while( !Next().Eol )
				{
					if( Curr == '.' )
					{
						Next();
					}
					if( !Apply( ref tflags, Opcode ) )
					{
						throw new ParseError( this, "Expected access modifier after property accessor" );
					}
				}
			}
			var pmrk = Cgen.Func2prop( MemberMark, MemberName );
			saved.At = At;
			Pindent = Indent;
			MemberMark = Cgen.PropFuncStart( pmrk, MemberName, MemberType = code );
			var mark = Cgen.PropFuncBody( pmrk, MemberMark, MemberName, code, tflags );
			Block( flags | Flag.Nosize );
			Cgen.PropFuncEnd( pmrk, mark, MemberName, code );
			while( (!Eof) && (Indent > saved.Pi) )
			{
				saved.At = At;
				code = Opcode;
				tflags = MemberFlags;
				if( ((!Next().Eol) && (code.Tflag() == 0)) && ((Curr != '.') || (Next().Opcode.Tflag() == 0)) )
				{
					throw new ParseError( this, "Expected property accessor" );
				}
				if( code.Code() != Opcode.Prop.Code() )
				{
					for( ; ;  )
					{
						if( !Apply( ref tflags, Opcode ) )
						{
							throw new ParseError( this, "Expected access modifier after property accessor" );
						}
						if( !Next().Eol )
						{
							break;
						}
						if( Curr == '.' )
						{
							Next();
						}
					}
				}
				if( !Eol )
				{
					if( Opcode.Code() != Opcode.Prop.Code() )
					{
						throw new ParseError( this, "Expected property accessor after access modifier" );
					}
					code = Opcode;
					while( !Next().Eol )
					{
						if( Curr == '.' )
						{
							Next();
						}
						if( !Apply( ref tflags, Opcode ) )
						{
							throw new ParseError( this, "Expected access modifier after property accessor" );
						}
					}
				}
				MemberMark = Cgen.PropFuncStart( pmrk, MemberName, MemberType = code );
				mark = Cgen.PropFuncBody( pmrk, MemberMark, MemberName, code, tflags );
				Block( flags | Flag.Nosize );
				Cgen.PropFuncEnd( pmrk, mark, MemberName, code );
			}
			Pindent = saved.Pi;
			MemberType = Opcode.Prop;
			return true;
		}//Property
	}//Parser
}//Bee

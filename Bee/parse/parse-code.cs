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
		protected void FullExpr( Flag flags )
		{
			var mark = Cgen.ExprStart();
			if( ((flags & Flag.Noexpr) == 0) || (!Noexpr( flags )) )
			{
				Expression( flags & (~Flag.Noexpr) );
			}
			Cgen.ExprEnd( mark );
		}//FullExpr
		
		/// <summary>
		/// optional expression
		/// </summary>
		protected Parser OptExpr( Flag flags )
		{
			var mark = Cgen.ExprStart();
			if( (Curr == '=') || (Curr == ':') )
			{
				Next( true );
			}
			else if( Noexpr( flags ) )
			{
				goto skip;
			}
			Expression( flags & (~Flag.Noexpr) );
		skip:
			Cgen.ExprEnd( mark );
			return this;
		}//OptExpr
		
		protected void FullType( Flag flags )
		{
			var mark = Cgen.TypeStart();
			Type( flags );
			Cgen.TypeEnd( mark );
		}//FullType
		
		/// <summary>
		/// optional type reference
		/// </summary>
		protected Parser OptType( Flag flags )
		{
			if( (Opts & (Opt.Typed | Opt.Untyped)) == Opt.Untyped )
			{
				return this;
			}
			var mark = Cgen.TypeStart();
			if( Curr == ':' )
			{
				if( Next( true ).Word == null )
				{
					throw new ParseError( this, "Expected type reference" );
				}
			}
			else if( (Opts & (Opt.Typed | Opt.Untyped)) == (Opt.Typed | Opt.Untyped) )
			{
				goto skip;
			}
			Type( flags );
		skip:
			Cgen.TypeEnd( mark );
			return this;
		}//OptType
		
		/// <summary>
		/// true if there is no expression at current position
		/// (curr == ';' || eol || opcode.kind() >= opkind.statement)
		/// </summary>
		protected virtual bool Noexpr( Flag flags )
		{
			return ((Curr == ';') || Eol) || (Opcode.Kind() >= Opkind.Statement);
		}//Noexpr
		
		protected virtual bool Block( Flag flags )
		{
			var ind = Indent;
			if( (ind == 0) && First )
			{
				ind = -1;
			}
			var nosize = (flags & Flag.Nosize) != 0;
			var member = (flags & Flag.Member) != 0;
			flags &= ~(Flag.Nosize | Flag.Member);
			var block = nosize ? 0 : Cgen.BlockStart();
			var count = 0;
			while( !Eof )
			{
				while( ((Indent >= ind) && (Peek == ':')) && (Opcode == Opcode.Ident) )
				{
					Label( flags );
					count++;
				}
				if( First && (Indent <= ind) )
				{
					break;
				}
				if( Eol )
				{
					NextLine();
					if( (((count == 0) && member) && ((Opcode.Code() == Opcode.Prop.Code()) || (Opcode.Tflag() != 0))) && Property( flags ) )
					{
						break;
					}
					continue;
				}
				while( Curr == ';' )
				{
					Next();
				}
				if( (((Opcode == Opcode.Catch) || (Opcode == Opcode.Finally)) || (Opcode == Opcode.Case)) || (Opcode == Opcode.Default) )
				{
					break;
				}
				if( ((flags & Flag.Wasdo) != 0) && ((Opcode == Opcode.While) || (Opcode == Opcode.Until)) )
				{
					break;
				}
				if( ((flags & Flag.Wasif) != 0) && (Opcode == Opcode.Else) )
				{
					break;
				}
				Statement( flags );
				count++;
			}
			if( !nosize )
			{
				Cgen.BlockEnd( block, count );
			}
			return count > 0;
		}//Block
		
		protected virtual void Label( Flag flags )
		{
			if( Word.Length > 127 )
			{
				throw new ParseError( this, "Label too long" );
			}
			Cgen.Write( Opcode );
			Cgen.Ident( Word );
			Next().Next();
		}//Label
		
		protected virtual void Statement( Flag flags )
		{
			int mark;
			switch( Opcode )
			{
			default:
				if( (Word != null) && (Peek == ':') )
				{
					Label( flags );
					return;
				}
				FullExpr( flags );
				return;
			case Opcode.Goto:
				Next();
				if( Opcode == Opcode.Case )
				{
					Cgen.Write( Opcode.Cgoto );
					Next().FullExpr( flags | Flag.Limit );
					return;
				}
				if( Word == null )
				{
					throw new ParseError( this, "Expected label after goto (or case)" );
				}
				if( Word.Length > 127 )
				{
					throw new ParseError( this, "Label too long" );
				}
				Cgen.Write( Opcode.Goto );
				Cgen.Ident( Word );
				return;
			case Opcode.Return:
				Cgen.Write( Opcode );
				Next().FullExpr( flags | Flag.Noexpr );
				return;
			case Opcode.Raise:
				Cgen.Write( Opcode );
				Next().FullExpr( flags | Flag.Noexpr );
				return;
			case Opcode.Break:
			case Opcode.Continue:
				Cgen.Write( Opcode );
				Next();
				return;
			case Opcode.If:
			case Opcode.Unless:
				Cgen.Write( Opcode );
				Next().FullExpr( flags | Flag.Limit );
				if( ((Curr == ';') || (Curr == ':')) || (Opcode == Opcode.Then) )
				{
					Next();
				}
				Block( flags | Flag.Wasif );
				if( Opcode == Opcode.Else )
				{
					Cgen.Write( Opcode );
					if( Next().Curr == ':' )
					{
						Next();
					}
					Block( flags );
				}
				return;
			case Opcode.Else:
				throw new ParseError( this, "Unexpected 'else'" );
			case Opcode.While:
			case Opcode.Until:
				Cgen.Write( Opcode );
				Next().FullExpr( flags | Flag.Limit );
				if( ((Curr == ';') || (Curr == ':')) || (Opcode == Opcode.Do) )
				{
					Next();
				}
				Block( flags );
				return;
			case Opcode.Do:
				var doat = Cgen.Write( Opcode );
				Next();
				Block( flags | Flag.Wasdo );
				if( Opcode != Opcode.While )
				{
					if( Opcode != Opcode.Until )
					{
						throw new ParseError( this, "Expected 'while' or 'until' for 'do'" );
					}
					Cgen.Write( Opcode.Dountil, doat );
				}
				Next().FullExpr( flags );
				return;
			case Opcode.For:
				var forat = Cgen.Write( Opcode );
				Next();
				FullExpr( (flags | Flag.Limit) | Flag.Noexpr );
				if( (Curr == ':') || (Opcode == Opcode.In) )
				{
					Cgen.Write( Opcode.Foreach, forat );
					Next();
					goto for_in;
				}
				if( Curr == ';' )
				{
					Next();
				}
				FullExpr( (flags | Flag.Limit) | Flag.Noexpr );
				if( Curr == ';' )
				{
					Next();
				}
				if( Noexpr( flags ) )
				{
					Cgen.BlockEnd( Cgen.BlockStart(), 0 );
				}
				else
				{
					mark = Cgen.BlockStart();
					FullExpr( flags | Flag.Limit );
					Cgen.BlockEnd( mark, 1 );
				}
				if( Curr == ';' )
				{
					Next();
				}
				Block( flags );
				return;
			case Opcode.Foreach:
				Cgen.Write( Opcode );
				Next();
				FullExpr( (flags | Flag.Limit) | Flag.Noexpr );
				if( (Curr == ':') || (Opcode == Opcode.In) )
				{
					Next();
				}
			for_in:
				FullExpr( (flags | Flag.Limit) | Flag.Noexpr );
				if( Curr == ';' )
				{
					Next();
				}
				Block( flags );
				return;
			case Opcode.Try:
				Cgen.Write( Opcode.Catch );
				Next().Block( flags );
				mark = Cgen.BlockStart();
				var count = 0;
				while( Opcode == Opcode.Catch )
				{
					Next();
					Cgen.Ident( "" );
					FullType( flags );
					if( (Curr == ';') || (Curr == ':') )
					{
						Next();
					}
					Block( flags );
					count++;
				}
				if( Opcode == Opcode.Else )
				{
					if( (Curr == ';') || (Curr == ':') )
					{
						Next();
					}
					Block( flags );
					count++;
				}
				Cgen.BlockEnd( mark, count );
				if( Opcode != Opcode.Finally )
				{
					Cgen.BlockEnd( Cgen.BlockStart(), 0 );
				}
				else
				{
					Next();
					if( (Curr == ';') || (Curr == ':') )
					{
						Next();
					}
					Block( flags );
				}
				return;
			case Opcode.Switch:
				Cgen.Write( Opcode.Switch );
				Next().FullExpr( flags | Flag.Limit );
				if( (Curr == ';') || (Curr == ':') )
				{
					Next();
				}
				mark = Cgen.BlockStart();
				count = 0;
				for( ; ;  )
				{
					if( Opcode == Opcode.Case )
					{
						Next().FullExpr( flags | Flag.Limit );
						if( (Curr == ';') || (Curr == ':') )
						{
							Next();
						}
						Block( flags );
						count++;
						continue;
					}
					if( Opcode == Opcode.Default )
					{
						Next();
						Cgen.Write( Opcode.Undef );
						if( (Curr == ';') || (Curr == ':') )
						{
							Next();
						}
						Block( flags );
						count++;
						continue;
					}
					break;
				}
				Cgen.BlockEnd( mark, count );
				return;
			case Opcode.Func:
				if( (Opts & Opt.Script) == 0 )
				{
					goto default;
				}
				if( Next().Word == null )
				{
					throw new ParseError( this, "Expected function name" );
				}
				var fname = Word;
				mark = Cgen.FuncStart( fname );
				Next();
				OptType( flags );
				mark = Cgen.FuncTypeEnd( mark, fname );
				var argc = 0;
				var paren = Curr == '(';
				if( paren || (Curr == ',') )
				{
					Next( true );
				}
				while( (paren || (!Eol)) && (!Eof) )
				{
					if( Word == null )
					{
						throw new ParseError( this, "Expected argument name" );
					}
					var argn = Word;
					var argm = Cgen.FuncArg( mark, fname, argc, argn );
					Next();
					OptType( flags );
					argm = Cgen.FuncArgDef( mark, argm, fname, argc, argn );
					OptExpr( flags );
					Cgen.FuncArgEnd( mark, argm, fname, argc++, argn );
					if( paren && (Curr == ')') )
					{
						Next();
						break;
					}
					if( Curr == ',' )
					{
						Next( true );
					}
				}
				mark = Cgen.FuncBody( mark, fname, argc, Tflag.None );
				Block( flags | Flag.Nosize );
				Cgen.FuncEnd( mark, fname );
				return;
			}
		}//Statement
	}//Parser
}//Bee

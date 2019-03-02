using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run
{
	public partial class Engine
	{
		protected override void Statement( Opcode op, byte[] code, ref int at )
		{
			Debug.Assert( (op.Kind() == Opkind.Statement) || (op.Kind() == Opkind.Statement2) );
			switch( op )
			{
			default:
				throw new NotImplementedException();
			case Opcode.Block:
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Push( this );
				}
				Block( code, ref at );
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Pop();
				}
				return;
			case Opcode.Return:
			case Opcode.Raise:
				Expression( code, ref at );
				goto case Opcode.Break;
			case Opcode.Break:
			case Opcode.Continue:
				Exit = op;
				return;
			case Opcode.For:
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Push( this );
				}
				Expression( code, ref at );
				var test = at;
				var notest = code[at] == 0;
				if( notest )
				{
					++at;
				}
				else
				{
					Expression( code, ref at );
				}
				var size = Cint( code, ref at );
				var last = at;
				var stts = at + size;
				var cend = (stts + 4) + Bits.Int( code, stts );
				if( (Value.Type != Vtype.Undef) && (!Value.Bool) )
				{
					at = cend;
					if( (Opts & Opt.BlockScope) != 0 )
					{
						Ctx.Pop();
					}
					return;
				}
				for( ; ;  )
				{
					at = stts;
					Block( code, ref at );
					if( (Exit != 0) && (Exit != Opcode.Continue) )
					{
						break;
					}
					at = last;
					Expression( code, ref at );
					if( !notest )
					{
						at = test;
						Expression( code, ref at );
						if( !Value.Bool )
						{
							break;
						}
					}
				}
				at = cend;
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Pop();
				}
				if( (Exit == Opcode.Break) || (Exit == Opcode.Continue) )
				{
					Exit = 0;
				}
				return;
			case Opcode.While:
			case Opcode.Until:
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Push( this );
				}
				test = at;
				do
				{
					at = test;
					Expression( code, ref at );
					if( Value.Bool == (op == Opcode.Until) )
					{
						break;
					}
					Block( code, ref at );
				}
				while( (Exit == 0) || (Exit == Opcode.Continue) );
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Pop();
				}
				if( (Exit == Opcode.Break) || (Exit == Opcode.Continue) )
				{
					Exit = 0;
				}
				return;
			case Opcode.Do:
			case Opcode.Dountil:
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Push( this );
				}
				do
				{
					Block( code, ref at );
					if( (Exit != 0) && (Exit != Opcode.Continue) )
					{
						break;
					}
					Expression( code, ref at );
				}
				while( Value.Bool != (op == Opcode.Dountil) );
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Pop();
				}
				if( (Exit == Opcode.Break) || (Exit == Opcode.Continue) )
				{
					Exit = 0;
				}
				return;
			case Opcode.If:
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Push( this );
				}
				Expression( code, ref at );
				if( Value.Bool )
				{
					Block( code, ref at );
					if( (at < code.Length) && (code[at] == unchecked((byte)Opcode.Else)) )
					{
						at++;
						size = Cint( code, ref at );
						at += size;
					}
				}
				else
				{
					size = Cint( code, ref at );
					at += size;
					if( (at < code.Length) && (code[at] == unchecked((byte)Opcode.Else)) )
					{
						at++;
						Block( code, ref at );
					}
				}
				if( (Opts & Opt.BlockScope) != 0 )
				{
					Ctx.Pop();
				}
				return;
			}
		}//Statement
		
		protected override void Other( Opcode op, byte[] code, ref int at )
		{
			switch( op )
			{
			default:
				throw new NotImplementedException();
			case Opcode.Func:
				var size = Cint( code, ref at );
				var body = at + size;
				Debug.Assert( code[at + 2] == 0 );
				at += 3;
				var argc = code[at++];
				var fname = Cident( code, ref at );
				var ftsz = Cint( code, ref at );
				var ftat = at;
				at += ftsz;
				var args = argc == 0 ? null : new ArgInfo[argc];
				for( var i = 0; i < argc; i++ )
				{
					var asz = Cint( code, ref at );
					var aend = at + asz;
					args[i].Name = Cident( code, ref at );
					var tsz = Cint( code, ref at );
					args[i].Type = at;
					at += tsz;
					var vsz = Cint( code, ref at );
					args[i].Value = at;
					Debug.Assert( (at + vsz) == aend );
					at = aend;
				}
				Debug.Assert( at == body );
				at = body;
				size = Cint( code, ref at );
				Ctx.Root.Set( fname, new Value( Root.Create( code, at, size, ftat, args, null, Ctx.Vars ) ) );
				at += size;
				Value = new Value( Vtype.Ident, Ctx.Root, fname );
				return;
			}
		}//Other
	}//Engine
}//Bee.Run

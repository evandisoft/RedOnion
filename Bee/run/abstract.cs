using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run
{
	/// <summary>
	/// base class for both runtime engine and source generators
	/// </summary>
	[DebuggerDisplay("{Current} inside {Inside}")]
	public abstract class AbstractEngine
	{
		private Opcode _exit;
		/// <summary>
		/// exit code (of last statement, code block or whole program)
		/// </summary>
		public Opcode Exit
		{
			get
			{
				return _exit;
			}
			protected set
			{
				_exit = value;
			}
		}//Exit
		
		/// <summary>
		/// run script
		/// </summary>
		[Alias("exec")]
		public AbstractEngine Eval( byte[] code )
		{
			Eval( code, 0, code.Length );
			return this;
		}//Eval
		
		/// <summary>
		/// run script
		/// </summary>
		[Alias("exec")]
		public AbstractEngine Eval( byte[] code, int at, int size )
		{
			this.Eval_( code, at, size );
			return this;
		}//Eval
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Eval_( byte[] code, int at, int size )
		{
			Exit = 0;
			var end = at + size;
			while( at < end ){
				Process( code, ref at );
				if( Exit != 0 )
				{
					break;
				}
			}
		}//Eval_
		
		/// <summary>
		/// evaluate expression
		/// </summary>
		public AbstractEngine Expression( byte[] code )
		{
			Expression( code, 0 );
			return this;
		}//Expression
		
		/// <summary>
		/// evaluate expression
		/// </summary>
		public AbstractEngine Expression( byte[] code, int at )
		{
			this.Expression_( code, at );
			return this;
		}//Expression
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Expression_( byte[] code, int at )
		{
			Current = 0;
			Inside = 0;
			Expression( code, ref at );
		}//Expression_
		
		/// <summary>
		/// code of current operation
		/// </summary>
		protected Opcode Current;
		/// <summary>
		/// hint for source generator, unused in real engine
		/// </summary>
		protected Opcode Inside;
		protected abstract void Literal( Opcode op, byte[] code, ref int at );
		
		protected abstract void Binary( Opcode op, byte[] code, ref int at );
		
		protected abstract void Unary( Opcode op, byte[] code, ref int at );
		
		protected abstract void Special( Opcode op, byte[] code, ref int at );
		
		protected abstract void Statement( Opcode op, byte[] code, ref int at );
		
		protected abstract void Other( Opcode op, byte[] code, ref int at );
		
		protected AbstractEngine Block( byte[] code, ref int at )
		{
			this.Block_( code, ref at );
			return this;
		}//Block
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Block_( byte[] code, ref int at )
		{
			Exit = 0;
			var size = Cint( code, ref at );
			var end = at + size;
			while( at < end ){
				Process( code, ref at );
				if( Exit != 0 )
				{
					break;
				}
			}
			at = end;
		}//Block_
		
		protected AbstractEngine Process( byte[] code, ref int at )
		{
			this.Process_( code, ref at );
			return this;
		}//Process
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Process_( byte[] code, ref int at )
		{
			var op = code[at];
			if( op < unchecked((byte)Opkind.Statement) )
			{
				Expression( code, ref at );
				return;
			}
			at++;
			var prev = Inside;
			Inside = Current;
			Current = ((Opcode)op);
			if( op < unchecked((byte)Opkind.Access) )
			{
				Statement( Current, code, ref at );
			}
			else
			{
				Other( Current, code, ref at );
			}
			Current = Inside;
			Inside = prev;
		}//Process_
		
		protected AbstractEngine Expression( byte[] code, ref int at )
		{
			this.Expression_( code, ref at );
			return this;
		}//Expression
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Expression_( byte[] code, ref int at )
		{
			var prev = Inside;
			Inside = Current;
			var op = ((Opcode)code[at++]).Extend();
			Current = op;
			var kind = op.Kind();
			switch( kind )
			{
			case Opkind.Literal:
			case Opkind.Number:
				Literal( op, code, ref at );
				break;
			case Opkind.Special:
				Special( op, code, ref at );
				break;
			default:
				if( kind >= Opkind.Statement )
				{
					throw new InvalidOperationException();
				}
				if( op.Binary() )
				{
					Binary( op, code, ref at );
					break;
				}
				if( op.Unary() )
				{
					Unary( op, code, ref at );
					break;
				}
				Special( op, code, ref at );
				break;
			}
			Current = Inside;
			Inside = prev;
		}//Expression_
		
		protected int Cint( byte[] code, ref int at )
		{
			var v = Bits.Int( code, at );
			at += 4;
			return v;
		}//Cint
		
		protected long Clong( byte[] code, ref int at )
		{
			var v = Bits.Long( code, at );
			at += 8;
			return v;
		}//Clong
		
		protected short Cshort( byte[] code, ref int at )
		{
			var v = Bits.Short( code, at );
			at += 2;
			return v;
		}//Cshort
		
		protected uint Cuint( byte[] code, ref int at )
		{
			var v = Bits.Uint( code, at );
			at += 4;
			return v;
		}//Cuint
		
		protected ulong Culong( byte[] code, ref int at )
		{
			var v = Bits.Ulong( code, at );
			at += 8;
			return v;
		}//Culong
		
		protected ushort Cushort( byte[] code, ref int at )
		{
			var v = Bits.Ushort( code, at );
			at += 2;
			return v;
		}//Cushort
		
		protected float Cfloat( byte[] code, ref int at )
		{
			var v = Bits.Float( code, at );
			at += 4;
			return v;
		}//Cfloat
		
		protected double Cdouble( byte[] code, ref int at )
		{
			var v = Bits.Double( code, at );
			at += 8;
			return v;
		}//Cdouble
		
		protected string Cident( byte[] code, ref int at )
		{
			var x = at;
			var n = code[x++];
			var s = Encoding.UTF8.GetString( code, x, n );
			at = x + n;
			return s;
		}//Cident
	}//AbstractEngine
}//Bee.Run

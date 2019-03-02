using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	/// <summary>
	/// B# source generator from pseudo-code (do not use with compressed code from codeGenerator! pseudoGenerator only!)
	/// </summary>
	[DebuggerDisplay("{Current}/{Inside}: {Sb}")]
	public partial class BsGenerator: Run.AbstractEngine
	{
		private CultureInfo _culture = CultureInfo.InvariantCulture;
		/// <summary>
		/// culture settings for formatting (invariant by default)
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}//Culture
		
		public override string ToString(  )
		{
			return Sb.ToString();
		}//ToString
		
		public BsGenerator Reset(  )
		{
			this.Reset_();
			return this;
		}//Reset
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Reset_(  )
		{
			Sb.Length = 0;
			Current = 0;
			Inside = 0;
			Indent = 0;
		}//Reset_
		
		/// <summary>
		/// generate source
		/// </summary>
		public new BsGenerator Eval( byte[] code, int at, int size )
		{
			this.Eval_( code, at, size );
			return this;
		}//Eval
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Eval_( byte[] code, int at, int size )
		{
			Reset();
			if( size == 0 )
			{
				return;
			}
			var end = at + size;
			for( ; ;  )
			{
				Process( code, ref at );
				if( at >= end )
				{
					break;
				}
				Line();
			}
		}//Eval_
		
		protected StringBuilder Sb = new StringBuilder();
		protected int Indent = 0;
		/// <summary>
		/// append one character to output
		/// </summary>
		protected BsGenerator Write( char @char )
		{
			Sb.Append( @char );
			return this;
		}//Write
		
		/// <summary>
		/// append string to output
		/// </summary>
		protected BsGenerator Write( string @string )
		{
			Sb.Append( @string );
			return this;
		}//Write
		
		/// <summary>
		/// append formatted string to output (according to @culture which is invariant by default)
		/// </summary>
		protected BsGenerator Write( string @string, params object[] @params )
		{
			Sb.AppendFormat( Culture, @string, @params );
			return this;
		}//Write
		
		/// <summary>
		/// end current line and append indentation (immediatelly, make sore to adjust indent before using this!)
		/// </summary>
		protected BsGenerator Line(  )
		{
			Sb.AppendLine();
			if( Indent > 0 )
			{
				Sb.Append( '\t', Indent );
			}
			return this;
		}//Line
	}//BsGenerator
}//Bee

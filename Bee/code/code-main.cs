using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bee
{
	/// <summary>
	/// generates uncompressed pseudo-code (for source generators)
	/// </summary>
	[DebuggerDisplay("Code: {_debuggerDisplay_code}; Vals: {_debuggerDisplay_vals};" + " Parser: {Parser._lnum}:{Parser._at}: {Parser._curr}; {Parser._word}; {Parser._line}")]
	public partial class PseudoGenerator: Parser.IGenerator
	{
		protected Parser Parser;
		public PseudoGenerator()
		{
			this.Parser = new Parser(this);
		}
		
		public PseudoGenerator(Parser.Opt opt)
		{
			this.Parser = new Parser(this);
			this.Parser.Opts = opt;
		}
		
		public PseudoGenerator(Parser.Opt opton, Parser.Opt optoff)
		{
			this.Parser = new Parser(this);
			this.Parser.Opts = (Parser.Opts | opton) & (~optoff);
		}
		
		protected PseudoGenerator(Parser parser)
		{
			this.Parser = parser;
		}
		
		public PseudoGenerator Reset()
		{
			Parser.Reset();
			CodeAt = 0;
			return this;
		}
		
		public bool Eof => Parser.Eof;
		private byte[] _code = new byte[256];
		/// <summary>
		/// buffer for final code (units, classes, methods, statements, expressions)
		/// </summary>
		public byte[] Code
		{
			get => _code;
			private set => _code = value;
		}
		
		private int _codeAt;
		/// <summary>
		/// write position (top) for code buffer
		/// </summary>
		public int CodeAt
		{
			get => _codeAt;
			protected set => _codeAt = value;
		}
		
		/// <summary>
		/// copy code to separate byte array
		/// </summary>
		public byte[] ToArray()
		{
			var arr = new byte[CodeAt];
			Array.Copy(Code, 0, arr, 0, CodeAt);
			return arr;
		}
		
		/// <summary>
		/// compile expression at parser position to code buffer
		/// </summary>
		public PseudoGenerator Expression()
		{
			var at = ValsAt;
			Parser.Expression();
			Rewrite(ValsAt);
			ValsAt = at;
			return this;
		}
		
		/// <summary>
		/// compile type at parser position to code buffer
		/// </summary>
		public PseudoGenerator Type()
		{
			var at = ValsAt;
			Parser.Type();
			Rewrite(ValsAt, true);
			ValsAt = at;
			return this;
		}
		
		/// <summary>
		/// compile provided expression to code buffer
		/// </summary>
		public PseudoGenerator Expression(string @string)
		{
			Parser.Line = @string;
			Expression();
			return this;
		}
		
		/// <summary>
		/// compile provided type to code buffer
		/// </summary>
		public PseudoGenerator Type(string @string)
		{
			Parser.Line = @string;
			Type();
			return this;
		}
		
		/// <summary>
		/// compile full source file / script from string
		/// </summary>
		public PseudoGenerator Unit(string @string)
		{
			Reset();
			Parser.Unit(@string);
			return this;
		}
		
		/// <summary>
		/// compile full source file / script from stream
		/// </summary>
		public PseudoGenerator Unit(TextReader textReader, bool dispose)
		{
			Reset();
			Parser.Unit(textReader, dispose);
			return this;
		}
		
		/// <summary>
		/// compile full source file / script from stream
		/// </summary>
		public PseudoGenerator Unit(Stream stream, bool dispose)
		{
			Reset();
			Parser.Unit(stream, dispose);
			return this;
		}
	}
}

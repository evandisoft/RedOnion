using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	/// <summary>
	/// C# source generator from pseudo-code (do not use with compressed code!)
	/// </summary>
	public partial class CsGenerator: BsGenerator
	{
		[Flags]
		public enum Alias
		{
			None = 0,
			AliasOnly = 1,
			FirstUpper = 2,
			ExceptLocal = 3,
			ExceptVars = 4,
		}//Alias
		
		private Alias _aliasing = Alias.ExceptLocal;
		public Alias Aliasing
		{
			get
			{
				return _aliasing;
			}
			set
			{
				_aliasing = value;
			}
		}//Aliasing
		
		private Tflag _defClassAccess = Tflag.Public;
		public Tflag DefClassAccess
		{
			get
			{
				return _defClassAccess;
			}
			set
			{
				_defClassAccess = value;
			}
		}//DefClassAccess
		
		private Tflag _defMethodAccess = Tflag.Public;
		public Tflag DefMethodAccess
		{
			get
			{
				return _defMethodAccess;
			}
			set
			{
				_defMethodAccess = value;
			}
		}//DefMethodAccess
		
		private Tflag _defPropertyAccess = Tflag.Public;
		public Tflag DefPropertyAccess
		{
			get
			{
				return _defPropertyAccess;
			}
			set
			{
				_defPropertyAccess = value;
			}
		}//DefPropertyAccess
		
		/// <summary>
		/// add local variable
		/// </summary>
		public bool AddLocal( string @string )
		{
			if( Local == null )
			{
				Local = new HashSet<string>();
			}
			else if( ((LocalStack != null) && (LocalStack.Count > 0)) && (Local == LocalStack.Peek()) )
			{
				Local = new HashSet<string>( Local );
			}
			return Local.Add( @string );
		}//AddLocal
		
		/// <summary>
		/// check if local variable of specified name exists
		/// </summary>
		public bool HasLocal( string @string )
		{
			return (Local != null) && Local.Contains( @string );
		}//HasLocal
		
		/// <summary>
		/// save current set of local variables and create new set with same content
		/// </summary>
		public void PushLocal(  )
		{
			if( LocalStack == null )
			{
				LocalStack = new Stack<HashSet<string>>();
			}
			LocalStack.Push( Local );
		}//PushLocal
		
		/// <summary>
		/// load previous (saved/pushed) set of local variables
		/// </summary>
		public void PopLocal(  )
		{
			Local = LocalStack.Pop();
		}//PopLocal
		
		/// <summary>
		/// check if bultin of specified name exists
		/// </summary>
		public bool HasBuiltin( string @string )
		{
			return (Builtins != null) && Builtins.Contains( @string );
		}//HasBuiltin
		
		/// <summary>
		/// check if builtin or local variable of specified name exists
		/// </summary>
		public bool LocalOrBuiltin( string @string )
		{
			return HasBuiltin( @string ) || HasLocal( @string );
		}//LocalOrBuiltin
		
		public HashSet<string> CopyBuiltins(  )
		{
			return new HashSet<string>( Builtins );
		}//CopyBuiltins
		
		public static HashSet<string> CopyCsBuiltins(  )
		{
			return new HashSet<string>( CsBuiltins );
		}//CopyCsBuiltins
		
		private ICollection<string> _builtins = CsBuiltins;
		/// <summary>
		/// bultin literals and types
		/// </summary>
		public ICollection<string> Builtins
		{
			protected get
			{
				return _builtins;
			}
			set
			{
				_builtins = value;
			}
		}//Builtins
		
		private IDictionary<string, string> _aliases = StdAliases;
		/// <summary>
		/// aliases (fully qualified)
		/// </summary>
		public IDictionary<string, string> Aliases
		{
			protected get
			{
				return _aliases;
			}
			set
			{
				_aliases = value;
			}
		}//Aliases
		
		public new CsGenerator Reset(  )
		{
			this.Reset_();
			return this;
		}//Reset
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Reset_(  )
		{
			Space = null;
			WasBlock = false;
			Sb2.Length = 0;
			Name.Length = 0;
			base.Reset_();
		}//Reset_
		
		/// <summary>
		/// generate source
		/// </summary>
		public new CsGenerator Eval( byte[] code, int at, int size )
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
			if( Space != null )
			{
				Indent--;
				Line();
				Write( "}" );
			}
		}//Eval_
		
		/// <summary>
		/// used for name manipulation (first upper, aliases)
		/// </summary>
		protected StringBuilder Name = new StringBuilder();
		protected StringBuilder Sb2 = new StringBuilder();
		protected void Swap(  )
		{
			var sb1 = Sb;
			Sb = Sb2;
			Sb2 = sb1;
		}//Swap
		
		protected void Paste(  )
		{
			Sb.Append( Sb2 );
			Sb2.Length = 0;
		}//Paste
		
		protected HashSet<string> Local;
		protected Stack<HashSet<string>> LocalStack;
		/// <summary>
		/// bultin literals and types
		/// </summary>
		protected static HashSet<string> CsBuiltins = new HashSet<string>( new string[] {
			"null",
			"false",
			"true",
			"string",
			"char",
			"bool",
			"byte",
			"ushort",
			"uint",
			"ulong",
			"sbyte",
			"short",
			"int",
			"long",
			"float",
			"double",
			"decimal"
		} );
		/// <summary>
		/// standard aliases
		/// </summary>
		protected static Dictionary<string, string> StdAliases = new Dictionary<string, string>();
		static CsGenerator(  )
		{
			StdAliases["sys"] = "System";
			StdAliases["System.cols"] = "System.Collections";
			StdAliases["System.Collections.gen"] = "System.Collections.Generic";
			StdAliases["System.Collections.spec"] = "System.Collections.Specialized";
			StdAliases["System.io"] = "System.IO";
			StdAliases["System.IO.zip"] = "System.IO.Compression";
			StdAliases["System.lang"] = "System.Globalization";
			StdAliases["System.model"] = "System.ComponentModel";
			StdAliases["System.cmodel"] = "System.ComponentModel";
			StdAliases["System.run"] = "System.Runtime";
			StdAliases["System.Runtime.i"] = "System.Runtime.InteropServices";
			StdAliases["System.Runtime.is"] = "System.Runtime.InteropServices";
			StdAliases["System.Runtime.inter"] = "System.Runtime.InteropServices";
			StdAliases["System.Runtime.interop"] = "System.Runtime.InteropServices";
			StdAliases["System.interop"] = "System.Runtime.InteropServices";
			StdAliases["System.ref"] = "System.Reflection";
			StdAliases["System.refl"] = "System.Reflection";
			StdAliases["System.reflect"] = "System.Reflection";
			StdAliases["System.diag"] = "System.Diagnostics";
			StdAliases["System.debug"] = "System.Diagnostics";
			StdAliases["System.winforms"] = "System.Windows.Forms";
			StdAliases["System.wf"] = "System.Windows.Forms";
			StdAliases["System.win"] = "System.Windows";
			StdAliases["System.wins"] = "System.Windows";
			StdAliases["ms"] = "Microsoft";
			StdAliases["Microsoft.cs"] = "Microsoft.CSharp";
			StdAliases["Microsoft.vs"] = "Microsoft.VisualStudio";
			StdAliases["Microsoft.VisualStudio.test"] = "Microsoft.VisualStudio.TestTools.UnitTesting";
			StdAliases["Microsoft.VisualStudio.utest"] = "Microsoft.VisualStudio.TestTools.UnitTesting";
		}//.ctor
		
		/// <summary>
		/// change name for C# (remove leading '$' or replace aliases and make first uppercase ...if set in @aliasing)
		/// </summary>
		protected string Unalias( bool afterDot = false, bool type = false )
		{
			var s = Name.ToString();
			Name.Length = 0;
			return Unalias( s, afterDot, type );
		}//Unalias
		
		/// <summary>
		/// change name for C# (remove leading '$' or replace aliases and make first uppercase ...if set in @aliasing)
		/// </summary>
		protected string Unalias( string s, bool afterDot = false, bool type = false )
		{
			Debug.Assert( Name.Length == 0 );
			if( Aliasing == Alias.None )
			{
				if( s[0] == '$' )
				{
					s = s.Substring( 1 );
				}
				if( (!type) && HasBuiltin( s ) )
				{
					s = '@' + s;
				}
				return s;
			}
			for( var pos = 0; pos < s.Length;  )
			{
				var dot = s.IndexOf( '.', pos );
				if( dot < 0 )
				{
					dot = s.Length;
				}
				if( Name.Length > 0 )
				{
					Name.Append( '.' );
				}
				if( s[pos] == '$' )
				{
					var part = s.Substring( pos + 1, (dot - pos) - 1 );
					if( (!type) && HasBuiltin( part ) )
					{
						Name.Append( '@' );
					}
					Name.Append( part );
				}
				else
				{
					var fst = Name.Length;
					var part = s.Substring( pos, dot - pos );
					Name.Append( part );
					string other;
					if( ((!afterDot) && (Aliases != null)) && Aliases.TryGetValue( Name.ToString(), out other ) )
					{
						Name.Length = 0;
						Name.Append( other );
					}
					else if( (Aliasing >= Alias.FirstUpper) && ((((Aliasing == Alias.FirstUpper) || (fst > 0)) || afterDot) || (!HasLocal( Name.ToString() ))) )
					{
						Name[fst] = System.Char.ToUpper( Name[fst] );
					}
					else if( ((fst == 0) && (!type)) && HasBuiltin( part ) )
					{
						Name.Insert( 0, '@' );
					}
				}
				pos = dot + 1;
			}
			s = Name.ToString();
			Name.Length = 0;
			return s;
		}//Unalias
	}//CsGenerator
}//Bee

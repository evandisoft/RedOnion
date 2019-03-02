using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bee.Run.Ecma262
{
	/// <summary>
	/// string function (used to create new string objects)
	/// </summary>
	public class FunctionFun: Obj
	{
		private FunctionObj _prototype;
		/// <summary>
		/// prototype of all function objects
		/// </summary>
		public FunctionObj Prototype
		{
			get
			{
				return _prototype;
			}
		}//Prototype
		
		public FunctionFun( Engine engine, IObject baseClass, FunctionObj prototype )
			: base( engine, baseClass, new Props( "prototype", prototype ) )
		{
			_prototype = prototype;
		}//.ctor
		
		public override Value Call( IObject self, int argc )
		{
			return new Value( Create( argc ) );
		}//Call
		
		public override IObject Create( int argc )
		{
			if( argc == 0 )
			{
				return null;
			}
			string arglist = null;
			for( var i = 0; i < (argc - 1); i++ )
			{
				if( arglist == null )
				{
					arglist = Arg( argc, i ).String;
				}
				else
				{
					arglist += ", " + Arg( argc, i ).String;
				}
			}
			List<Engine.ArgInfo> args = null;
			if( arglist != null )
			{
				var scanner = new Scanner();
				scanner.Line = arglist;
				for( ; ;  )
				{
					if( scanner.Word == null )
					{
						return null;
					}
					if( args == null )
					{
						args = new List<Engine.ArgInfo>();
					}
					args.Add( new Engine.ArgInfo() {
						Name = scanner.Word,
						Type = -1,
						Value = -1
					} );
					if( scanner.Next().Eol )
					{
						scanner.NextLine();
					}
					if( scanner.Eof )
					{
						break;
					}
					if( scanner.Curr != ',' )
					{
						return null;
					}
					if( scanner.Next().Eol )
					{
						scanner.NextLine();
					}
					if( scanner.Eof )
					{
						break;
					}
				}
			}
			var body = Arg( argc, argc - 1 ).String;
			var code = Engine.Compile( body );
			return new FunctionObj( Engine, Prototype, code, 0, code.Length, -1, args?.ToArray(), (Engine.Opts & Engine.Opt.FuncText) == 0 ? null : ((("function anonymous(" + (args == null ? "" : System.String.Join( ", ", args.Select( x => x.Name ) ))) + ") {\n") + body) + "\n}" );
		}//Create
	}//FunctionFun
	
	/// <summary>
	/// function object (callable, can construct)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {_string}")]
	public class FunctionObj: Obj
	{
		private byte[] _code;
		/// <summary>
		/// shared code
		/// </summary>
		public byte[] Code
		{
			get
			{
				return _code;
			}
			protected set
			{
				_code = value;
			}
		}//Code
		
		private int _codeAt;
		/// <summary>
		/// function code position
		/// </summary>
		public int CodeAt
		{
			get
			{
				return _codeAt;
			}
			protected set
			{
				_codeAt = value;
			}
		}//CodeAt
		
		private int _codeSize;
		/// <summary>
		/// function code size
		/// </summary>
		public int CodeSize
		{
			get
			{
				return _codeSize;
			}
			protected set
			{
				_codeSize = value;
			}
		}//CodeSize
		
		private int _typeAt;
		/// <summary>
		/// function type code position
		/// </summary>
		public int TypeAt
		{
			get
			{
				return _typeAt;
			}
			protected set
			{
				_typeAt = value;
			}
		}//TypeAt
		
		private string _arglist;
		/// <summary>
		/// comma-separated list of argument names
		/// </summary>
		public string Arglist
		{
			get
			{
				return _arglist;
			}
			protected set
			{
				_arglist = value;
			}
		}//Arglist
		
		private Engine.ArgInfo[] _args;
		/// <summary>
		/// array of argument names and values (will be null if empty)
		/// </summary>
		protected Engine.ArgInfo[] Args
		{
			get
			{
				return _args;
			}
			set
			{
				_args = value;
			}
		}//Args
		
		private String _string;
		/// <summary>
		/// full function code as string
		/// </summary>
		public String String
		{
			get
			{
				return _string;
			}
			protected set
			{
				_string = value;
			}
		}//String
		
		/// <summary>
		/// get function code as string (if enabled in engine options - opt.funcText)
		/// </summary>
		public override Value Value
		{
			get
			{
				return new Value( this.String );
			}
		}//Value
		
		private IObject _scope;
		/// <summary>
		/// private variables/fields
		/// </summary>
		public IObject Scope
		{
			get
			{
				return _scope;
			}
			protected set
			{
				_scope = value;
			}
		}//Scope
		
		/// <summary>
		/// create function.prototype
		/// </summary>
		public FunctionObj( Engine engine, Obj baseClass )
			: base( engine, baseClass )
		{
		}//.ctor
		
		/// <summary>
		/// create new function object
		/// </summary>
		public FunctionObj( Engine engine, FunctionObj baseClass, byte[] code, int codeAt, int codeSize, int typeAt, Engine.ArgInfo[] args, string @string = null, IObject scope = null )
			: base( engine, baseClass, Stdprops )
		{
			this.Code = code;
			this.CodeAt = codeAt;
			this.CodeSize = codeSize;
			this.TypeAt = typeAt;
			this.Args = args;
			this.Arglist = args == null ? "" : String.Join( ", ", args.Select( x => x.Name ) );
			this.String = @string ?? "function";
			this.Scope = scope;
		}//.ctor
		
		public override Value Call( IObject self, int argc )
		{
			CreateContext( self );
			var args = Ctx.Vars.BaseClass;
			if( this.Args != null )
			{
				for( var i = 0; i < this.Args.Length; i++ )
				{
					args.Set( this.Args[i].Name, i < argc ? Arg( argc, i ) : this.Args[i].Value < 0 ? new Value() : Engine.Expression( Code, this.Args[i].Value ).Result );
				}
			}
			Engine.Eval( Code, CodeAt, CodeSize );
			return DestroyContext();
		}//Call
		
		public override IObject Create( int argc )
		{
			var it = new Obj( Engine, Engine.Box( Get( "prototype" ) ) );
			CreateContext( it );
			var args = Ctx.Vars.BaseClass;
			if( this.Args != null )
			{
				for( var i = 0; i < this.Args.Length; i++ )
				{
					args.Set( this.Args[i].Name, i < argc ? Arg( argc, i ) : this.Args[i].Value < 0 ? new Value() : Engine.Expression( Code, this.Args[i].Value ).Result );
				}
			}
			Engine.Eval( Code, CodeAt, CodeSize );
			DestroyContext();
			return it;
		}//Create
		
		private static Props _stdprops = new Props();
		public static Props Stdprops
		{
			get
			{
				return _stdprops;
			}
		}//Stdprops
	}//FunctionObj
}//Bee.Run.Ecma262

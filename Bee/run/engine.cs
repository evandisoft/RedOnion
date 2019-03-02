using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run
{
	/// <summary>
	/// runtime engine (use only with compressed code from codeGenerator! not from pseudoGenerator!)
	/// </summary>
	public partial class Engine: AbstractEngine
	{
		[Flags]
		public enum Opt
		{
			None = 0,
			BlockScope = 1 << 0,
			FuncText = 1 << 1,
		}//Opt
		
		private Opt _opts;
		/// <summary>
		/// engine options
		/// </summary>
		public Opt Opts
		{
			get
			{
				return _opts;
			}
			set
			{
				_opts = value;
			}
		}//Opts
		
		public struct ArgInfo
		{
			public string Name;
			public int Type;
			public int Value;
		}//ArgInfo
		
		public interface IRoot: IObject
		{
			/// <summary>
			/// box value (stringObj, numberObj, ...)
			/// </summary>
			IObject Box( Value value );
			
			/// <summary>
			/// create new function
			/// </summary>
			IObject Create( byte[] code, int codeAt, int codeSize, int typeAt, ArgInfo[] args, string @string = null, IObject scope = null );
			
			/// <summary>
			/// get typeref (stringFun, numberFun, ...)
			/// </summary>
			Value Get( Opcode opcode );
			
			/// <summary>
			/// get typeref with param (array or generic)
			/// </summary>
			Value Get( Opcode opcode, Value value );
			
			/// <summary>
			/// get typeref with params (array or generic)
			/// </summary>
			Value Get( Opcode opcode, params Value[] par );
		}//IRoot
		
		private IRoot _root;
		/// <summary>
		/// root object (global namespace)
		/// </summary>
		public IRoot Root
		{
			get
			{
				return _root;
			}
			set
			{
				_root = value;
			}
		}//Root
		
		private static Parser.Opt DefaultParserOptions = (Parser.Opt.Script | Parser.Opt.Untyped) | Parser.Opt.Typed;
		public Engine(  )
		{
			Cgen = new CodeGenerator( DefaultParserOptions );
			Root = new Ecma262.Root( this );
			Ctx = new Context( this );
		}//.ctor
		
		public Engine( IRoot root )
		{
			Cgen = new CodeGenerator( DefaultParserOptions );
			this.Root = root;
			Ctx = new Context( this );
		}//.ctor
		
		public Engine( IRoot root, Parser.Opt opt )
		{
			Cgen = new CodeGenerator( opt );
			this.Root = root;
			Ctx = new Context( this );
		}//.ctor
		
		public Engine( IRoot root, Parser.Opt opton, Parser.Opt optoff )
		{
			Cgen = new CodeGenerator( opton, optoff );
			this.Root = root;
			Ctx = new Context( this );
		}//.ctor
		
		/// <summary>
		/// reset engine
		/// </summary>
		public Engine Reset(  )
		{
			this.Reset_();
			return this;
		}//Reset
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Reset_(  )
		{
			Exit = 0;
			Root.Reset();
			Cgen.Reset();
			Args.Clear();
			Ctx = new Context( this );
			Ctxstack.Clear();
		}//Reset_
		
		/// <summary>
		/// result of last expression (rvalue)
		/// </summary>
		public Value Result
		{
			get
			{
				return Value.Type == Vtype.Ident ? ((IProps)Value.Ptr).Get( Value.Str ) : Value;
			}
		}//Result
		
		/// <summary>
		/// compile source to code
		/// </summary>
		public byte[] Compile( string @string )
		{
			byte[] code;
			try
			{
				code = Cgen.Unit( @string ).ToArray();
			}
			finally
			{
				Cgen.Reset();
			}
			return code;
		}//Compile
		
		/// <summary>
		/// run script (given as string)
		/// </summary>
		[Alias("exec")]
		public Engine Eval( string @string )
		{
			Eval( Compile( @string ) );
			return this;
		}//Eval
		
		/// <summary>
		/// box value (stringObj, numberObj, ...)
		/// </summary>
		public virtual IObject Box( Value value )
		{
			for( ; ;  )
			{
				switch( value.Type )
				{
				default:
					return Root.Box( value );
				case Vtype.Object:
					return ((Obj)value.Ptr);
				case Vtype.Ident:
					value = ((IProps)value.Ptr).Get( value.Str );
					continue;
				}
			}
		}//Box
		
		/// <summary>
		/// run script
		/// </summary>
		[Alias("exec")]
		public new Engine Eval( byte[] code )
		{
			Eval( code, 0, code.Length );
			return this;
		}//Eval
		
		[Alias("exec")]
		public new Engine Eval( byte[] code, int at, int size )
		{
			this.Eval_( code, at, size );
			return this;
		}//Eval
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Eval_( byte[] code, int at, int size )
		{
			base.Eval_( code, at, size );
		}//Eval_
		
		/// <summary>
		/// evaluate expression
		/// </summary>
		public new Engine Expression( byte[] code )
		{
			Expression( code, 0 );
			return this;
		}//Expression
		
		/// <summary>
		/// evaluate expression
		/// </summary>
		public new Engine Expression( byte[] code, int at )
		{
			this.Expression_( code, at );
			return this;
		}//Expression
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Expression_( byte[] code, int at )
		{
			base.Expression_( code, at );
		}//Expression_
		
		protected new Engine Block( byte[] code, ref int at )
		{
			this.Block_( code, ref at );
			return this;
		}//Block
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Block_( byte[] code, ref int at )
		{
			Exit = Opcode.Undef;
			base.Block_( code, ref at );
		}//Block_
		
		/// <summary>
		/// result of last expression (lvalue)
		/// </summary>
		protected Value Value;
		/// <summary>
		/// code generator
		/// </summary>
		protected CodeGenerator Cgen;
		/// <summary>
		/// argument list for function calls
		/// </summary>
		protected internal Arglist Args = new Arglist();
		/// <summary>
		/// argument list for function calls
		/// </summary>
		public class Arglist: List<Value>
		{
			public int Length
			{
				get
				{
					return Count;
				}
			}//Length
			
			public void Remove( int last )
			{
				RemoveRange( Count - last, last );
			}//Remove
			
			public Value Arg( int argc, int @int = 0 )
			{
				var idx = (Count - argc) + @int;
				return idx < Count ? this[idx] : new Value();
			}//Arg
		}//Arglist
		
		/// <summary>
		/// stack of blocks of current function/method
		/// </summary>
		public struct Context: IProps
		{
			private IObject _self;
			/// <summary>
			/// current object accessible by 'this' keyword
			/// </summary>
			public IObject Self
			{
				get
				{
					return _self;
				}
			}//Self
			
			private IObject _vars;
			/// <summary>
			/// variables of current block (previous block/scope is in baseClass)
			/// </summary>
			public IObject Vars
			{
				get
				{
					return _vars;
				}
			}//Vars
			
			private IObject _root;
			/// <summary>
			/// root (activation) object (new variables not declared with var will be created here)
			/// </summary>
			public IObject Root
			{
				get
				{
					return _root;
				}
			}//Root
			
			/// <summary>
			/// root context
			/// </summary>
			internal Context( Engine engine )
			{
				_self = _vars = (_root = engine.Root);
			}//.ctor
			
			/// <summary>
			/// function execution context
			/// </summary>
			internal Context( Engine engine, IObject self, IObject scope )
			{
				_self = self ?? engine.Root;
				_root = _vars = engine.CreateVars( engine.CreateVars( scope ?? engine.Root ) );
				_vars.Set( "arguments", new Value( Vars.BaseClass ) );
			}//.ctor
			
			public void Push( Engine engine )
			{
				_vars = engine.CreateVars( Vars );
			}//Push
			
			public void Pop(  )
			{
				_vars = Vars.BaseClass;
			}//Pop
			
			public bool Has( string @string )
			{
				return Vars.Has( @string );
			}//Has
			
			public IObject Which( string @string )
			{
				return Vars.Which( @string );
			}//Which
			
			public Value Get( string @string )
			{
				return Vars.Get( @string );
			}//Get
			
			public bool Get( string @string, out Value value )
			{
				return Vars.Get( @string, out value );
			}//Get
			
			public bool Set( string @string, Value value )
			{
				return Vars.Set( @string, value );
			}//Set
			
			public bool Del( string @string )
			{
				return Vars.Del( @string );
			}//Del
			
			public void Reset(  )
			{
				Vars.Reset();
			}//Reset
		}//Context
		
		/// <summary>
		/// current context (method)
		/// </summary>
		protected internal Context Ctx;
		/// <summary>
		/// stack of contexts (methods)
		/// </summary>
		protected internal Stack<Context> Ctxstack = new Stack<Context>();
		/// <summary>
		/// create new execution/activation context (for function call)
		/// </summary>
		protected internal void CreateContext( IObject self )
		{
			CreateContext( self, Ctx.Vars );
		}//CreateContext
		
		/// <summary>
		/// create new execution/activation context (for function call with scope - usually function inside function)
		/// </summary>
		protected internal void CreateContext( IObject self, IObject scope )
		{
			Ctxstack.Push( Ctx );
			Ctx = new Context( this, self, scope );
		}//CreateContext
		
		/// <summary>
		/// create new variables holder object
		/// </summary>
		protected internal virtual IObject CreateVars( IObject vars )
		{
			return new Obj( this, vars );
		}//CreateVars
		
		/// <summary>
		/// destroy last execution/activation context
		/// </summary>
		protected internal Value DestroyContext(  )
		{
			Ctx = Ctxstack.Pop();
			var value = Exit == Opcode.Return ? Result : new Value();
			if( Exit != Opcode.Raise )
			{
				Exit = Opcode.Undef;
			}
			return value;
		}//DestroyContext
	}//Engine
}//Bee.Run

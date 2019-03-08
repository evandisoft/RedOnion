using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedOnion.Script.Parsing;

namespace RedOnion.Script
{
	/// <summary>
	/// Runtime engine
	/// </summary>
	public partial class Engine : AbstractEngine
	{
		[Flags]
		public enum Option
		{
			None = 0,
			BlockScope	= 1 << 0,
			FuncText	= 1 << 1,
		}

		/// <summary>
		/// Engine options
		/// </summary>
		public Option Options { get; set; } = Option.BlockScope;

		public struct ArgInfo
		{
			public string Name;
			public int Type;
			public int Value;
		}

		public interface IRoot : IObject
		{
			/// <summary>
			/// Box value (StringObj, NumberObj, ...)
			/// </summary>
			IObject Box(Value value);

			/// <summary>
			/// Create new function
			/// </summary>
			IObject Create(string[] strings, byte[] code, int codeAt, int codeSize, int typeAt, ArgInfo[] args, string body = null, IObject scope = null);

			/// <summary>
			/// Get type reference (StringFun, NumberFun, ...)
			/// </summary>
			Value Get(OpCode OpCode);

			/// <summary>
			/// Get type reference with parameter (array or generic)
			/// </summary>
			Value Get(OpCode OpCode, Value value);

			/// <summary>
			/// Get type reference with parameter (array or generic)
			/// </summary>
			Value Get(OpCode OpCode, params Value[] par);
		}

		/// <summary>
		/// Root object (global namespace)
		/// </summary>
		public IRoot Root { get; set; }

		/// <summary>
		/// Every statement or function call will decrease this (if positive)
		/// and will throw exception if it reaches zero
		/// </summary>
		/// <remarks>Indexing and creation of objects counted as well</remarks>
		public int ExecutionCountdown;
		public class TookTooLong : Exception
		{
			public TookTooLong() : base("Took too long") { }
		}
		protected void CountStatement()
		{
			if (ExecutionCountdown > 0 && --ExecutionCountdown == 0)
				throw new TookTooLong();
		}

		private static Parser.Option DefaultParserOptions =
			Parser.Option.Script | Parser.Option.Untyped | Parser.Option.Typed;

		public Engine()
		{
			Parser = new Parser(DefaultParserOptions);
			Root = new BasicObjects.Root(this);
			Ctx = new Context(this);
		}

		public Engine(IRoot root)
		{
			Parser = new Parser(DefaultParserOptions);
			Root = root;
			Ctx = new Context(this);
		}

		public Engine(IRoot root, Parser.Option opt)
		{
			Parser = new Parser(opt);
			Root = root;
			Ctx = new Context(this);
		}

		public Engine(IRoot root, Parser.Option opton, Parser.Option optoff)
		{
			Parser = new Parser(opton, optoff);
			Root = root;
			Ctx = new Context(this);
		}

		/// <summary>
		/// Reset engine
		/// </summary>
		public virtual void Reset()
		{
			Exit = 0;
			Root.Reset();
			Parser.Reset();
			Args.Clear();
			Ctx = new Context(this);
			CtxStack.Clear();
		}

		/// <summary>
		/// Result of last expression (rvalue)
		/// </summary>
		public Value Result => Value.Type == ValueKind.Reference
			? ((IProperties)Value.ptr).Get(Value.str)
			: Value;

		/// <summary>
		/// Compile source to code
		/// </summary>
		public byte[] Compile(string source, out string[] strings)
		{
			byte[] code;
			try
			{
				Parser.Unit(source);
				code = Parser.CodeToArray();
				strings = Parser.StringsToArray();
			}
			finally
			{
				Parser.Reset();
			}
			return code;
		}

		/// <summary>
		/// Run script (given as string)
		/// </summary>
		public Engine Execute(string source)
		{
			var code = Compile(source, out var strings);
			Execute(strings, code);
			return this;
		}

		/// <summary>
		/// Run script
		/// </summary>
		public new Engine Execute(string[] strings, byte[] code)
			=> Execute(strings, code, 0, code.Length);

		/// <summary>
		/// Run script
		/// </summary>
		public new Engine Execute(string[] strings, byte[] code, int at, int size)
		{
			ExecCode(strings, code, at, size);
			return this;
		}

		/// <summary>
		/// Evaluate expression
		/// </summary>
		public new Engine Expression(string[] strings, byte[] code)
			=> Expression(strings, code, 0);

		/// <summary>
		/// Evaluate expression
		/// </summary>
		public new Engine Expression(string[] strings, byte[] code, int at)
		{
			EvalExpression(strings, code, at);
			return this;
		}

		/// <summary>
		/// Box value (StringObj, NumberObj, ...)
		/// </summary>
		public virtual IObject Box(Value value)
		{
			for (;;)
			{
				switch (value.Type)
				{
				default:
					return Root.Box(value);
				case ValueKind.Object:
					return (IObject)value.ptr;
				case ValueKind.Reference:
					value = ((IProperties)value.ptr).Get(value.str);
					continue;
				}
			}
		}

		/// <summary>
		/// Result of last expression (lvalue)
		/// </summary>
		protected Value Value;
		/// <summary>
		/// Parser
		/// </summary>
		protected Parser Parser;
		/// <summary>
		/// Argument list for function calls
		/// </summary>
		protected internal Arglist Args = new Arglist();
		/// <summary>
		/// Argument list for function calls
		/// </summary>
		public class Arglist : List<Value>
		{
			public int Length => Count;
			public void Remove(int last)
				=> RemoveRange(Count - last, last);

			public Value Arg(int argc, int n = 0)
			{
				var idx = Count - argc + n;
				return idx < Count ? this[idx] : new Value();
			}
		}

		/// <summary>
		/// Stack of blocks of current function/method
		/// </summary>
		public struct Context : IProperties
		{
			/// <summary>
			/// Current object accessible by 'this' keyword
			/// </summary>
			public IObject Self { get; }

			/// <summary>
			/// Variables of current block (previous block/scope is in baseClass)
			/// </summary>
			public IObject Vars { get; private set; }

			/// <summary>
			/// Root (activation) object (new variables not declared with var will be created here)
			/// </summary>
			public IObject Root { get; }

			/// <summary>
			/// Root context
			/// </summary>
			internal Context(Engine engine)
			{
				Self = Vars = Root = engine.Root;
			}

			/// <summary>
			/// Function execution context
			/// </summary>
			internal Context(Engine engine, IObject self, IObject scope)
			{
				Self = self ?? engine.Root;
				Root = Vars = engine.CreateVars(engine.CreateVars(scope ?? engine.Root));
				Vars.Set("arguments", new Value(Vars.BaseClass));
			}

			public void Push(Engine engine)
				=> Vars = engine.CreateVars(Vars);
			public void Pop()
				=> Vars = Vars.BaseClass;
			public bool Has(string name)
				=> Vars.Has(name);
			public IObject Which(string name)
				=> Vars.Which(name);
			public Value Get(string name)
				=> Vars.Get(name);
			public bool Get(string name, out Value value)
				=> Vars.Get(name, out value);
			public bool Set(string name, Value value)
				=> Vars.Set(name, value);
			public bool Delete(string name)
				=> Vars.Delete(name);
			public void Reset()
				=> Vars.Reset();
		}

		/// <summary>
		/// Current context (method)
		/// </summary>
		protected internal Context Ctx;
		/// <summary>
		/// Stack of contexts (methods)
		/// </summary>
		protected internal Stack<Context> CtxStack = new Stack<Context>();
		/// <summary>
		/// Create new execution/activation context (for function call)
		/// </summary>
		protected internal void CreateContext(IObject self)
			=> CreateContext(self, Ctx.Vars);

		/// <summary>
		/// Create new execution/activation context (for function call with scope - usually function inside function)
		/// </summary>
		protected internal void CreateContext(IObject self, IObject scope)
		{
			CtxStack.Push(Ctx);
			Ctx = new Context(this, self, scope);
		}

		/// <summary>
		/// Create new variables holder object
		/// </summary>
		protected internal virtual IObject CreateVars(IObject vars)
			=> new BasicObjects.BasicObject(this, vars);

		/// <summary>
		/// Destroy last execution/activation context
		/// </summary>
		protected internal Value DestroyContext()
		{
			Ctx = CtxStack.Pop();
			var value = Exit == OpCode.Return ? Result : new Value();
			if (Exit != OpCode.Raise)
				Exit = OpCode.Undefined;
			return value;
		}
	}
}

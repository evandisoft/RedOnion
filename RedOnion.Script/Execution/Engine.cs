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
	public partial class Engine : AbstractEngine, IEngine
	{
		/// <summary>
		/// Engine options
		/// </summary>
		public EngineOption Options { get; set; }
			= EngineOption.BlockScope | EngineOption.Strict;
		public bool HasOption(EngineOption option) => (Options & option) != 0;

		/// <summary>
		/// Root object (global namespace)
		/// </summary>
		public IEngineRoot Root { get; set; }

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
			Context = new EngineContext(this);
		}

		public Engine(Func<IEngine, IEngineRoot> createRoot)
		{
			Parser = new Parser(DefaultParserOptions);
			Root = createRoot(this);
			Context = new EngineContext(this);
		}

		public Engine(Func<IEngine, IEngineRoot> createRoot, Parser.Option opt)
		{
			Parser = new Parser(opt);
			Root = createRoot(this);
			Context = new EngineContext(this);
		}

		public Engine(Func<IEngine, IEngineRoot> createRoot, Parser.Option opton, Parser.Option optoff)
		{
			Parser = new Parser(opton, optoff);
			Root = createRoot(this);
			Context = new EngineContext(this);
		}

		/// <summary>
		/// Run script in a string
		/// </summary>
		public void Execute(string source)
			=> Execute(Compile(source));

		/// <summary>
		/// Reset engine
		/// </summary>
		public virtual void Reset()
		{
			Exit = 0;
			Root.Reset();
			Parser.Reset();
			Arguments.Clear();
			Context = new EngineContext(this);
			ContextStack.Clear();
		}

		/// <summary>
		/// Compile source to code
		/// </summary>
		public CompiledCode Compile(string source)
		{
			string[] strings;
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
			return new CompiledCode(strings, code)
			{
				Source = source
			};
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
		/// Parser
		/// </summary>
		protected Parser Parser;
		/// <summary>
		/// Argument list for function calls
		/// </summary>
		public ArgumentList Arguments { get; } = new ArgumentList();

		/// <summary>
		/// Current context (method)
		/// </summary>
		protected EngineContext Context;
		/// <summary>
		/// Stack of contexts (methods)
		/// </summary>
		protected Stack<EngineContext> ContextStack = new Stack<EngineContext>();

		/// <summary>
		/// Create new execution/activation context (for function call)
		/// </summary>
		public IObject CreateContext(IObject self, IObject scope = null)
		{
			ContextStack.Push(Context);
			Context = new EngineContext(this, self, scope);
			return Context.Vars.BaseClass;
		}

		/// <summary>
		/// Create new variables holder object
		/// </summary>
		public virtual IObject CreateVars(IObject vars)
			=> new BasicObjects.BasicObject(this, vars);

		/// <summary>
		/// Destroy last execution/activation context
		/// </summary>
		public Value DestroyContext()
		{
			Context = ContextStack.Pop();
			var value = Exit == OpCode.Return ? Result : new Value();
			if (Exit != OpCode.Raise)
				Exit = OpCode.Undefined;
			return value;
		}
	}
}

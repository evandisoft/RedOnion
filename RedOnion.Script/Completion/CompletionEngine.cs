using System;
using System.Collections.Generic;
using RedOnion.Script.Parsing;

namespace RedOnion.Script.Completion
{
	public class CompletionEngine : IEngine
	{
		/// <summary>
		/// Completion/replacement suggestions for point of interest
		/// </summary>
		public ArraySegment<string> Suggestions
			=> new ArraySegment<string>(_suggestions, 0, _suggestionsCount);
		protected string[] _suggestions = new string[64];
		protected int _suggestionsCount;
		/// <summary>
		/// Get copy of the suggestions
		/// (unfortunately ArraySegment is not IList in .NET 3.5)
		/// </summary>
		/// <returns></returns>
		public string[] GetSuggestions()
		{
			string[] it = new string[_suggestionsCount];
			Array.Copy(_suggestions, it, it.Length);
			return it;
		}

		public CompletionEngine()
			: this(engine => new CompletionRoot(engine)) { }
		public CompletionEngine(Func<IEngine, IEngineRoot> createRoot)
			: this(createRoot, Engine.DefaultParserOptions) { }
		public CompletionEngine(Func<IEngine, IEngineRoot> createRoot, Parser.Option options)
		{
		}

		public virtual IList<string> Complete(
			string source, int at, out int replaceFrom, out int replaceTo)
		{
			Reset();
			//Interest = at;
			//Execute(source);
			replaceFrom = at; //Parser.TokenStart;
			replaceTo = at; //Parser.TokenEnd;
			return GetSuggestions();
		}
		public virtual string Documentation(string source, int at)
			=> "TODO";

		/// <summary>
		/// Engine options
		/// </summary>
		public EngineOption Options { get; set; }
			= Engine.DefaultOptions;
		public bool HasOption(EngineOption option) => (Options & option) != 0;

		/// <summary>
		/// Root object (global namespace)
		/// </summary>
		public IEngineRoot Root { get; set; }
		/// <summary>
		/// Exit code (of last statement, code block or whole program)
		/// </summary>
		public OpCode Exit { get; protected set; }
		/// <summary>
		/// Result of last expression (lvalue)
		/// </summary>
		protected Value Value;
		/// <summary>
		/// Result of last expression (rvalue)
		/// </summary>
		public Value Result => Value.RValue;
		/// <summary>
		/// Argument list for function calls
		/// </summary>
		public ArgumentList Arguments { get; } = new ArgumentList();

		/// <summary>
		/// Reset engine
		/// </summary>
		public virtual void Reset()
		{
			Exit = 0;
			Root.Reset();
			//Parser.Reset();
			Arguments.Clear();
			Context = new EngineContext(this);
			ContextStack.Clear();
		}

		CompiledCode IEngine.Compile(string source)
			=> throw new NotImplementedException();
		void IEngine.Execute(CompiledCode code, int at, int size)
			=> throw new NotImplementedException();
		Value IEngine.Evaluate(CompiledCode code, int at)
			=> throw new NotImplementedException();

		/// <summary>
		/// Box value (StringObj, NumberObj, ...)
		/// </summary>
		public virtual IObject Box(Value value)
		{
			for (; ; )
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

		public virtual void Log(string msg) { }
	}
}

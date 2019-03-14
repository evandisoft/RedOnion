using System;
using System.Collections.Generic;
using System.Reflection;
using RedOnion.Script.Parsing;

namespace RedOnion.Script.Completion
{
	public partial class CompletionEngine : IEngine
	{
		/// <summary>
		/// Completion/replacement suggestions for point of interest
		/// </summary>
		public ArraySegment<string> Suggestions
			=> new ArraySegment<string>(_suggestions, 0, _suggestionsCount);
		protected string[] _suggestions = new string[64];
		protected int _suggestionsCount;
		protected void AddSuggestion(string name)
		{
			if (_suggestionsCount == _suggestions.Length)
				Array.Resize(ref _suggestions, _suggestions.Length << 1);
			_suggestions[_suggestionsCount++] = name;
		}
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

		protected IEngineRoot linked;
		public CompletionEngine(IEngine linked)
			: this(linked?.Root) { }
		public CompletionEngine(IEngineRoot linked)
			=> Root = new CompletionRoot(this, this.linked = linked);

		protected Lexer lexer = new Lexer();
		protected int interest, replaceAt, replaceEnd;
		protected IObject found;

		public virtual IList<string> Complete(
			string source, int at, out int replaceFrom, out int replaceTo)
		{
			Reset();
			interest = at;
			this.replaceAt = at;
			this.replaceEnd = at;
			lexer.Source = source;
			Execute();
			replaceFrom = this.replaceAt;
			replaceTo = this.replaceEnd;
			if (found != null)
			{
				FillFrom(found);
				RemoveDuplicates();
			}
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
			Arguments.Clear();
			Context = new EngineContext(this);
			ContextStack.Clear();
			if (_suggestionsCount > 0)
			{
				Array.Clear(_suggestions, 0, _suggestionsCount);
				_suggestionsCount = 0;
			}
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

		private void FillFrom(IObject obj)
		{
			if (obj == null)
				return;
			for (; ; )
			{
				FillFrom(obj.BaseProps);
				FillFrom(obj.MoreProps);
				if (obj.HasFeature(ObjectFeatures.TypeReference|ObjectFeatures.Proxy))
				{
					Type type = obj.Type;
					if (type != null)
					{
						var binding = BindingFlags.Public;
						if (obj.HasFeature(ObjectFeatures.TypeReference))
							binding |= BindingFlags.Static;
						if (obj.HasFeature(ObjectFeatures.Proxy))
							binding |= BindingFlags.Instance;
						foreach (var member in type.GetMembers(binding))
						{
							if (member is ConstructorInfo)
								continue;
							AddSuggestion(member.Name);
						}
					}
				}
				bool wasRoot = obj == Root;
				if ((obj = obj.BaseClass) != null)
					continue;
				if (!wasRoot)
					break;
				obj = linked;
				if (obj == null)
					return;
			}
		}
		private void FillFrom(IProperties iprops)
		{
			if (!(iprops is Properties props))
				return;
			foreach (var name in props.Keys)
				AddSuggestion(name);
		}
		private void RemoveDuplicates(string prefix = null)
		{
			if (_suggestionsCount <= 1)
				return;
			int i, j;
			if (prefix != null && prefix.Length > 0)
			{
				for (i = 0, j = i + 1; j < _suggestionsCount; j++)
				{
					if (!_suggestions[j].StartsWith(prefix,
						StringComparison.OrdinalIgnoreCase))
						_suggestions[++i] = _suggestions[j];
				}
				_suggestionsCount = i + 1;
			}
			Array.Sort(_suggestions, 0, _suggestionsCount, StringComparer.OrdinalIgnoreCase);
			for (i = 0, j = i + 1; j < _suggestionsCount; j++)
			{
				if (string.Compare(_suggestions[j], _suggestions[i],
					StringComparison.OrdinalIgnoreCase) != 0)
					_suggestions[++i] = _suggestions[j];
			}
			_suggestionsCount = i + 1;
		}
	}
}

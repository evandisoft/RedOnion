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
		protected int interest, replaceAt, replaceTo;
		protected object found;

		public virtual IList<string> Complete(
			string source, int at, out int replaceAt, out int replaceTo)
		{
			Reset();
			interest = at;
			this.replaceAt = at;
			this.replaceTo = at;
			lexer.Source = source;

			try
			{
				Execute();
			}
			catch (Exception ex)
			{
				this.DebugLog("{0} in completion: {1}", ex.GetType().Name, ex.Message);
				replaceAt = at;
				replaceTo = at;
				return new string[0];
			}

			replaceAt = this.replaceAt;
			replaceTo = this.replaceTo;
			if (found != null)
			{
				FillFrom(found);
				RemoveDuplicates(at == replaceAt ? null : source.Substring(replaceAt, at-replaceAt));
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
			if (value.IsReference)
				value = value.RValue;
			if (value.Kind == ValueKind.Object)
				return (IObject)value.ptr;
			return Root.Box(value);
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
		public IObject CreateContext(IObject self, IScope scope = null)
		{
			ContextStack.Push(Context);
			Context = new EngineContext(this, self, scope);
			return Context.Vars.BaseClass;
		}

		/// <summary>
		/// Create new variables holder object
		/// </summary>
		public virtual IScope CreateVars(IObject baseClass)
			=> new BasicObjects.BasicObject(this, baseClass);

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

		private void FillFrom(object value)
		{
			if (value == null)
				return;
			for (; ; )
			{
				var type = value as Type;
				var props = value as IProperties;
				IObject obj = null;
				if (props != null)
				{
					obj = props as IObject;
					if (obj != null)
					{
						FillFrom(obj.BaseProps);
						FillFrom(obj.MoreProps);
						if (obj.HasFeature(ObjectFeatures.TypeReference|ObjectFeatures.Proxy))
							type = obj.Type;
					}
					else
					{
						if (props is IDictionary<string, Value> dict)
						{
							foreach (var name in dict.Keys)
								AddSuggestion(name);
						}
						return;
					}
				}
				if (type != null)
				{
					var binding = BindingFlags.Public;
					if (obj != null && obj.HasFeature(ObjectFeatures.TypeReference))
						binding |= BindingFlags.Static;
					if (obj == null || obj.HasFeature(ObjectFeatures.Proxy))
						binding |= BindingFlags.Instance;
					foreach (var member in type.GetMembers(binding))
					{
						if (member.MemberType == MemberTypes.Constructor)
							continue;
						if (member.MemberType == MemberTypes.Method
							&& ((MethodInfo)member).IsSpecialName)
							continue;
						AddSuggestion(member.Name);
					}
				}
				bool wasRoot = value == Root;
				if (obj != null && (value = obj.BaseClass) != null)
					continue;
				if (!wasRoot)
					break;
				value = linked;
				if (value == null)
					return;
			}
		}
		private void RemoveDuplicates(string prefix = null)
		{
			int i, j;
			if (prefix != null && prefix.Length > 0)
			{
				for (i = 0, j = 0; j < _suggestionsCount; j++)
				{
					if (_suggestions[j].StartsWith(prefix,
						StringComparison.OrdinalIgnoreCase))
						_suggestions[i++] = _suggestions[j];
				}
				_suggestionsCount = i;
			}
			if (_suggestionsCount <= 1)
				return;
			Array.Sort(_suggestions, 0, _suggestionsCount, StringComparer.OrdinalIgnoreCase);
			for (i = 0, j = 1; j < _suggestionsCount; j++)
			{
				if (string.Compare(_suggestions[j], _suggestions[i],
					StringComparison.OrdinalIgnoreCase) != 0)
					_suggestions[++i] = _suggestions[j];
			}
			_suggestionsCount = i + 1;
		}
	}
}

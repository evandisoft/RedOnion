using System;
using RedOnion.Script.Parsing;

namespace RedOnion.Script.Completion
{
	public class CompletionEngine : Engine<CompletionParser>
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

		public CompletionEngine(Func<IEngine, IEngineRoot> createRoot)
			: base(createRoot, new CompletionParser(Engine.DefaultParserOptions)) { }
		public CompletionEngine(Func<IEngine, IEngineRoot> createRoot, Parser.Option options)
			: base(createRoot, new CompletionParser(options)) { }
	}
}

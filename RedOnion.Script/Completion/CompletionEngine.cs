using System;
using RedOnion.Script.Parsing;

namespace RedOnion.Script.Completion
{
	public class CompletionEngine : Engine
	{
		public CompletionEngine(Func<IEngine, IEngineRoot> createRoot)
			: base(createRoot, new CompletionParser(Engine.DefaultParserOptions)) { }
		public CompletionEngine(Func<IEngine, IEngineRoot> createRoot, Parser.Option options)
			: base(createRoot, new CompletionParser(options)) { }
	}
}

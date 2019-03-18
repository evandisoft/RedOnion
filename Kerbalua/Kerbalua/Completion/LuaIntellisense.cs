using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Kerbalua.Utility;
using System.Collections.Generic;

namespace Kerbalua.Completion {
	public class LuaIntellisense {
		public LuaIntellisense()
		{
		}

		static public ParsedIncompleteVar Parse(string str)
		{
			ICharStream stream = CharStreams.fromstring(str);
			ITokenSource lexer = new IncompleteLuaLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);

			var parser = new IncompleteLuaParser(tokens) {
				BuildParseTree = true
			};

			IParseTree tree = parser.incompleteChunk();
			
			var intellisenseListener = new LastIncompleteVarExtractor();
			ParseTreeWalker.Default.Walk(intellisenseListener, tree);
			var context = intellisenseListener.LastIncompleteVar;

			var parsedResult = new ParsedIncompleteVar(context);

			return parsedResult;
		}

		class LastIncompleteVarExtractor : IncompleteLuaBaseListener {
			public IncompleteLuaParser.IncompleteVarContext LastIncompleteVar;

			public override void EnterIncompleteVar([NotNull] IncompleteLuaParser.IncompleteVarContext context)
			{
				LastIncompleteVar = context;
			}
		}

		//class ReversedTokenStream : ITokenStream {
		//	ITokenStream stream;

		//	public ReversedTokenStream(ITokenStream stream)
		//	{
		//		this.stream = stream;
		//	}

		//	public ITokenSource TokenSource => stream.TokenSource;

		//	public int Index => Size - stream.Index;

		//	public int Size => stream.Size;

		//	public string SourceName => stream.SourceName;

		//	public void Consume()
		//	{
		//		stream.Consume();
		//	}

		//	[return: NotNull]
		//	public IToken Get(int i)
		//	{
		//		return stream.Get(Size - i);
		//	}

		//	[return: NotNull]
		//	public string GetText(Interval interval)
		//	{
		//		Interval newInterval = new Interval(interval.b, interval.a);

		//		return stream.GetText(newInterval);
		//	}

		//	[return: NotNull]
		//	public string GetText()
		//	{
		//		return stream.GetText();
		//	}

		//	[return: NotNull]
		//	public string GetText(RuleContext ctx)
		//	{
		//		return stream.GetText(ctx);
		//	}

		//	[return: NotNull]
		//	public string GetText(IToken start, IToken stop)
		//	{
		//		return stream.GetText(stop, start);
		//	}

		//	public int LA(int i)
		//	{
		//		return stream.LA(-i);
		//	}

		//	[return: NotNull]
		//	public IToken LT(int k)
		//	{
		//		return stream.LT(-k);
		//	}

		//	public int Mark()
		//	{
		//		stream.m
		//	}

		//	public void Release(int marker)
		//	{
		//		throw new NotImplementedException();
		//	}

		//	public void Seek(int index)
		//	{
		//		throw new NotImplementedException();
		//	}
		//}

		//class ReversedTokenSource : ITokenSource {
		//	ITokenSource source;
		//	IList<IToken> reversedSource;

		//	public ReversedTokenSource(ITokenSource source)
		//	{
		//		this.source = source;

		//		//while(InputStream.
		//	}

		//	public int Line => throw new NotImplementedException();

		//	public int Column => throw new NotImplementedException();

		//	//public ICharStream InputStream => source.InputStream.
		//	public string SourceName => source.SourceName;

		//	public ITokenFactory TokenFactory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		//	[return: NotNull]
		//	public IToken NextToken()
		//	{
		//		throw new NotImplementedException();
		//	}
		//}


	}
}

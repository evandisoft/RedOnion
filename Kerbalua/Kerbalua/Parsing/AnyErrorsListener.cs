using System.IO;
using Antlr4.Runtime;

namespace Kerbalua.Parsing
{
	/// <summary>
	/// This just listens for any errors.
	/// Perhaps there is a simpler way to be strict.
	/// But I found this method quickly
	/// </summary>
	public class AnyErrorsListener : IAntlrErrorListener<IToken>
	{
		public bool HasError = false;

		public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			//Console.WriteLine("Found Errors");
			HasError = true;
		}
	}
}

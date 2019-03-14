using System;
using RedOnion.Script.Parsing;

namespace RedOnion.Script.Completion
{
	public class CompletionParser : Parser
	{
		/// <summary>
		/// Point of interest (character offset into source)
		/// </summary>
		public int Interest { get; set; }

		/// <summary>
		/// Start position of the token (found to have point of interest inside)
		/// </summary>
		public int TokenStart { get; protected set; }
		/// <summary>
		/// End position of the token (found to have point of interest inside)
		/// </summary>
		public int TokenEnd { get; protected set; }

		public CompletionParser(Option options) : base(options) { }
	}
}

using Kerbalui.Controls;

namespace LiveRepl.UI.ReplParts
{
	public class ReplOutoutArea:TextArea
	{
		public Repl repl;

		public ReplOutoutArea(Repl repl)
		{
			this.repl=repl;
		}
	}
}
using Kerbalui.Layout;
using UnityEngine;

namespace LiveRepl.UI.ReplParts {
	public class Repl:VerticalSpacer {

		public ReplGroup replGroup;

		public ReplOutoutArea replOutoutArea;
		public ReplInputArea replInputArea;

		public Repl(ReplGroup replGroup)
		{
			this.replGroup=replGroup;

			AddWeighted(1, replOutoutArea=new ReplOutoutArea(this));
			AddMinSized(replInputArea=new ReplInputArea(this));
		}
	}
}

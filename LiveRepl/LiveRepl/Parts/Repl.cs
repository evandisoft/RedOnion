using Kerbalui.Layout;
using UnityEngine;

namespace LiveRepl.Parts {
	public class Repl:VerticalSpacer {

		public ScriptWindowParts uiparts;

		public Repl(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddWeighted(1, uiparts.replOutoutArea=new ReplOutoutArea(uiparts));
			AddMinSized(uiparts.replInputArea=new ReplInputArea(uiparts));
		}
	}
}

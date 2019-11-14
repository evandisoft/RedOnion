using Kerbalui.Layout;
using LiveRepl.Decorators;
using UnityEngine;

namespace LiveRepl.Parts {
	public class Repl:VerticalSpacer {

		public ScriptWindowParts uiparts;

		public Repl(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddWeighted(1, uiparts.replOutoutArea=new ReplOutoutArea(uiparts));
			AddMinSized(new ScriptDisabledElement(uiparts,uiparts.replInputArea=new ReplInputArea(uiparts)));
		}
	}
}

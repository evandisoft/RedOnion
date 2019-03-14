using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedOnion.Script.BasicObjects;

namespace RedOnion.Script.Completion
{
	public class CompletionRoot : Root
	{
		public CompletionRoot(IEngine engine)
			: base(engine, true) { }
	}
}

using Kerbalui.Controls;
using Kerbalui.Decorators;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	public class ReplInputArea:EditingArea
	{
		public Repl repl;

		public ReplInputArea(Repl repl):base(new TextArea())
		{
			this.repl=repl;
		}

		protected override void DecoratorUpdate()
		{
			base.DecoratorUpdate();
			if (hadKeyDownThisUpdate)
			{
				repl.needsResize=true;
			}
			//repl.needsResize=true;
		}

		public override Vector2 MinSize
		{
			get
			{
				float minHeight=editableTextControl.MinSize.y;
				if (HorizontalScrollBarPresent)
				{
					minHeight+=ScrollbarWidth;
				}
				return new Vector2(0, minHeight);
			}
		}
	}
}
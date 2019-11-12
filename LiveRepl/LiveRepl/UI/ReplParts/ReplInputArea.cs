using Kerbalui.Controls;
using Kerbalui.Decorators;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	public class ReplInputArea:OldEditingArea
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
		}

		public override Vector2 MinSize
		{
			get
			{
				float minHeight=editableText.MinSize.y;
				if (HorizontalScrollBarPresent)
				{
					minHeight+=ScrollbarWidth;
				}
				//Debug.Log("minHeight "+minHeight);
				return new Vector2(0, minHeight);
			}
		}
	}
}
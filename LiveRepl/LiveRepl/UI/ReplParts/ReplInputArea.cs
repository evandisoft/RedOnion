using Kerbalui.Controls;
using Kerbalui.Decorators;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	public class ReplInputArea:EditingAreaScroller
	{
		public Repl repl;

		public ReplInputArea(Repl repl):base(new EditingArea(new TextArea()))
		{
			this.repl=repl;
		}

		protected override void DecoratorUpdate()
		{
			base.DecoratorUpdate();
			if (editingArea.hadKeyDownThisUpdate)
			{
				repl.needsResize=true;
			}
		}

		public override Vector2 MinSize
		{
			get
			{
				float minHeight=editingArea.MinSize.y;
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
using Kerbalui.Controls;
using Kerbalui.Decorators;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	public class ReplInputArea:EditingAreaScroller
	{
		public Repl repl;
		const float extraBottomSpace=5;

		public ReplInputArea(Repl repl):base(new EditingArea(new TextArea()))
		{
			this.repl=repl;
		}

		protected override void DecoratorUpdate()
		{
			bool receivedInput=false;
			if (HasFocus())
			{
				receivedInput=Event.current.type==EventType.KeyDown;
			}
			base.DecoratorUpdate();

			if (editingArea.ReceivedInput || receivedInput)
			{
				editingArea.ReceivedInput=true;
				repl.needsResize=true;
			}
		}

		bool firstUse=false;
		public override Vector2 MinSize
		{
			get
			{
				float minHeight=editingArea.MinSize.y+extraBottomSpace;
				if (HorizontalScrollBarPresent)
				{
					minHeight+=ScrollbarWidth;
				}
				//Debug.Log("minHeight "+minHeight);
				if (!firstUse)
				{
					repl.needsResize=true;
					firstUse=true;
				}
				return new Vector2(0, minHeight);
			}
		}
	}
}
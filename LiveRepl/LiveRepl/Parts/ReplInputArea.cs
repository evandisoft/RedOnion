using Kerbalui.Controls;
using Kerbalui.Decorators;
using UnityEngine;

namespace LiveRepl.Parts
{
	public class ReplInputArea:EditingAreaScroller
	{
		public ScriptWindowParts uiparts;
		const float extraBottomSpace=5;

		public ReplInputArea(ScriptWindowParts uiparts):base(new EditingArea(new TextArea()))
		{
			this.uiparts=uiparts;
		}

		protected override void DecoratorUpdate()
		{
			//if (editingArea.TrySetFont(uiparts.fontSelector.CurrentFont))
			//{
			//	uiparts.scriptWindow.needsResize=true;
			//}

			bool receivedInput=false;
			if (HasFocus())
			{
				receivedInput=Event.current.type==EventType.KeyDown;
			}
			base.DecoratorUpdate();

			if (editingArea.ReceivedInput || receivedInput)
			{
				editingArea.ReceivedInput=true;
				uiparts.scriptWindow.needsResize=true;
			}
		}

		public override bool HorizontalScrollBarPresent { get => rect.width<editingArea.MinSize.x; }
		public override bool VerticalScrollBarPresent { get => false; }


		bool firstUse=false;
		public override Vector2 MinSize
		{
			get
			{
				float minHeight=editingArea.MinSize.y; //+extraBottomSpace;
				if (HorizontalScrollBarPresent)
				{
					minHeight+=ScrollbarWidth;
				}
				//Debug.Log("minHeight "+minHeight);
				if (!firstUse)
				{
					uiparts.scriptWindow.needsResize=true;
					firstUse=true;
				}
				return new Vector2(0, minHeight);
			}
		}
	}
}
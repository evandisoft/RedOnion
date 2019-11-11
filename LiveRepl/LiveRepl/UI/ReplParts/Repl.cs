using Kerbalui.Groups;
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

		//public InputBox inputBox = new InputBox();
		//public OutputBox outputBox = new OutputBox();
		//KeyBindings specialKeyBindings = new KeyBindings();

		//public void Update(Rect rect, bool visible = true, GUIStyle style = null)
		//{
		//	GUIStyle inputStyle = new GUIStyle(GUI.skin.textArea) {
		//		//font = GUIUtil.GetMonoSpaceFont()
		//	};

		//	GUI.BeginGroup(rect);
		//	{
		//		float inputHeight = inputStyle.CalcHeight(inputBox.content, rect.width);

		//		float inputStart = rect.height - inputHeight;

		//		Rect outputRect = new Rect(0, 0, rect.width, inputStart);
		//		Rect inputRect = new Rect(0, inputStart, rect.width, inputHeight);

		//		// Allow copying event to get through to output box. Any other keydown gives
		//		// input box focus.
		//		if (outputBox.HasFocus() && Event.current.type == EventType.KeyDown) {
		//			switch (Event.current.keyCode) {
		//			case KeyCode.Insert:
		//			case KeyCode.LeftControl:
		//			case KeyCode.RightControl:
		//			case KeyCode.C:
		//				if (!Event.current.control) {
		//					// Don't give focus for the followup char events.
		//					if(Event.current.keyCode!=KeyCode.None)
		//						inputBox.GrabFocus();
		//				}
		//				break;
					
		//			default:
		//				// Don't give focus for the followup char events.
		//				if (Event.current.keyCode != KeyCode.None)
		//					inputBox.GrabFocus();
		//				break;
		//			}
		//		}
		//		outputBox.Update(outputRect, visible);

		//		inputBox.Update(inputRect, visible, inputStyle);

		//	}
		//	GUI.EndGroup();
		//}
	}
}

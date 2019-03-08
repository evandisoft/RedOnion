using UnityEngine;

namespace Kerbalua.Gui {
	public class Repl {
		public InputBox inputBox = new InputBox();
		public OutputBox outputBox = new OutputBox();

		public void Render(Rect rect)
		{
			GUIStyle inputStyle = new GUIStyle(GUI.skin.textArea) {
				//font = GUIUtil.GetMonoSpaceFont()
			};

			GUI.BeginGroup(rect);
			{
				float inputHeight = inputStyle.CalcHeight(inputBox.content, rect.width);
			 
				float inputStart = rect.height - inputHeight;

				Rect outputRect = new Rect(0, 0, rect.width, inputStart);
				Rect inputRect = new Rect(0, inputStart, rect.width, inputHeight);

				inputBox.Render(inputRect,inputStyle);
				outputBox.Render(outputRect);
			}
			GUI.EndGroup();
		}
	}
}

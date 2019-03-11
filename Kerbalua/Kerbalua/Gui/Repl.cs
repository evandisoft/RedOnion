using UnityEngine;

namespace Kerbalua.Gui {
	public class Repl:IRectRenderer {
		public InputBox inputBox = new InputBox();
		public OutputBox outputBox = new OutputBox();

		public void Update(Rect rect, bool visible = true, GUIStyle style = null)
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

				inputBox.Update(inputRect, true, inputStyle);
				outputBox.Update(outputRect);
			}
			GUI.EndGroup();
		}
	}
}

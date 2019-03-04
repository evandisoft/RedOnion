using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public interface IRectRender {
		void Render(Rect rect);
		Rect GetContentRect();
	}
}

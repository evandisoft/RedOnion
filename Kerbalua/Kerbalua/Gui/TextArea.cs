using System;
using UnityEngine;

namespace Kerbalua.Gui {
    public abstract class TextArea {
        public abstract void Render(Rect rect);

        public GUIContent content = new GUIContent("");
    }
}

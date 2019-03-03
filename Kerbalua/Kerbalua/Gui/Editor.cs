using UnityEngine;

namespace Kerbalua.Gui {
    public class Editor {
        public TextArea editingArea = new TextArea();

        public void Render(Rect rect)
        {
            editingArea.Render(rect);
        }
    }
}

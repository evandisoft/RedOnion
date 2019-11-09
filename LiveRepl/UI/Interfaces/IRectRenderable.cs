using UnityEngine;

namespace LiveRepl.UI.Elements
{
    public interface IRectRenderable:IUpdateable
    {
		void SetRect(Rect rect);
	}
}
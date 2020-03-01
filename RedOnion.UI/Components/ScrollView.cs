using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class ScrollView : UUI.ScrollRect
	{
		public override float preferredWidth => UUI.LayoutUtility.GetPreferredWidth(content);
		public override float preferredHeight => UUI.LayoutUtility.GetPreferredHeight(content);
	}
}

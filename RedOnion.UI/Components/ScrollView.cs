using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class ScrollView : UUI.ScrollRect
	{
		public override float preferredWidth
			=> UUI.LayoutUtility.GetPreferredWidth(content)
			+ (verticalScrollbar ? UUI.LayoutUtility.GetPreferredWidth(
				(RectTransform)verticalScrollbar.transform)
			- verticalScrollbarSpacing : 0f);
		public override float preferredHeight
			=> UUI.LayoutUtility.GetPreferredHeight(content)
			+ (horizontalScrollbar ? UUI.LayoutUtility.GetPreferredHeight(
				(RectTransform)horizontalScrollbar.transform)
			- horizontalScrollbarSpacing : 0f);

		//note: we should split this to SetLayoutHorizontal and SetLayoutVertical
		public override void SetLayoutHorizontal()
		{
			var hscroll = horizontalScrollbar;
			var vscroll = verticalScrollbar;
			if (!(bool)hscroll || !hscroll.gameObject.activeSelf)
				hscroll = null;
			if (!(bool)vscroll || !vscroll.gameObject.activeSelf)
				vscroll = null;
			var hrect = (RectTransform)hscroll?.transform;
			var vrect = (RectTransform)vscroll?.transform;
			var scroll = new Vector2(
				vrect == null ? 0f : UUI.LayoutUtility.GetPreferredWidth(vrect),
				hrect == null ? 0f : UUI.LayoutUtility.GetPreferredHeight(hrect));
			viewRect.sizeDelta = new Vector2(
				vrect == null ? 0f : -scroll.x - verticalScrollbarSpacing,
				hrect == null ? 0f : -scroll.y - horizontalScrollbarSpacing);
			if (hrect != null)
			{
				hrect.pivot = new Vector2(0f, 0f);
				hrect.anchorMin = new Vector2(0f, 0f);
				hrect.anchorMax = new Vector2(1f, 0f);
				hrect.anchoredPosition = new Vector2(0f, 0f);
				hrect.sizeDelta = new Vector2(vrect == null ? 0f : -scroll.x - verticalScrollbarSpacing, scroll.y);
			}
			if (vrect != null)
			{
				vrect.pivot = new Vector2(1f, 1f);
				vrect.anchorMin = new Vector2(1f, 0f);
				vrect.anchorMax = new Vector2(1f, 1f);
				vrect.anchoredPosition = new Vector2(1f, 0f);
				vrect.sizeDelta = new Vector2(scroll.x, hrect == null ? 0f : -scroll.y - horizontalScrollbarSpacing);
			}
			base.SetLayoutHorizontal();
		}
	}
}

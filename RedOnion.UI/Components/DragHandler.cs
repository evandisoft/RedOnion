using UnityEngine;
using UnityEngine.EventSystems;

namespace RedOnion.UI.Components
{
	[RequireComponent(typeof(RectTransform))]
	public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private Vector2 dragDelta = new Vector2(float.NaN, float.NaN);
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
			=> dragDelta = GetComponent<RectTransform>().anchoredPosition - eventData.position;
		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
			=> dragDelta = new Vector2(float.NaN, float.NaN);
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (!float.IsNaN(dragDelta.x))
				GetComponent<RectTransform>().anchoredPosition = dragDelta + eventData.position;
		}
	}
}

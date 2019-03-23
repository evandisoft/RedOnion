using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Fix the tooltips on KSC

namespace RedOnion.UI.Components
{
	[RequireComponent(typeof(RectTransform))]
	public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		Vector2 dragDelta = new Vector2(float.NaN, float.NaN);
		readonly string lockID = "RedOnion.DragHandler";
		bool locked = false;
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			dragDelta = GetComponent<RectTransform>().anchoredPosition - eventData.position;
			InputLockManager.SetControlLock(ControlTypes.UI_DRAGGING, lockID);
			locked = true;
			
		}
		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			InputLockManager.RemoveControlLock(lockID);
			locked = false;
			dragDelta = new Vector2(float.NaN, float.NaN);
		}
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (!float.IsNaN(dragDelta.x))
				GetComponent<RectTransform>().anchoredPosition = dragDelta + eventData.position;
		}
		void OnDestroy()
		{
			if (locked)
			{
				InputLockManager.RemoveControlLock(lockID);
				locked = false;
			}
		}
	}
}

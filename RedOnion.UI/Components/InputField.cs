using UnityEngine;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class InputField : UUI.InputField, UUI.ILayoutGroup
	{
		readonly string lockID = "RedOnion.InputField";
		bool locked = false;
		public override void OnSelect(BaseEventData eventData)
		{
			InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, lockID);
			locked = true;
			base.OnSelect(eventData);
		}
		public override void OnDeselect(BaseEventData eventData)
		{
			InputLockManager.RemoveControlLock(lockID);
			locked = false;
			base.OnDeselect(eventData);
		}
		protected override void OnDestroy()
		{
			if (locked)
			{
				InputLockManager.RemoveControlLock(lockID);
				locked = false;
			}
			base.OnDestroy();
		}

		public override float minWidth => 40f;
		public override float preferredWidth => 100f;
		public override float minHeight => base.preferredHeight;
		public override float preferredHeight => base.preferredHeight + 6f;

		void UUI.ILayoutController.SetLayoutHorizontal()
		{
			UpdateSizeOf(textComponent?.rectTransform);
			UpdateSizeOf(placeholder?.rectTransform);
		}
		void UUI.ILayoutController.SetLayoutVertical()
		{
			UpdateSizeOf(textComponent?.rectTransform);
			UpdateSizeOf(placeholder?.rectTransform);
		}
		static void UpdateSizeOf(RectTransform rt)
		{
			if (rt == null)
				return;
			rt.pivot = new Vector2(.5f, .5f);
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.anchoredPosition = Vector2.zero;
			rt.sizeDelta = new Vector2(-12f, -6f);
		}
	}
}

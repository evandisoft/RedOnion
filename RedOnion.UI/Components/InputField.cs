using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class InputField : UUI.InputField, UUI.ILayoutGroup
	{
		static readonly string lockID = "RedOnion.InputField";
		bool locked = false;
		public TextBox TextBox { get; set; }
		public class TextBoxEvent : UnityEvent<TextBox> { };
		public TextBoxEvent Selected { get; } = new TextBoxEvent();
		public TextBoxEvent Deselected { get; } = new TextBoxEvent();
		public class TextBoxEvent<T> : UnityEvent<TextBox, T> { };
		public TextBoxEvent<string> Changed { get; } = new TextBoxEvent<string>();
		public TextBoxEvent<string> Submitted { get; } = new TextBoxEvent<string>();

		public InputField()
		{
			onValueChanged.AddListener(text => Changed.Invoke(TextBox, text));
			onEndEdit.AddListener(text => Submitted.Invoke(TextBox, text));
		}

		public override void OnSelect(BaseEventData eventData)
		{
			// UI_DIALOGS disable NavBall toggle
			InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT|ControlTypes.UI_DIALOGS, lockID);
			locked = true;
			base.OnSelect(eventData);
			Selected.Invoke(TextBox);
		}
		public override void OnDeselect(BaseEventData eventData)
		{
			InputLockManager.RemoveControlLock(lockID);
			locked = false;
			base.OnDeselect(eventData);
			Deselected.Invoke(TextBox);
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

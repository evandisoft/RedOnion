using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class ToggleableButton : UUI.Button
	{
		bool _toggleable, _pressed;
		public bool Toggleable
		{
			get => _toggleable;
			set
			{
				if ((_toggleable = value) == false)
					_pressed = false;
			}
		}
		public bool Pressed
		{
			get => _pressed;
			set
			{
				if (value == _pressed)
					return;
				Toggle();
			}
		}
		protected bool disableTransition;
		public override bool IsInteractable()
			=> !disableTransition && base.IsInteractable();
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!Toggleable)
				base.OnPointerClick(eventData);
			else Toggle();
		}
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (Pressed) disableTransition = true;
			base.OnPointerEnter(eventData);
			disableTransition = false;
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (Pressed) disableTransition = true;
			base.OnPointerExit(eventData);
			disableTransition = false;
		}

		public void Toggle()
		{
			if (!Toggleable)
			{
				Press();
				return;
			}
			_pressed = !_pressed;
			if (!IsActive() || !IsInteractable())
				return;
			DoStateTransition(_pressed ? SelectionState.Pressed : currentSelectionState, false);
			onClick.Invoke();
		}
		public void Press()
		{
			if (!Toggleable)
			{
				base.OnSubmit(null);
				return;
			}
			if (!_pressed)
				Toggle();
		}
	}
}

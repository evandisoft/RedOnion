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
				if (_toggleable = value)
					return;
				_pressed = false;
				image.sprite = normalSprite;
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
		public Sprite normalSprite;
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!Toggleable)
				base.OnPointerClick(eventData);
			else Toggle();
		}

		public void Toggle()
		{
			if (!Toggleable)
			{
				Press();
				return;
			}
			_pressed = !_pressed;
			image.sprite = _pressed ? spriteState.pressedSprite : normalSprite;
			if (!IsActive() || !IsInteractable())
				return;
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

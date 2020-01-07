using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class ToggleableButton : UUI.Selectable, IPointerClickHandler, ISubmitHandler
	{

		public Button Button { get; set; }
		public event Action<Button> Click;

		public Sprite normalSprite;
		public Sprite normalHighlight;
		public Sprite pressedHighlight;
		bool _toggleable, _pressed;
		int _exclusive;

		public bool Toggleable
		{
			get => _toggleable;
			set
			{
				_toggleable = value;
				if (value) return;
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
		public bool Exclusive
		{
			get => _exclusive > 0;
			set
			{
				if (Exclusive == value)
					return;
				_exclusive = value ? 1 : 0;
				if (!value)
					return;
				_toggleable = true;
				if (_pressed)
					EnsureExclusive();
			}
		}
		public int ExclusiveLevel
		{
			get => _exclusive;
			set
			{
				if (_exclusive == value)
					return;
				if (value <= 0)
				{
					_exclusive = 0;
					return;
				}
				_exclusive = value;
				_toggleable = true;
				if (_pressed)
					EnsureExclusive();
			}
		}

		public void Press()
			=> Press(true, true);
		public void Press(bool action = true, bool transition = true)
		{
			if (Toggleable)
			{
				if (!_pressed)
					Toggle(action);
				else if (action)
					Click?.Invoke(Button);
				return;
			}
			
			if (action)
				Click?.Invoke(Button);
			if (transition && IsActive() && IsInteractable() && colors.fadeDuration > 0f)
			{
				DoStateTransition(SelectionState.Pressed, instant: false);
				StartCoroutine(Unpress());
			}
		}
		IEnumerator Unpress()
		{
			float elapsed = 0f;
			while (elapsed < colors.fadeDuration)
			{
				elapsed += Time.unscaledDeltaTime;
				yield return null;
			}
			DoStateTransition(currentSelectionState, instant: false);
		}
		public void Toggle()
			=> Toggle(true);
		public void Toggle(bool action)
		{
			if (!Toggleable)
			{
				Press(action);
				return;
			}
			if (normalSprite == null)
				normalSprite = image.sprite;
			if (normalHighlight == null && !Pressed)
				normalHighlight = spriteState.highlightedSprite;
			if (pressedHighlight == null)
				pressedHighlight = Pressed ? spriteState.highlightedSprite : normalSprite;
			_pressed = !_pressed;
			image.sprite = Pressed ? spriteState.pressedSprite : normalSprite;
			spriteState = new UUI.SpriteState
			{
				highlightedSprite = (Pressed ? pressedHighlight : normalHighlight)
				?? spriteState.highlightedSprite,
				pressedSprite = spriteState.pressedSprite,
				disabledSprite = spriteState.disabledSprite
			};
			if (Exclusive && Pressed)
				EnsureExclusive();
			if (action)
				Click?.Invoke(Button);
		}
		protected void EnsureExclusive()
		{
			if (!Exclusive || !Pressed)
				return;
			var parent = Button?.Parent as Panel;
			if (parent == null)
				return;
			if (_exclusive == 1)
				EnsureExclusive(parent, _exclusive);
			else
			{
				int levels = 1;
				do
				{
					var p2 = parent.Parent as Panel;
					if (p2 == null)
						break;
					parent = p2;
				} while (++levels < _exclusive);
				EnsureExclusive(parent, levels);
			}
		}
		void EnsureExclusive(Panel parent, int levels)
		{
			if (parent == null)
				return;
			if (levels <= 1)
			{
				foreach (var e in parent)
				{
					var other = e as Button;
					if (other == null || other == Button)
						continue;
					if (other.Exclusive && other.Pressed)
						other.Toggle(false);
				}
			}
			else
			{
				levels--;
				foreach (var e in parent)
					EnsureExclusive(e as Panel, levels);
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left
				&& IsActive() && IsInteractable())
			{
				if (!Toggleable)
					Press(transition: false);
				else if (!Exclusive || !Pressed)
					Toggle();
				else
					Click?.Invoke(Button);
			}
		}
		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (IsActive() && IsInteractable())
			{
				if (!Toggleable)
					Press();
				else if (!Exclusive || !Pressed)
					Toggle();
				else
					Click?.Invoke(Button);
			}
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			var esys = EventSystem.current;
			if (esys != null && esys.currentSelectedGameObject == gameObject)
				esys.SetSelectedGameObject(null);
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			var esys = EventSystem.current;
			if (esys != null && esys.currentSelectedGameObject == gameObject)
				esys.SetSelectedGameObject(null);
		}
	}
}

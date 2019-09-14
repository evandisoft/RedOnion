using RedOnion.UI.Components;
using System;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Button : Element
	{
		protected ToggleableButton Core { get; private set; }
		protected BackgroundImage Image { get; private set; }

		protected Label label;
		protected Label LabelCore
		{
			get
			{
				if (label == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					label = Add(new Label("Label")
					{
						Text = "Button",
						TextColor = Skin.button.normal.textColor,
						FontStyle = Skin.button.fontStyle
					});
				}
				return label;
			}
		}

		protected Image icon;
		protected Image IconCore
		{
			get
			{
				if (icon == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					icon = Add(new Image());
				}
				return icon;
			}
		}

		public Button()
		{
			Image = GameObject.AddComponent<BackgroundImage>();
			Image.type = UUI.Image.Type.Sliced;
			Image.sprite = Skin.button.normal.background;
			Core = GameObject.AddComponent<ToggleableButton>();
			Core.Button = this;
			Core.colors = UUI.ColorBlock.defaultColorBlock;
			Core.image = Image;
			Core.normalSprite = Skin.button.normal.background;
			Core.normalHighlight = Skin.button.highlight.background;
			Core.pressedHighlight = Skin.button.active.background;
			Core.spriteState = new UUI.SpriteState()
			{
				pressedSprite = Skin.button.active.background,
				highlightedSprite = Skin.button.highlight.background,
				disabledSprite = Skin.button.disabled.background
			};
			Core.transition = UUI.Selectable.Transition.SpriteSwap;
			InnerPadding = new Padding(8f, 4f);
			Spacing = 6f;
			Layout = Layout.Horizontal;
		}
		public Button(string text) : this() => Text = text;
		public Button(string text, UnityAction<Button> click) : this(text) => Core.Click.AddListener(click);

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			Core = null;
			Image = null;
			label = null;
			icon = null;
			base.Dispose(true);
		}

		public Event<Button> Click
			=> new Event<Button>(Core.Click);

		public bool Toggleable
		{
			get => Core.Toggleable;
			set => Core.Toggleable = value;
		}
		public bool Pressed
		{
			get => Core.Pressed;
			set => Core.Pressed = value;
		}
		public bool Exclusive
		{
			get => Core.Exclusive;
			set => Core.Exclusive = value;
		}
		public int ExclusiveLevel
		{
			get => Core.ExclusiveLevel;
			set => Core.ExclusiveLevel = value;
		}
		public void Press() => Core.Press();
		public void Toggle() => Core.Toggle();
		public void Press(bool action) => Core.Press(action);
		public void Toggle(bool action) => Core.Toggle(action);

		public string Text
		{
			get => label == null ? "" : label.Text;
			set
			{
				if (value == null || value.Length == 0)
				{
					if (label == null)
						return;
					label.Dispose();
					label = null;
					return;
				}
				LabelCore.Text = value;
			}
		}

		public Texture IconTexture
		{
			get => icon?.Texture;
			set
			{
				if (value == null)
				{
					if (icon == null)
						return;
					icon.Dispose();
					icon = null;
					return;
				}
				IconCore.Texture = value;
			}
		}
		public Sprite IconSprite
		{
			get => icon?.Sprite;
			set
			{
				if (value == null)
				{
					if (icon == null)
						return;
					icon.Dispose();
					icon = null;
					return;
				}
				IconCore.Sprite = value;
			}
		}

		public new LayoutPadding LayoutPadding
		{
			get => base.LayoutPadding;
			set => base.LayoutPadding = value;
		}
		public new Padding InnerPadding
		{
			get => base.InnerPadding;
			set => base.InnerPadding = value;
		}
		public new Vector2 InnerSpacing
		{
			get => base.InnerSpacing;
			set => base.InnerSpacing = value;
		}
		public new float Padding
		{
			get => base.Padding;
			set => base.Padding = value;
		}
		public new float Spacing
		{
			get => base.Spacing;
			set => base.Spacing = value;
		}
	}
}

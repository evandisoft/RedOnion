using RedOnion.UI.Components;
using System;
using UnityEngine;
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
			Core = GameObject.AddComponent<ToggleableButton>();
			Image = GameObject.AddComponent<BackgroundImage>();
			Image.sprite = Skin.button.normal.background;
			Image.type = UUI.Image.Type.Sliced;
			Core.image = Image;
			Core.normalSprite = Skin.button.normal.background;
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

		public Event Click
		{
			get => new Event(Core.onClick);
			set { }
		}
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
		public void Press() => Core.Press();
		public void Toggle() => Core.Toggle();

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

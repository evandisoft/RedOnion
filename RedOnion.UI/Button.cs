using RedOnion.Attributes;
using RedOnion.UI.Components;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[Description("Clickable button (or toggle-button).")]
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

		[Description("Create new button without a text.")]
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
		[Description("Create new button with specified text.")]
		public Button(string text) : this() => Text = text;
		[Description("Create new button with specified text and click-action.")]
		public Button(string text, Action<Button> click) : this(text) => Core.Click += click;

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

		[Description("Executed when clicked.")]
		public event Action<Button> Click
		{
			add => Core.Click += value;
			remove => Core.Click -= value;
		}

		[Description("Enabled/Disabled (for clicks, Unity: `interactable`).")]
		public bool Enabled
		{
			get => Core.interactable;
			set => Core.interactable = value;
		}

		[Description("Toggleable button (false by default).")]
		public bool Toggleable
		{
			get => Core.Toggleable;
			set => Core.Toggleable = value;
		}
		[Description("Button is pressed (mainly for toggleable).")]
		public bool Pressed
		{
			get => Core.Pressed;
			set => Core.Pressed = value;
		}
		[Description("Toggleable button is exclusive (other toggleable buttons get released when this one is pressed).")]
		public bool Exclusive
		{
			get => Core.Exclusive;
			set => Core.Exclusive = value;
		}
		[Description("Depth (parent-chain) of the exclusivity logic.")]
		public int ExclusiveLevel
		{
			get => Core.ExclusiveLevel;
			set => Core.ExclusiveLevel = value;
		}
		[Description("Press the button.")]
		public void Press() => Core.Press();
		[Description("Toggle the button.")]
		public void Toggle() => Core.Toggle();
		[Description("Press the button optionally not executing the `Click` action.")]
		public void Press(bool action) => Core.Press(action);
		[Description("Toggle the button optionally not executing the `Click` action.")]
		public void Toggle(bool action) => Core.Toggle(action);

		[Description("Text of the button.")]
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

		[Unsafe, Description("Icon on the button.")]
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
		[Unsafe, Description("Sprite of the icon on the button.")]
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

		[Description("The combined inner padding and spacing (6 floats in total, set to `8f, 6f, 8f, 4f, 6f, 4f` by default).")]
		public new LayoutPadding LayoutPadding
		{
			get => base.LayoutPadding;
			set => base.LayoutPadding = value;
		}
		[Description("Inner padding (border left empty inside this button).")]
		public new Padding InnerPadding
		{
			get => base.InnerPadding;
			set => base.InnerPadding = value;
		}
		[Description("Inner spacing (between text and icon).")]
		public new Vector2 InnerSpacing
		{
			get => base.InnerSpacing;
			set => base.InnerSpacing = value;
		}
		[Description("`InnerPadding.All` - one number if all are the same, or NaN.")]
		public new float Padding
		{
			get => base.Padding;
			set => base.Padding = value;
		}
		[Description("Spacing - one number if both are the same, or NaN.")]
		public new float Spacing
		{
			get => base.Spacing;
			set => base.Spacing = value;
		}
	}
}

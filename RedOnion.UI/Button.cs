using RedOnion.UI.Components;
using System;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Button : Element
	{
		protected UUI.Button Core { get; private set; }
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
					label = Add(new Label("Label") { Text = "Button" });
				}
				return label;
			}
		}

		protected Icon icon;
		protected Icon IconCore
		{
			get
			{
				if (icon == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					icon = Add(new Icon("Icon"));
				}
				return icon;
			}
		}

		public Button(string name = null)
			: base(name)
		{
			Core = GameObject.AddComponent<UUI.Button>();
			Image = GameObject.AddComponent<BackgroundImage>();
			Image.sprite = Skin.button.normal.background;
			Core.image = Image;
			Core.spriteState = new UUI.SpriteState()
			{
				pressedSprite = Skin.button.active.background,
				highlightedSprite = Skin.button.highlight.background,
				disabledSprite = Skin.button.disabled.background
			};
			InnerPadding = new Padding(8f, 4f);
			Spacing = 6f;
			Layout = Layout.Horizontal;
		}

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

		public event UnityAction Click
		{
			add => Core.onClick.AddListener(value);
			remove => Core.onClick.RemoveListener(value);
		}

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
				}
				IconCore.Texture = value;
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

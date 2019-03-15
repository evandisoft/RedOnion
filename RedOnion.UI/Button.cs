using System;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Button : Element
	{
		protected UISkinDef skin;
		protected UIStyle style;
		protected UUI.Image Image { get; private set; }
		protected UUI.Button Core { get; private set; }

		private Label label;
		protected Label LabelCore
		{
			get
			{
				if (label == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name ?? GetType().Name);
					Add(label = new Label(skin, style, "Label"));
				}
				return label;
			}
		}

		private Icon icon;
		protected Icon IconCore
		{
			get
			{
				if (icon == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name ?? GetType().Name);
					icon = new Icon("Icon");
					icon.Anchors = label == null ? Anchors.Center : Anchors.Left;
					Add(icon);
				}
				return icon;
			}
		}

		public Button(string name = null)
			: this(DefaultSkin, DefaultSkin.button, name) { }
		public Button(UISkinDef skin, UIStyle style, string name = null)
			: base(name)
		{
			this.skin = skin;
			this.style = style;
			Image = GameObject.AddComponent<UUI.Image>();
			Image.sprite = style.normal.background;
			Core = GameObject.AddComponent<UUI.Button>();
		}

		public event UnityAction Click
		{
			add => Core.onClick.AddListener(value);
			remove => Core.onClick.RemoveListener(value);
		}

		public bool HasLabel => label != null;
		public bool HasIcon => icon != null;

		public string Text
		{
			get => label == null ? "" : label.Text;
			set => LabelCore.Text = value;
		}

		public Texture IconTexture
		{
			get => icon?.Texture;
			set => IconCore.Texture = value;
		}
	}
}

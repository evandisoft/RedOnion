using System;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Button : Element
	{
		public UUI.Image Image { get; private set; }
		public UUI.Button Core { get; private set; }

		public Label label;
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

		public Icon icon;
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
			Image = GameObject.AddComponent<UUI.Image>();
			Image.sprite = Skin.button.normal.background;
			Core = GameObject.AddComponent<UUI.Button>();
			Layout = Layout.Horizontal;
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
	}
}

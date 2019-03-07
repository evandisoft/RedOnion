using System;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Button : Element
	{
		protected UUI.Image Image { get; private set; }
		protected UUI.Button Core { get; private set; }

		private Element icon;
		private Label label;
		protected bool HasIcon => icon != null;
		protected bool HasLabel => label != null;

		protected Element Icon
		{
			get
			{
				if (icon == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(GetType().Name);
					icon = new Element(this, "Icon");
					if (label == null)
						icon.Anchors = new Rect(.5f, .5f, .5f, .5f);
					else
						icon.Anchors = new Rect(0, .5f, 0, .5f);
					icon.SizeDelta = new Vector2(13, 13);
				}
				return icon;
			}
		}

		protected Label LabelCore
		{
			get
			{
				if (label == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(GetType().Name);
					label = new Label(this, "Label");
				}
				return label;
			}
		}

		public Button(Element parent = null, string name = null)
			: this(UISkinManager.defaultSkin.button, parent, name) { }
		public Button(UIStyle style, Element parent = null, string name = null)
			: base(parent, name)
		{
			Image = GameObject.AddComponent<UUI.Image>();
			Image.sprite = style.normal.background;
			Core = GameObject.AddComponent<UUI.Button>();
		}

		public event UnityAction Click
		{
			add => Core.onClick.AddListener(value);
			remove => Core.onClick.RemoveListener(value);
		}
	}
}

using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Label : Element
	{
		public UUI.Text Core { get; private set; }

		public Label(string name = null)
			: base(name)
		{
			Core = GameObject.AddComponent<UUI.Text>();
			Core.alignment = TextAnchor.MiddleLeft;
			Core.font = Skin.font;
			Core.fontSize = 14;
			Core.fontStyle = Skin.label.fontStyle;
			Core.color = Skin.label.normal.textColor;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			Core = null;
			base.Dispose(true);
		}

		public string Text
		{
			get => Core.text ?? "";
			set => Core.text = value ?? "";
		}
		public Color TextColor
		{
			get => Core.color;
			set => Core.color = value;
		}
		public TextAnchor TextAlign
		{
			get => Core.alignment;
			set => Core.alignment = value;
		}
		public int FontSize
		{
			get => Core.fontSize;
			set => Core.fontSize = value;
		}
		public FontStyle FontStyle
		{
			get => Core.fontStyle;
			set => Core.fontStyle = value;
		}
	}
}

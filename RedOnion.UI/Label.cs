using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Label : Element
	{
		protected UUI.Text Core { get; private set; }

		public Label(string name = null)
			: this(DefaultSkin, DefaultSkin.label, name) { }
		public Label(UISkinDef skin, UIStyle style, string name = null)
			: base(name)
		{
			Core = GameObject.AddComponent<UUI.Text>();
			Core.alignment = TextAnchor.MiddleCenter;
			Core.font = style.font ?? skin.font;
			Core.fontSize = style.fontSize;
			Core.fontStyle = style.fontStyle;
			Core.color = style.normal.textColor;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			base.Dispose(true);
			Core = null;
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
	}
}

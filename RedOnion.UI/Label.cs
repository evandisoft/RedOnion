using System;
using System.ComponentModel;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[Description("`UI.Label` is simple line of text used to label other elements.")]
	public class Label : Element
	{
		protected internal UUI.Text Core { get; private set; }

		[Description("Create label element with no text.")]
		public Label()
		{
			Core = GameObject.AddComponent<UUI.Text>();
			Core.alignment = TextAnchor.MiddleLeft;
			Core.font = Skin.font;
			Core.fontSize = 14;
			Core.fontStyle = Skin.label.fontStyle;
			Core.color = Skin.label.normal.textColor;
		}
		[Description("Create label element with specified text.")]
		public Label(string text) : this() => Text = text;

		protected override void Dispose(bool disposing)
		{
			if (!disposing || RootObject == null)
				return;
			Core = null;
			base.Dispose(true);
		}

		[Description("The text on the label.")]
		public string Text
		{
			get => Core.text ?? "";
			set => Core.text = value ?? "";
		}
		[Description("Foreground color of the text.")]
		public Color TextColor
		{
			get => Core.color;
			set => Core.color = value;
		}
		[Description("How to align text within the label.")]
		public TextAnchor TextAlign
		{
			get => Core.alignment;
			set => Core.alignment = value;
		}
		[Description("Size of the font used for the text.")]
		public int FontSize
		{
			get => Core.fontSize;
			set => Core.fontSize = value;
		}
		[Description("Style of the font used for the text.")]
		public FontStyle FontStyle
		{
			get => Core.fontStyle;
			set => Core.fontStyle = value;
		}
	}
}

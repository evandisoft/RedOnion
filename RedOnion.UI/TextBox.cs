using RedOnion.UI.Components;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class TextBox : Element
	{
		public InputField Core { get; private set; }
		public Label Label { get; private set; }
		protected BackgroundImage Image { get; private set; }

		public TextBox(string name = null)
			: base(name)
		{
			Image = GameObject.AddComponent<BackgroundImage>();
			Image.sprite = Skin.textField.normal.background;
			Image.type = UUI.Image.Type.Sliced;
			Core = GameObject.AddComponent<InputField>();
			Label = Add(new Label()
			{
				Anchors = Anchors.Fill,
				TextColor = Skin.textField.normal.textColor,
				TextAlign = TextAnchor.UpperLeft
			});
			Core.textComponent = Label.Core;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			Core = null;
			Label.Dispose();
			Label = null;
			Image = null;
			base.Dispose(true);
		}

		public string Text
		{
			get => Core.text ?? "";
			set => Core.text = value ?? "";
		}
		public Color TextColor
		{
			get => Label.TextColor;
			set => Label.TextColor = value;
		}
		public TextAnchor TextAlign
		{
			get => Label.TextAlign;
			set => Label.TextAlign = value;
		}

		public bool MultiLine
		{
			get => Core.multiLine;
			set => Core.lineType = value
				? UUI.InputField.LineType.MultiLineNewline
				: UUI.InputField.LineType.SingleLine;
		}
	}
}

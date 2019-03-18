using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Label : Element
	{
		protected UUI.Text Core { get; private set; }

		public Label(string name = null)
			: base(name)
		{
			Core = GameObject.AddComponent<UUI.Text>();
			Core.alignment = TextAnchor.MiddleLeft;
			Core.font = Skin.font;
			Core.fontSize = 14;
			Core.fontStyle = FontStyle.Normal;
			Core.color = Color.black;
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

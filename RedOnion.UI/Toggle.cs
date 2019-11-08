using RedOnion.UI.Components;
using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Toggle : Element
	{
		public UUI.Toggle Core { get; private set; }
		public Simple ToggleFrame { get; private set; }
		public Image ImageCore { get; private set; }

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
						Text = "Toggle",
						TextColor = Skin.button.normal.textColor,
						FontStyle = Skin.button.fontStyle
					});
				}
				return label;
			}
		}

		public Toggle()
		{
			Core = GameObject.AddComponent<UUI.Toggle>();
			// TODO: width by height (aspect-ratio fitter)
			ToggleFrame = Add(new Simple {
				Sprite = Skin.toggle.normal.background
			});
			ImageCore = ToggleFrame.Add(new Image {
				Sprite = Skin.toggle.active.background
			});
			InnerPadding = new Padding(8f, 4f);
			Spacing = 6f;
			Layout = Layout.Horizontal;
			Core.spriteState = new UUI.SpriteState
			{
				disabledSprite = Skin.toggle.disabled.background,
				pressedSprite = Skin.toggle.active.background,
				highlightedSprite = Skin.toggle.highlight.background
			};
			Core.graphic = ImageCore.Core;
			//Core.targetGraphic = ImageCore.Core;
		}
		public Toggle(string text) : this() => Text = text;

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			ToggleFrame.Dispose();
			ImageCore.Dispose();
			LabelCore.Dispose();
			base.Dispose(true);
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
	}
}

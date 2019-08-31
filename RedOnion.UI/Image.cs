using RedOnion.UI.Components;
using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Image : Element
	{
		protected UUI.RawImage raw;
		public UUI.RawImage Raw
		{
			get
			{
				if (raw == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					Core = null;
					raw = GameObject.AddComponent<UUI.RawImage>();
				}
				return raw;
			}
			protected set
			{
				if (raw == value)
					return;
				if (raw != null)
					GameObject.Destroy(raw);
				raw = value;
			}
		}

		protected UUI.Image image;
		public UUI.Image Core
		{
			get
			{
				if (image == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					Raw = null;
					image = GameObject.AddComponent<UUI.Image>();
				}
				return image;
			}
			protected set
			{
				if (image == value)
					return;
				if (image != null)
					GameObject.Destroy(image);
				image = value;
			}
		}

		public Image(string name = null)
			: base(name)
		{
		}

		public bool AutoSize { get; set; } = true;
		public bool HasRaw => raw != null;
		public bool HasImage => image != null;
		public Texture Texture
		{
			get => raw?.texture ?? image?.sprite?.texture;
			set
			{
				Raw.texture = value;
				if (!AutoSize) return;
				Raw.SetNativeSize();
				MinWidth = value.width;
				MinHeight = value.height;
			}
		}
		public Color Color
		{
			get => raw?.color ?? new Color();
			set => Raw.color = value;
		}

		public Sprite Sprite
		{
			get => image?.sprite;
			set
			{
				Core.sprite = value;
				Core.type = UUI.Image.Type.Sliced;
			}
		}
		public UUI.Image.Type ImageType
		{
			get => Core?.type ?? UUI.Image.Type.Sliced;
			set => Core.type = value;
		}
	}
}

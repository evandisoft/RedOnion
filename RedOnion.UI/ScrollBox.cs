using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class ScrollBox : Element
	{
		protected UUI.RawImage image;
		protected UUI.RawImage Image
		{
			get
			{
				if (image == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					image = GameObject.AddComponent<UUI.RawImage>();
				}
				return image;
			}
			set
			{
				if (image == value)
					return;
				if (image != null)
					GameObject.Destroy(image);
				image = value;
			}
		}

		public ScrollBox(string name = null)
			: base(name)
		{
			Layout = Layout.Horizontal;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			image = null;
			base.Dispose(disposing);
		}

		public Color Color
		{
			get => image?.color ?? new Color();
			set => Image.color = value;
		}
		public Texture Texture
		{
			get => image?.texture;
			set => Image.texture = value;
		}
	}
}

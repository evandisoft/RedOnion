using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Panel : Element
	{
		private UUI.RawImage image;
		protected bool HasImage => image != null;
		protected UUI.RawImage Image
		{
			get
			{
				if (image == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(GetType().Name);
					image = GameObject.AddComponent<UUI.RawImage>();
				}
				return image;
			}
		}

		public Panel(Element parent = null, string name = null)
			: base(parent, name)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			base.Dispose(disposing);
			image = null;
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

using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public partial class Panel : Element
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

		public Panel(string name = null)
			: base(name)
		{
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

		public new Layout Layout
		{
			get => base.Layout;
			set => base.Layout = value;
		}
		public new LayoutPadding LayoutPadding
		{
			get => base.LayoutPadding;
			set => base.LayoutPadding = value;
		}
		public new Anchors ChildAnchors
		{
			get => base.ChildAnchors;
			set => base.ChildAnchors = value;
		}
		public new Padding InnerPadding
		{
			get => base.InnerPadding;
			set => base.InnerPadding = value;
		}
		public new Vector2 InnerSpacing
		{
			get => base.InnerSpacing;
			set => base.InnerSpacing = value;
		}
		public new float Padding
		{
			get => base.Padding;
			set => base.Padding = value;
		}
		public new float Spacing
		{
			get => base.Spacing;
			set => base.Spacing = value;
		}
	}
}

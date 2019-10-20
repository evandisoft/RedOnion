using RedOnion.UI.Components;
using System;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Simple : Element
	{
		public Simple() : base() { }
		public Simple(Layout layout) : base() => Layout = layout;

		protected UUI.RawImage rawImage;
		public UUI.RawImage RawImage
		{
			get
			{
				if (rawImage == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					Image = null;
					rawImage = GameObject.AddComponent<UUI.RawImage>();
				}
				return rawImage;
			}
			set
			{
				if (rawImage == value)
					return;
				if (rawImage != null)
					GameObject.Destroy(rawImage);
				rawImage = value;
			}
		}

		protected BackgroundImage image;
		public BackgroundImage Image
		{
			get
			{
				if (image == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					RawImage = null;
					image = GameObject.AddComponent<BackgroundImage>();
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

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			RawImage = null;
			Image = null;
			base.Dispose(disposing);
		}

		public new E Add<E>(E element) where E : Element
			=> base.Add(element);
		public new E Remove<E>(E element) where E : Element
			=> base.Remove(element);
		public new Element Add(Element element)
			=> base.Add(element);
		public new Element Remove(Element element)
			=> base.Remove(element);
		public new void Add(params Element[] elements)
			=> base.Add(elements);
		public new void Remove(params Element[] elements)
			=> base.Remove(elements);

		public Panel AddPanel(Layout layout)
			=> Add(new Panel(layout));
		public Panel AddHorizontal()
			=> Add(new Panel(Layout.Horizontal));
		public Panel AddVertical()
			=> Add(new Panel(Layout.Vertical));
		public Label AddLabel(string text)
			=> Add(new Label(text));
		public TextBox AddText(string text)
			=> Add(new TextBox(text));
		public TextBox AddTextBox(string text)
			=> Add(new TextBox(text));
		public Button AddButton(string text)
			=> Add(new Button(text));
		public Button AddButton(string text, Action<Button> click)
			=> Add(new Button(text, click));
		public Button AddToggle(string text)
			=> Add(new Button(text) { Toggleable = true });
		public Button AddToggle(string text, Action<Button> click)
			=> Add(new Button(text, click) { Toggleable = true });
		public Button AddExclusive(string text)
			=> Add(new Button(text) { Exclusive = true });
		public Button AddExclusive(string text, Action<Button> click)
			=> Add(new Button(text, click) { Exclusive = true });
		public Button AddExclusive2(string text)
			=> Add(new Button(text) { ExclusiveLevel = 2 });
		public Button AddExclusive2(string text, Action<Button> click)
			=> Add(new Button(text, click) { ExclusiveLevel = 2 });

		public Color Color
		{
			get => rawImage?.color ?? new Color();
			set => RawImage.color = value;
		}
		public Texture Texture
		{
			get => rawImage?.texture;
			set => RawImage.texture = value;
		}
		public Sprite Sprite
		{
			get => image?.sprite;
			set
			{
				Image.sprite = value;
				Image.type = UUI.Image.Type.Sliced;
			}
		}
		public UUI.Image.Type ImageType
		{
			get => image?.type ?? UUI.Image.Type.Sliced;
			set => Image.type = value;
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

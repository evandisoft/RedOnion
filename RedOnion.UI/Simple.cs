using RedOnion.UI.Components;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[Description(
@"`UI.Simple` is the base class for [`UI.Panel`](Panel.md) without the child tracking and enumerating functionality.
It is meant mainly for internal usage, for implementation of other composite controls, not for direct user/script usage.")]
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

		[Description("Add new element onto this panel.")]
		public new Element Add(Element element)
			=> base.Add(element);
		[Description("Remove element from this panel.")]
		public new Element Remove(Element element)
			=> base.Remove(element);

		public new void Add(params Element[] elements)
			=> base.Add(elements);
		public new void Remove(params Element[] elements)
			=> base.Remove(elements);

		[Description("Add new panel with specified layout.")]
		public Panel AddPanel(Layout layout)
			=> Add(new Panel(layout));
		[Description("Add new panel with horizontal layout.")]
		public Panel AddHorizontal()
			=> Add(new Panel(Layout.Horizontal));
		[Description("Add new panel with vertical layout.")]
		public Panel AddVertical()
			=> Add(new Panel(Layout.Vertical));

		[Description("Add new label with specified text.")]
		public Label AddLabel(string text)
			=> Add(new Label(text));

		public TextBox AddText(string text)
			=> Add(new TextBox(text));
		[Description("Add new label with specified text.")]
		public TextBox AddTextBox(string text)
			=> Add(new TextBox(text));

		[Description("Add new button with specified text.")]
		public Button AddButton(string text)
			=> Add(new Button(text));
		[Description("Add new button with specified text and click-action.")]
		public Button AddButton(string text, Action<Button> click)
			=> Add(new Button(text, click));

		[Description("Add new toggle-button with specified text.")]
		public Button AddToggle(string text)
			=> Add(new Button(text) { Toggleable = true });
		[Description("Add new toggle-button with specified text and click-action.")]
		public Button AddToggle(string text, Action<Button> click)
			=> Add(new Button(text, click) { Toggleable = true });

		[Description("Add new exclusive toggle-button (radio button) with specified text.")]
		public Button AddExclusive(string text)
			=> Add(new Button(text) { Exclusive = true });
		[Description("Add new exclusive toggle-button with specified text and click-action.")]
		public Button AddExclusive(string text, Action<Button> click)
			=> Add(new Button(text, click) { Exclusive = true });

		[Description("Add new toggle-button exclusive in parent of this panel with specified text.")]
		public Button AddExclusive2(string text)
			=> Add(new Button(text) { ExclusiveLevel = 2 });
		[Description("Add new toggle-button exclusive in parent of this panel with specified text and click-action.")]
		public Button AddExclusive2(string text, Action<Button> click)
			=> Add(new Button(text, click) { ExclusiveLevel = 2 });

		[Description("Background color.")]
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

		[Description("Layout (how child elements are placed).")]
		public new Layout Layout
		{
			get => base.Layout;
			set => base.Layout = value;
		}
		[Description("The combined inner padding and spacing (6 floats in total, set to `0f, 3f, 0f, 0f, 3f, 0f` by default).")]
		public new LayoutPadding LayoutPadding
		{
			get => base.LayoutPadding;
			set => base.LayoutPadding = value;
		}
		[Description(
@"This currently controls layout's `childAlignment` and
`childForceExpandWidth/Height`, but plan is to use custom `LayoutComponent`.
You can try `Anchors.Fill` (to make all inner elements fill their cell)
or `Anchors.MiddleLeft/MiddleCenter/UpperLeft...`")]
		public new Anchors ChildAnchors
		{
			get => base.ChildAnchors;
			set => base.ChildAnchors = value;
		}
		[Description("Inner padding (border left empty inside this panel).")]
		public new Padding InnerPadding
		{
			get => base.InnerPadding;
			set => base.InnerPadding = value;
		}
		[Description("Inner spacing (between elements).")]
		public new Vector2 InnerSpacing
		{
			get => base.InnerSpacing;
			set => base.InnerSpacing = value;
		}
		[Description("`InnerPadding.All` - one number if all are the same, or NaN.")]
		public new float Padding
		{
			get => base.Padding;
			set => base.Padding = value;
		}
		[Description("Spacing - one number if both are the same, or NaN.")]
		public new float Spacing
		{
			get => base.Spacing;
			set => base.Spacing = value;
		}
	}
}

using RedOnion.UI.Components;
using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public partial class Element : IDisposable
	{
		protected GameObject GameObject { get; private set; }
		protected RectTransform RectTransform { get; private set; }

		public string Name
		{
			get => GameObject.name ?? GetType().FullName;
			set => GameObject.name = value;
		}
		public Element(string name = null)
		{
			GameObject = new GameObject(name);
			GameObject.layer = UILayer;
			RectTransform = GameObject.AddComponent<RectTransform>();
			RectTransform.pivot = new Vector2(.5f, .5f);
			RectTransform.anchorMin = new Vector2(.5f, .5f);
			RectTransform.anchorMax = new Vector2(.5f, .5f);
		}

		// virtual so that we can later redirect it in Window (to content panel)
		protected virtual void AddElement(Element element)
			=> element.GameObject.transform.SetParent(GameObject.transform, false);
		protected virtual void RemoveElement(Element element)
		{
			if (element.GameObject.transform.parent == GameObject.transform)
				element.GameObject.transform.SetParent(null);
		}

		public E Add<E>(E element) where E : Element
		{
			AddElement(element);
			return element;
		}
		public E Remove<E>(E element) where E : Element
		{
			RemoveElement(element);
			return element;
		}
		// to prevent matching the later (params) instead of the above (generic)
		public Element Add(Element element)
		{
			AddElement(element);
			return element;
		}
		public Element Remove(Element element)
		{
			RemoveElement(element);
			return element;
		}
		public void Add(params Element[] elements)
		{
			foreach (var element in elements)
				Add(element);
		}
		public void Remove(params Element[] elements)
		{
			foreach (var element in elements)
				Remove(element);
		}

		~Element() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			GameObject.DestroyGameObject();
			GameObject = null;
			RectTransform = null;
			layoutElement = null;
			layoutGroup = null;
		}

		/* TODO: Use our LayoutComponent
		public Anchors Anchors
		{
			get => new Anchors(RectTransform);
			set
			{
				RectTransform.anchorMin = new Vector2(value.left, 1f-value.bottom);
				RectTransform.anchorMax = new Vector2(value.right, 1f-value.top);
			}
		}
		*/

		// TODO: Use our LayoutComponent
		private UUI.LayoutElement layoutElement;
		private UUI.LayoutElement LayoutElement
		{
			get
			{
				if (layoutElement == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					layoutElement = GameObject.AddComponent<UUI.LayoutElement>();
					layoutElement.layoutPriority = 3;
				}
				return layoutElement;
			}
		}
		private float ConvertLayoutElementValue(float? value)
			=> value.HasValue && value.Value >= 0f ? value.Value : float.NaN;
		public float MinWidth
		{
			get => ConvertLayoutElementValue(layoutElement?.minWidth);
			set => LayoutElement.minWidth = value >= 0f ? value : -1f;
		}
		public float MinHeight
		{
			get => ConvertLayoutElementValue(layoutElement?.minHeight);
			set => LayoutElement.minHeight = value >= 0f ? value : -1f;
		}
		public float PreferWidth
		{
			get => ConvertLayoutElementValue(layoutElement?.preferredWidth);
			set => LayoutElement.preferredWidth = value >= 0f ? value : -1f;
		}
		public float PreferHeight
		{
			get => ConvertLayoutElementValue(layoutElement?.preferredHeight);
			set => LayoutElement.preferredHeight = value >= 0f ? value : -1f;
		}
		public float FlexWidth
		{
			get => ConvertLayoutElementValue(layoutElement?.flexibleWidth);
			set => LayoutElement.flexibleWidth = value >= 0f ? value : -1f;
		}
		public float FlexHeight
		{
			get => ConvertLayoutElementValue(layoutElement?.flexibleHeight);
			set => LayoutElement.flexibleHeight = value >= 0f ? value : -1f;
		}

		// TODO: Use our LayoutComponent
		private UUI.HorizontalOrVerticalLayoutGroup layoutGroup;
		private Layout layout;
		public Layout Layout
		{
			get => layout;
			set
			{
				if (value == layout)
					return;
				if (layout == Layout.Horizontal || layout == Layout.Vertical)
				{
					GameObject.Destroy(layoutGroup);
					layoutGroup = null;
				}
				layout = value;
				if (value == Layout.Horizontal || value == Layout.Vertical)
				{
					layoutGroup = value == Layout.Horizontal
						? (UUI.HorizontalOrVerticalLayoutGroup)
						GameObject.AddComponent<UUI.HorizontalLayoutGroup>()
						: GameObject.AddComponent<UUI.VerticalLayoutGroup>();
					layoutGroup.childControlHeight = true;
					layoutGroup.childControlWidth = true;
					ApplyChildAnchors();
					ApplyPadding();
				}
			}
		}

		private LayoutPadding layoutPadding = new LayoutPadding(3);
		/// <summary>
		/// Padding and spacing
		/// </summary>
		public LayoutPadding LayoutPadding
		{
			get => layoutPadding;
			set
			{
				if (layoutPadding == value)
					return;
				layoutPadding = value;
				if (layoutGroup != null)
					ApplyPadding();
			}
		}
		private void ApplyPadding()
		{
			layoutGroup.padding = layoutPadding.ToRectOffset();
			layoutGroup.spacing = layout == Layout.Horizontal
				? layoutPadding.xgap : layoutPadding.ygap;
		}

		private Anchors childAnchors = Anchors.Invalid;
		/// <summary>
		/// Override anchors of all children
		/// (use Anchors.Invalid to reset)
		/// </summary>
		public Anchors ChildAnchors
		{
			get => childAnchors;
			set
			{
				if (childAnchors == value)
					return;
				childAnchors = value;
				if (layoutGroup != null)
					ApplyChildAnchors();
			}
		}
		private void ApplyChildAnchors()
		{
			layoutGroup.childAlignment = childAnchors.ToTextAnchor();
			layoutGroup.childForceExpandWidth = childAnchors.right + 1f/3f > childAnchors.left;
			layoutGroup.childForceExpandHeight = childAnchors.bottom + 1f/3f > childAnchors.top;
		}

		public Padding InnerPadding
		{
			get => layoutPadding.Padding;
			set => layoutPadding.Padding = value;
		}
		public Vector2 InnerSpacing
		{
			get => layoutPadding.Spacing;
			set => layoutPadding.Spacing = value;
		}
		public float Padding
		{
			get => layoutPadding.Padding.All;
			set => layoutPadding.Padding = new Padding(value);
		}
		public float Spacing
		{
			get => layoutPadding.xgap == layoutPadding.ygap ? layoutPadding.xgap : float.NaN;
			set => layoutPadding.Spacing = new Vector2(value, value);
		}
	}
}

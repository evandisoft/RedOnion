using RedOnion.UI.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public partial class Element : IDisposable
	{
		// these may become protected later
		public GameObject GameObject { get; private set; }
		public RectTransform RectTransform { get; private set; }

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

		// TODO: Use our LayoutComponent
		public Anchors Anchors
		{
			get => new Anchors(RectTransform);
			set
			{
				RectTransform.anchorMin = new Vector2(value.left, 1f-value.bottom);
				RectTransform.anchorMax = new Vector2(value.right, 1f-value.top);
			}
		}

		// TODO: Use our LayoutComponent
		public UUI.LayoutElement layoutElement;
		private UUI.LayoutElement LayoutElement
		{
			get
			{
				if (layoutElement == null)
				{
					if (GameObject == null)
						throw new ObjectDisposedException(Name);
					layoutElement = GameObject.AddComponent<UUI.LayoutElement>();
				}
				return layoutElement;
			}
		}
		private float? ConvertLayoutElementValue(float? value)
			=> value.HasValue && !(value.Value >= 0f) ? null : value;
		public float? MinWidth
		{
			get => ConvertLayoutElementValue(layoutElement?.minWidth);
			set => LayoutElement.minWidth = value ?? -1f;
		}
		public float? MinHeight
		{
			get => ConvertLayoutElementValue(layoutElement?.minHeight);
			set => LayoutElement.minHeight = value ?? -1f;
		}
		public float? PreferWidth
		{
			get => ConvertLayoutElementValue(layoutElement?.preferredWidth);
			set => LayoutElement.preferredWidth = value ?? -1f;
		}
		public float? PreferHeight
		{
			get => ConvertLayoutElementValue(layoutElement?.preferredHeight);
			set => LayoutElement.preferredHeight = value ?? -1f;
		}
		public float? FlexWidth
		{
			get => ConvertLayoutElementValue(layoutElement?.flexibleWidth);
			set => LayoutElement.flexibleWidth = value ?? -1f;
		}
		public float? FlexHeight
		{
			get => ConvertLayoutElementValue(layoutElement?.flexibleHeight);
			set => LayoutElement.flexibleHeight = value ?? -1f;
		}

		// TODO: Use our LayoutComponent
		public UUI.HorizontalOrVerticalLayoutGroup layoutGroup;
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
					if (value == Layout.Horizontal || value == Layout.Vertical)
					{
						var align = layoutGroup.childAlignment;
						var padding = layoutGroup.padding;
						var spacing = layoutGroup.spacing;
						GameObject.Destroy(layoutGroup);
						layoutGroup = value == Layout.Horizontal
							? (UUI.HorizontalOrVerticalLayoutGroup)
							GameObject.AddComponent<UUI.HorizontalLayoutGroup>()
							: GameObject.AddComponent<UUI.VerticalLayoutGroup>();
						layoutGroup.childControlHeight = true;
						layoutGroup.childControlWidth = true;
						layoutGroup.childForceExpandHeight = false;
						layoutGroup.childForceExpandWidth = false;
						layoutGroup.childAlignment = align;
						layoutGroup.padding = padding;
						layoutGroup.spacing = spacing;
						layout = value;
						return;
					}
					GameObject.Destroy(layoutGroup);
					layoutGroup = null;
					layout = value;
					return;
				}
				if (value == Layout.Horizontal || value == Layout.Vertical)
				{
					layoutGroup = value == Layout.Horizontal
						? (UUI.HorizontalOrVerticalLayoutGroup)
						GameObject.AddComponent<UUI.HorizontalLayoutGroup>()
						: GameObject.AddComponent<UUI.VerticalLayoutGroup>();
					layoutGroup.childControlHeight = true;
					layoutGroup.childControlWidth = true;
					layoutGroup.childForceExpandHeight = false;
					layoutGroup.childForceExpandWidth = false;
					layoutGroup.childAlignment = TextAnchor.MiddleCenter;
					layoutGroup.padding = new RectOffset(3, 3, 3, 3);
					layoutGroup.spacing = 3;
					layout = value;
					return;
				}
				layout = value;
			}
		}
		public RectOffset Padding
		{
			get => layoutGroup?.padding ?? new RectOffset(3, 3, 3, 3);
			set
			{
				if (layoutGroup != null)
					layoutGroup.padding = value;
			}
		}
		public float Spacing
		{
			get => layoutGroup?.spacing ?? 3f;
			set
			{
				if (layoutGroup != null)
					layoutGroup.spacing = value;
			}
		}
		public TextAnchor ChildAlignment
		{
			get => layoutGroup?.childAlignment ?? TextAnchor.MiddleCenter;
			set
			{
				if (layoutGroup != null)
					layoutGroup.childAlignment = value;
			}
		}

		/*
		public Vector2 Position
		{
			get
			{
				var pt = RectTransform.anchoredPosition;
				return new Vector2(
					(_anchors & Anchors.Left) != 0 ? pt.x : -pt.x,
					(_anchors & Anchors.Top) != 0 ? -pt.y : pt.y);
			}
			set
			{
				var prev = RectTransform.anchoredPosition;
				var next = new Vector2(
					(_anchors & Anchors.Left) != 0 ? value.x : -value.x,
					(_anchors & Anchors.Top) != 0 ? -value.y : value.y);
				if (prev == next)
					return;
				RectTransform.anchoredPosition = next;
			}
		}
		public Vector2 SizeDelta
		{
			get
			{
				var sz = RectTransform.sizeDelta;
				if (_anchors == Anchors.TopLeft)
					return sz;
				var pt = RectTransform.anchoredPosition;
				return new Vector2(
					(_anchors & Anchors.LeftRight) == Anchors.LeftRight ? -sz.x-pt.x : sz.x,
					(_anchors & Anchors.TopBottom) == Anchors.TopBottom ? -sz.y+pt.y : sz.y);
			}
			set => SetSizeDelta(value);
		}
		protected virtual void SetSizeDelta(Vector2 value)
		{
			if (_anchors == Anchors.TopLeft)
				RectTransform.sizeDelta = value;
			else
			{
				var pt = RectTransform.anchoredPosition;
				RectTransform.sizeDelta = new Vector2(
					(_anchors & Anchors.LeftRight) == Anchors.LeftRight ? -value.x-pt.x : value.x,
					(_anchors & Anchors.TopBottom) == Anchors.TopBottom ? -value.y+pt.y : value.y);
			}
		}
		*/
	}
}

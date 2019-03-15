using System;
using System.Collections.Generic;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public enum Anchors
	{
		Top		= 1<<0,
		Bottom	= 1<<1,
		Left	= 1<<2,
		Right	= 1<<3,

		Center = 0,
		TopLeft = Top|Left,
		TopRight = Top|Right,
		BottomLeft = Bottom|Left,
		BottomRight = Bottom|Right,

		TopBottom = Top|Bottom,
		TopBottomLeft = TopBottom|Left,
		TopBottomRight = TopBottom|Right,
		LeftRight = Left|Right,
		TopLeftRight = Top|LeftRight,
		BottomLeftRight = Bottom|LeftRight,
		Fill = TopBottom|LeftRight
	}

	public partial class Element: IDisposable
	{
		protected static readonly int UILayer = LayerMask.NameToLayer("UI");

		private static UISkinDef _defaultSkin;
		public static UISkinDef DefaultSkin
		{
			get => _defaultSkin ?? UISkinManager.defaultSkin;
			set => _defaultSkin = value;
		}

		protected internal GameObject GameObject { get; private set; }
		protected RectTransform RectTransform { get; private set; }

		public string Name { get => GameObject.name; set => GameObject.name = value; }

		public Element(string name = null)
		{
			GameObject = new GameObject(name);
			GameObject.layer = UILayer;
			RectTransform = GameObject.AddComponent<RectTransform>();
			RectTransform.anchorMin = new Vector2(0f, 1f);
			RectTransform.anchorMax = new Vector2(0f, 1f);
			RectTransform.pivot = new Vector2(0f, 1f);
			_anchors = Anchors.TopLeft;
		}

		public virtual void Add(Element element)
			=> element.GameObject.transform.SetParent(GameObject.transform, false);
		public void Add(params Element[] elements)
		{
			foreach (var element in elements)
				Add(element);
		}
		public virtual void Remove(Element element)
		{
			if (element.GameObject.transform.parent == GameObject.transform)
				element.GameObject.transform.SetParent(null);
		}
		protected void Remove(params Element[] elements)
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
		}

		private Anchors _anchors;
		public Anchors Anchors
		{
			get => _anchors;
			set
			{
				if (_anchors == value)
					return;
				RectTransform.anchorMin = new Vector2(
					(value & Anchors.Left) != 0 ? 0f : (value & Anchors.Right) != 0 ? 1f : 0.5f,
					(value & Anchors.Bottom) != 0 ? 0f : (value & Anchors.Top) != 0 ? 1f : 0.5f);
				RectTransform.anchorMax = new Vector2(
					(value & Anchors.Right) != 0 ? 1f : (value & Anchors.Left) != 0 ? 0f : 0.5f,
					(value & Anchors.Top) != 0 ? 1f : (value & Anchors.Bottom) != 0 ? 0f : 0.5f);
				RectTransform.pivot = new Vector2(
					(value & Anchors.LeftRight) == Anchors.Center ? 0.5f :
					(value & Anchors.LeftRight) != Anchors.Right ? 0f : 1f,
					(value & Anchors.TopBottom) == Anchors.Center ? 0.5f :
					(value & Anchors.TopBottom) != Anchors.Bottom ? 1f : 0f);
				_anchors = value;
			}
		}
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
				RectTransform.anchoredPosition = new Vector2(
					(_anchors & Anchors.Left) != 0 ? value.x : -value.x,
					(_anchors & Anchors.Top) != 0 ? -value.y : value.y); ;
			}
		}
		public float X
		{
			get => Position.x;
			set => Position = new Vector2(value, Y);
		}
		public float Y
		{
			get => Position.y;
			set => Position = new Vector2(X, value);
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
			set
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
		}
		public float W
		{
			get => SizeDelta.x;
			set => SizeDelta = new Vector2(value, H);
		}
		public float H
		{
			get => SizeDelta.y;
			set => SizeDelta = new Vector2(W, value);
		}
	}
}

using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public partial class Element: IDisposable
	{
		public Element Parent { get; internal set; }
		protected internal GameObject GameObject { get; private set; }
		protected RectTransform RectTransform { get; private set; }

		public Element(Element parent = null, string name = null)
		{
			GameObject = new GameObject(name);
			RectTransform = GameObject.AddComponent<RectTransform>();
			RectTransform.pivot = new Vector2(0, 1);
			RectTransform.anchorMin = new Vector2(0, 1);
			RectTransform.anchorMax = new Vector2(0, 1);
			RectTransform.anchoredPosition = Vector2.zero;
			RectTransform.sizeDelta = Vector2.zero;
			if ((Parent = parent) != null)
			{
				GameObject.transform.SetParent(parent.GameObject.transform, false);
				GameObject.SetLayerRecursive(parent.GameObject.layer);
			}
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

		public float X
		{
			get => RectTransform.anchoredPosition.x;
			set => RectTransform.anchoredPosition = new Vector2(value, RectTransform.position.y);
		}
		public float Y
		{
			get => -RectTransform.anchoredPosition.y;
			set => RectTransform.anchoredPosition = new Vector2(RectTransform.position.x, -value);
		}
		public float Width
		{
			get => RectTransform.sizeDelta.x;
			set => RectTransform.sizeDelta = new Vector2(value, RectTransform.sizeDelta.y);
		}
		public float Height
		{
			get => RectTransform.sizeDelta.y;
			set => RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, value);
		}
		public Vector2 Position
		{
			get => new Vector2(X, Y);
			set => RectTransform.anchoredPosition = new Vector3(value.x, -value.y, 0f);
		}
		public Vector2 Location
		{
			get => new Vector2(X, Y);
			set => RectTransform.anchoredPosition = new Vector3(value.x, -value.y, 0f);
		}
		public Vector2 SizeDelta
		{
			get => RectTransform.sizeDelta;
			set => RectTransform.sizeDelta = value;
		}
		public Rect Anchors
		{
			get => new Rect(
				RectTransform.anchorMin.x,
				1f - RectTransform.anchorMax.y,
				RectTransform.anchorMax.x,
				1f - RectTransform.anchorMin.y);
			set
			{
				RectTransform.anchorMin = new Vector2(value.xMin, 1f - value.yMax);
				RectTransform.anchorMax = new Vector2(value.xMax, 1f - value.yMin);
			}
		}
		public Vector2 Pivot
		{
			get => new Vector2(RectTransform.pivot.x, -RectTransform.pivot.y);
			set => RectTransform.pivot = new Vector2(value.x, -value.y);
		}
	}
}

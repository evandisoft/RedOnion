using KSP.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Window : Panel
	{
		public struct Defaults
		{
			public Vector2 Position;
			public Vector2 SizeDelta;
			public Vector2 CloseButtonSize;
			public Texture2D CloseButtonIcon;
			public float FrameWidth;
			public Color TitleTextColor;
		}
		public static Defaults Default = new Defaults
		{
			Position = new Vector2(400, 400),
			SizeDelta = new Vector2(400, 300),
			CloseButtonSize = new Vector2(23, 23),
			CloseButtonIcon = LoadIcon(13, 13, "CloseButtonIcon.png"),
			FrameWidth = 4,
			TitleTextColor = new Color(0.8f, 0.8f, 0.8f, 0.8f)
		};

		protected Label TitleLabel { get; private set; }
		protected Button CloseButton { get; private set; }
		protected Panel ContentPanel { get; private set; }

		public Window(string name = null)
			: base(name ?? "Window")
		{
			GameObject.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform, false);

			Position = Default.Position;
			SizeDelta = Default.SizeDelta;
			Color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

			AddToFrame(TitleLabel = new Label("Window Title")
			{
				Anchors = Anchors.TopLeftRight,
				Position = new Vector2(Default.FrameWidth, Default.FrameWidth),
				SizeDelta = new Vector2(Default.CloseButtonSize.x + Default.FrameWidth, Default.CloseButtonSize.y),
				Text = "Window",
				TextColor = Default.TitleTextColor
			});
			AddToFrame(CloseButton = new Button("Window Close Button")
			{
				Anchors = Anchors.TopRight,
				Position = new Vector2(Default.FrameWidth, Default.FrameWidth),
				SizeDelta = new Vector2(Default.CloseButtonSize.x, Default.CloseButtonSize.y),
				IconTexture = Default.CloseButtonIcon
			});
			AddToFrame(ContentPanel = new Panel("Window Content Panel")
			{
				Anchors = Anchors.Fill,
				Position = new Vector2(Default.FrameWidth, Default.CloseButtonSize.y + 2*Default.FrameWidth),
				SizeDelta = new Vector2(Default.FrameWidth, Default.FrameWidth)
			});

			CloseButton.Click += Close;
			GameObject.AddComponent<DragHandler>();
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			Hide();
			base.Dispose(true);
			TitleLabel?.Dispose();
			TitleLabel = null;
			CloseButton?.Dispose();
			CloseButton = null;
		}

		protected void AddToFrame(Element e)
			=> base.Add(e);
		protected void RemoveFromFrame(Element e)
			=> base.Remove(e);
		public override void Add(Element e)
			=> ContentPanel.Add(e);
		public override void Remove(Element element)
			=> ContentPanel.Remove(element);

		public void Show()
		{
			if (GameObject == null)
				throw new ObjectDisposedException(GetType().Name);
			GameObject.SetActive(true);
		}
		public void Hide()
		{
			if (GameObject == null)
				throw new ObjectDisposedException(GetType().Name);
			GameObject.SetActive(false);
		}

		public event UnityAction Closed;
		public virtual void Close()
		{
			Hide();
			Closed?.Invoke();
		}

		public string Title
		{
			get => TitleLabel.Text;
			set => TitleLabel.Text = value;
		}

		[RequireComponent(typeof(RectTransform))]
		public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
		{
			private Vector2 dragDelta = new Vector2(float.NaN, float.NaN);
			void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
				=> dragDelta = GetComponent<RectTransform>().anchoredPosition - eventData.position;
			void IEndDragHandler.OnEndDrag(PointerEventData eventData)
				=> dragDelta = new Vector2(float.NaN, float.NaN);
			void IDragHandler.OnDrag(PointerEventData eventData)
			{
				if (!float.IsNaN(dragDelta.x))
					GetComponent<RectTransform>().anchoredPosition = dragDelta + eventData.position;
			}
		}
	}
}

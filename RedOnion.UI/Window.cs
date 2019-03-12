using KSP.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
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

		public Window(string name = null)
			: base(name)
		{
			GameObject.transform.SetParent(UIMasterController.Instance.appCanvas.transform, false);
			GameObject.SetLayerRecursive(UIMasterController.Instance.appCanvas.gameObject.layer);

			Position = Default.Position;
			SizeDelta = Default.SizeDelta;
			Color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

			Add(TitleLabel = new Label("Title")
			{
				Anchors = Anchors.TopLeftRight,
				Position = new Vector2(Default.FrameWidth, Default.FrameWidth),
				SizeDelta = new Vector2(Default.CloseButtonSize.x + Default.FrameWidth, Default.CloseButtonSize.y),
				Text = "Window",
				TextColor = Default.TitleTextColor
			});

			Add(CloseButton = new Button("CloseButton")
			{
				Anchors = Anchors.TopRight,
				Position = new Vector2(Default.FrameWidth, Default.FrameWidth),
				SizeDelta = new Vector2(Default.CloseButtonSize.x, Default.CloseButtonSize.y),
				IconTexture = Default.CloseButtonIcon
			});
			CloseButton.Click += Close;
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
	}
}

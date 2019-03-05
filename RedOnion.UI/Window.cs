using KSP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;

using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Window : Element, IDisposable
	{
		public struct Defaults
		{
			public DefaultControls.Resources Frame;
			public DefaultControls.Resources Title;
			public DefaultControls.Resources Close;

			public Vector2 Position;
			public Vector2 Size;
			public float FrameWidth;
			public Vector2 CloseButtonSize;
			public Texture2D CloseButtonIcon;
		}
		public static Defaults Default = new Defaults
		{
			Position = new Vector2(40, 40),
			Size = new Vector2(400, 300),
			FrameWidth = 4,
			CloseButtonSize = new Vector2(17, 17),
			CloseButtonIcon = LoadIcon(13, 13, "CloseButtonIcon.png")
		};

		protected GameObject FrameObject { get; private set; }
		protected RectTransform FrameRect { get; private set; }
		protected UUI.Image FrameImage { get; private set; }

		protected GameObject TitleObject { get; private set; }
		protected RectTransform TitleRect { get; private set; }
		protected UUI.Text TitleText { get; private set; }

		protected GameObject CloseObject { get; private set; }
		protected RectTransform CloseRect { get; private set; }
		protected UUI.Button CloseButton { get; private set; }
		protected UUI.Text CloseButtonText { get; private set; }

		public Window()
		{

			// main frame
			FrameObject = DefaultControls.CreatePanel(Default.Frame);
			FrameObject.transform.SetParent(UIMasterController.Instance.appCanvas.transform);
			// anchoring: top-left
			FrameRect = FrameObject.GetComponent<RectTransform>();
			FrameRect.anchorMin = new Vector2(0, 1);
			FrameRect.anchorMax = new Vector2(0, 1);
			FrameRect.pivot = new Vector2(0, 1);
			// position
			Position = Default.Position;
			Size = Default.Size;
			// background image
			FrameImage = FrameObject.GetComponent<UnityEngine.UI.Image>();
			FrameImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

			// title text
			TitleObject = DefaultControls.CreateText(Default.Title);
			TitleObject.transform.SetParent(FrameObject.transform, false);
			// anchoring: top-left-right
			TitleRect = TitleObject.GetComponent<RectTransform>();
			TitleRect.anchorMin = new Vector2(0, 1);
			TitleRect.anchorMax = new Vector2(1, 1);
			TitleRect.pivot = new Vector2(0, 1);
			// position
			TitleRect.anchoredPosition = new Vector3(Default.FrameWidth, -Default.FrameWidth);
			TitleRect.sizeDelta = new Vector2(-Default.CloseButtonSize.x - Default.FrameWidth, Default.CloseButtonSize.y);
			// text component
			TitleText = TitleObject.GetComponent<UnityEngine.UI.Text>();
			TitleText.text = "Window";
			TitleText.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
			TitleText.font = UISkinManager.defaultSkin.font;
			TitleText.fontSize = 14;

			// close button
			CloseObject = DefaultControls.CreateButton(Default.Close);
			CloseObject.transform.SetParent(FrameObject.transform, false);
			// anchoring: top-right
			CloseRect = CloseObject.GetComponent<RectTransform>();
			CloseRect.anchorMin = new Vector2(1, 1);
			CloseRect.anchorMax = new Vector2(1, 1);
			CloseRect.pivot = new Vector2(1, 1);
			// position
			CloseRect.anchoredPosition = new Vector3(-Default.FrameWidth, -Default.FrameWidth);
			CloseRect.sizeDelta = new Vector2(Default.CloseButtonSize.x, Default.CloseButtonSize.y);
			// button component
			CloseButton = CloseObject.GetComponent<UnityEngine.UI.Button>();
			CloseButton.onClick.AddListener(Close);
			// button text
			CloseButtonText = CloseObject.GetComponentInChildren<Text>();
			CloseButtonText.text = "";
		}

		public void Dispose()
		{
			if (FrameObject == null)
				return;
			Hide();
			FrameObject.DestroyGameObject();
			FrameObject = null;
			FrameRect = null;
			FrameImage = null;
			TitleObject = null;
			TitleRect = null;
			TitleText = null;
			CloseObject = null;
			CloseRect = null;
			CloseButton = null;
		}

		public void Show()
		{
			if (FrameObject == null)
				throw new ObjectDisposedException(nameof(Window));
			FrameObject.SetActive(true);
		}
		public void Hide()
		{
			if (FrameObject == null)
				throw new ObjectDisposedException(nameof(Window));
			FrameObject.SetActive(false);
		}
		public virtual void Close()
			=> Hide();
		public event UnityAction Closed
		{
			add => CloseButton.onClick.AddListener(value);
			remove => CloseButton.onClick.RemoveListener(value);
		}

		public float X
		{
			get => FrameRect.anchoredPosition.x;
			set => FrameRect.anchoredPosition = new Vector2(value, FrameRect.position.y);
		}
		public float Y
		{
			get => -FrameRect.anchoredPosition.y;
			set => FrameRect.anchoredPosition = new Vector2(FrameRect.position.x, -value);
		}
		public float Width
		{
			get => FrameRect.sizeDelta.x;
			set => FrameRect.sizeDelta = new Vector2(value, FrameRect.sizeDelta.y);
		}
		public float Height
		{
			get => FrameRect.sizeDelta.y;
			set => FrameRect.sizeDelta = new Vector2(FrameRect.sizeDelta.x, value);
		}
		public Vector2 Position
		{
			get => new Vector2(X, Y);
			set => FrameRect.anchoredPosition = new Vector3(value.x, -value.y, 0f);
		}
		public Vector2 Location
		{
			get => new Vector2(X, Y);
			set => FrameRect.anchoredPosition = new Vector3(value.x, -value.y, 0f);
		}
		public Vector2 Size
		{
			get => FrameRect.sizeDelta;
			set => FrameRect.sizeDelta = value;
		}
		public Rect Bounds
		{
			get => new Rect(Position, Size);
			set
			{
				Position = value.position;
				Size = value.size;
			}
		}
		public string Title
		{
			get => TitleText.text;
			set => TitleText.text = value;
		}
	}
}

using KSP.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Window : IDisposable
	{
		public class FramePanel : Panel
		{
			static Texture2D DefaultCloseButtonIcon = LoadIcon(13, 13, "CloseButtonIcon.png");

			public CanvasGroup Group { get; }
			public UUI.ContentSizeFitter Fitter { get; }
			public Element Header { get; }
			public Label Title { get; }
			public Button Close { get; }
			public Panel Content { get; }

			public FramePanel(string name = null)
				: base("Window Frame")
			{
				GameObject.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform, false);
				Group = GameObject.AddComponent<CanvasGroup>();
				Group.alpha = .9f;
				MinWidth = 160;
				MinHeight = 120;
				Fitter = GameObject.AddComponent<UUI.ContentSizeFitter>();
				Fitter.horizontalFit = UUI.ContentSizeFitter.FitMode.PreferredSize;
				Fitter.verticalFit = UUI.ContentSizeFitter.FitMode.PreferredSize;
				Color = new Color(.2f, .2f, .2f, .8f);
				Layout = Layout.Vertical;
				Padding = new RectOffset(4, 4, 4, 4);
				Spacing = 4;
				Header = Add(new Element("Window Title Row")
				{
					Layout = Layout.Horizontal,
					FlexWidth = 1f,
				});
				Title = Header.Add(new Label("Window Title")
				{
					Text = "Window",
					TextColor = Color.white,
					TextAlign = TextAnchor.MiddleLeft,
					FlexWidth = 1f
				});
				Close = Header.Add(new Button("Window Close Button")
				{
					IconTexture = DefaultCloseButtonIcon
				});
				Content = Add(new Panel("Window Content Panel")
				{
					Anchors = Anchors.Fill,
					Color = new Color(.5f, .5f, .5f, .5f),
					FlexWidth = 1f, FlexHeight = 1f
				});

				GameObject.AddComponent<Components.DragHandler>();
			}

			public bool Active
			{
				get => GameObject.activeSelf;
				set => GameObject.SetActive(value);
			}
			public bool Visible
			{
				get => GameObject.activeInHierarchy;
				set => GameObject.SetActive(value);
			}
		}

		public FramePanel Frame { get; private set; }
		public Panel Content { get; private set; }

		public string Name
		{
			get => Frame.Name;
			set => Frame.Name = value;
		}

		public Window(Layout layout)
			: this(null, layout) { }
		public Window(string name = null, Layout layout = Layout.Vertical)
		{
			Frame = new FramePanel(name);
			Content = Frame.Content;
			Content.Layout = layout;
			Frame.Close.Click += Close;
		}

		~Window() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing)
		{
			if (Frame == null)
				return;
			Frame.Dispose();
			Frame = null;
		}

		public void Show()
		{
			if (Frame == null)
				throw new ObjectDisposedException(GetType().Name);
			Frame.Visible = true;
		}
		public void Hide()
		{
			if (Frame == null)
				throw new ObjectDisposedException(GetType().Name);
			Frame.Visible = false;
		}

		public event UnityAction Closed;
		public virtual void Close()
		{
			Hide();
			Closed?.Invoke();
		}

		public string Title
		{
			get => Frame.Title.Text;
			set => Frame.Title.Text = value;
		}

		public float Alpha
		{
			get => Frame.Group.alpha;
			set => Frame.Group.alpha = value;
		}

		public float MinWidth
		{
			get => Frame.MinWidth.Value;
			set => Frame.MinWidth = Mathf.Max(80f, value);
		}
		public float MinHeight
		{
			get => Frame.MinHeight.Value;
			set => Frame.MinHeight = Mathf.Max(80f, value);
		}
		public float? PreferWidth
		{
			get => Frame.PreferWidth;
			set => Frame.PreferWidth = value;
		}
		public float? PreferHeight
		{
			get => Frame.PreferHeight;
			set => Frame.PreferHeight = value;
		}

		public E Add<E>(E element) where E : Element
			=> Content.Add(element);
		public E Remove<E>(E element) where E : Element
			=> Content.Add(element);
		public Element Add(Element element)
			=> Content.Add(element);
		public Element Remove(Element element)
			=> Content.Remove(element);
		public void Add(params Element[] elements)
			=> Content.Add(elements);
		public void Remove(params Element[] elements)
			=> Content.Remove(elements);
	}
}

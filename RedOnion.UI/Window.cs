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
			static Texture2D DefaultCloseButtonIcon = LoadIcon(13, 13, "WindowCloseButtonIcon.png");

			public new GameObject GameObject => base.GameObject;
			public new RectTransform RectTransform => base.RectTransform;
			public CanvasGroup Group { get; }
			public UUI.ContentSizeFitter Fitter { get; }
			public Panel Header { get; }
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
				LayoutPadding = new LayoutPadding(4);
				Header = Add(new Panel("Window Title Row")
				{
					Layout = Layout.Horizontal,
					FlexWidth = 1f,
				});
				Title = Header.Add(new Label("Window Title")
				{
					Text = "Window",
					TextColor = Color.white,
					TextAlign = TextAnchor.MiddleLeft,
					FlexWidth = 1f,
				});
				Close = Header.Add(new Button("Window Close Button")
				{
					IconTexture = DefaultCloseButtonIcon,
					Padding = 3f, Spacing = 3f,
				});
				Content = Add(new Panel("Window Content Panel")
				{
					Color = new Color(.5f, .5f, .5f, .5f),
					FlexWidth = 1f, FlexHeight = 1f,
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

		public string Name
		{
			get => Frame.Name;
			set => Frame.Name = value;
		}
		public SceneFlags Scenes { get; set; }
		public FramePanel Frame { get; private set; }
		public Panel Content { get; private set; }

		public Window(Layout layout)
			: this(null, layout) { }
		public Window(string name = null, Layout layout = Layout.Vertical)
		{
			Scenes = (SceneFlags)(1 << (int)HighLogic.LoadedScene);
			Frame = new FramePanel(name);
			Content = Frame.Content;
			Content.Layout = layout;
			Frame.Close.Click += Close;
			GameEvents.onGameSceneLoadRequested.Add(SceneChange);
		}

		~Window() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing)
		{
			if (Frame == null)
				return;
			Frame.Dispose();
			Frame = null;
			GameEvents.onGameSceneLoadRequested.Remove(SceneChange);
		}
		void SceneChange(GameScenes scene)
		{
			if (((int)Scenes & (1 << (int)scene)) == 0)
				Dispose();
		}

		public void Show()
		{
			if (Frame == null)
				throw new ObjectDisposedException(Name);
			Frame.Visible = true;
		}
		public void Hide()
		{
			if (Frame == null)
				throw new ObjectDisposedException(Name);
			Frame.Visible = false;
		}

		readonly Event closed = new Event(new UnityEvent());
		public Event Closed
		{
			get => closed;
			set { }
		}
		public virtual void Close()
		{
			Hide();
			Closed.Invoke();
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
			get => Frame.MinWidth;
			set => Frame.MinWidth = Mathf.Max(80f, value);
		}
		public float MinHeight
		{
			get => Frame.MinHeight;
			set => Frame.MinHeight = Mathf.Max(80f, value);
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

		public Vector2 Position
		{
			get
			{
				var pt = Frame.RectTransform.anchoredPosition;
				return new Vector2(pt.x, -pt.y);
			}
			set
			{
				Frame.RectTransform.anchoredPosition = new Vector2(value.x, -value.y);
			}
		}
		public float X
		{
			get => Frame.RectTransform.anchoredPosition.x;
			set => Position = new Vector2(value, Y);
		}
		public float Y
		{
			get => -Frame.RectTransform.anchoredPosition.y;
			set => Position = new Vector2(X, value);
		}
		public Vector2 Size
		{
			get => Frame.RectTransform.sizeDelta;
			set
			{
				Frame.Fitter.horizontalFit = value.x >= MinWidth
					? UUI.ContentSizeFitter.FitMode.Unconstrained
					: UUI.ContentSizeFitter.FitMode.PreferredSize;
				Frame.Fitter.verticalFit = value.y >= MinHeight
					? UUI.ContentSizeFitter.FitMode.Unconstrained
					: UUI.ContentSizeFitter.FitMode.PreferredSize;
				Frame.RectTransform.sizeDelta = new Vector2(
					value.x >= MinWidth ? value.x : Width,
					value.y >= MinHeight ? value.y : Height);
			}
		}
		public float Width
		{
			get => Size.x;
			set
			{
				Frame.Fitter.horizontalFit = value >= MinWidth
					? UUI.ContentSizeFitter.FitMode.Unconstrained
					: UUI.ContentSizeFitter.FitMode.PreferredSize;
				if (value >= MinWidth)
					Frame.RectTransform.sizeDelta = new Vector2(value, Height);
			}
		}
		public float Height
		{
			get => Size.y;
			set
			{
				Frame.Fitter.verticalFit = value >= MinHeight
					? UUI.ContentSizeFitter.FitMode.Unconstrained
					: UUI.ContentSizeFitter.FitMode.PreferredSize;
				if (value >= MinHeight)
					Frame.RectTransform.sizeDelta = new Vector2(Width, value);
			}
		}

		public Color FrameColor
		{
			get => Frame.Color;
			set => Frame.Color = value;
		}
		public Texture FrameTexture
		{
			get => Frame.Texture;
			set => Frame.Texture = value;
		}
		public Color HeaderColor
		{
			get => Frame.Header.Color;
			set => Frame.Header.Color = value;
		}
		public Texture HeaderTexture
		{
			get => Frame.Header.Texture;
			set => Frame.Header.Texture = value;
		}
		public Color TitleColor
		{
			get => Frame.Title.TextColor;
			set => Frame.Title.TextColor = value;
		}
		public Color ContentColor
		{
			get => Content.Color;
			set => Content.Color = value;
		}
		public Texture ContentTexture
		{
			get => Content.Texture;
			set => Content.Texture = value;
		}
		public Layout Layout
		{
			get => Content.Layout;
			set => Content.Layout = value;
		}
		public LayoutPadding LayoutPadding
		{
			get => Content.LayoutPadding;
			set => Content.LayoutPadding = value;
		}
		public Anchors ChildAnchors
		{
			get => Content.ChildAnchors;
			set => Content.ChildAnchors = value;
		}
		public Padding InnerPadding
		{
			get => Content.InnerPadding;
			set => Content.InnerPadding = value;
		}
		public Vector2 InnerSpacing
		{
			get => Content.InnerSpacing;
			set => Content.InnerSpacing = value;
		}
		public float Padding
		{
			get => Content.Padding;
			set => Content.Padding = value;
		}
		public float Spacing
		{
			get => Content.Spacing;
			set => Content.Spacing = value;
		}
	}
}

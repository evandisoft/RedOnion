using KSP.UI;
using System;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[Description(
@"Window is the root of all UI elements.
Make sure to keep reference to it and dispose it when you are done with it.
It may get garbage-collected otherwise, but that can take time and is rather backup measure.")]
	public class Window : IDisposable
	{
		public class FramePanel : Panel
		{
			static Texture2D DefaultCloseButtonIcon = LoadIcon(13, 13, "WindowCloseButtonIcon.png");

			public CanvasGroup Group { get; }
			public UUI.ContentSizeFitter Fitter { get; }
			public Panel Header { get; }
			public Label Title { get; }
			public Button Close { get; }
			public Panel Content { get; }
			public SceneFlags Scenes { get; set; }

			// weak reference to main window to allow auto-dispose when there is no hard-reference to it
			protected readonly WeakReference window;

			public FramePanel(Window window)
			{
				this.window = new WeakReference(window);
				Scenes = (SceneFlags)(1 << (int)HighLogic.LoadedScene);
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
				LayoutPadding = new LayoutPadding(0f);
				Header = Add(new Panel
				{
					Layout = Layout.Horizontal,
					FlexWidth = 1f,
				});
				Title = Header.Add(new Label
				{
					Text = "Window",
					TextColor = Color.white,
					TextAlign = TextAnchor.MiddleLeft,
					FlexWidth = 1f,
				});
				Close = Header.Add(new Button
				{
					IconTexture = DefaultCloseButtonIcon,
					Padding = 3f, Spacing = 3f,
				});
				Content = Add(new Panel
				{
					Color = new Color(.5f, .5f, .5f, .5f),
					FlexWidth = 1f, FlexHeight = 1f,
				});

				GameObject.AddComponent<Components.DragHandler>();
				GameEvents.onGameSceneLoadRequested.Add(SceneChange);
				Close.Click.Add(CloseWindow);
			}
			protected override void Dispose(bool disposing)
			{
				if (!disposing || GameObject == null)
					return;
				GameEvents.onGameSceneLoadRequested.Remove(SceneChange);
				base.Dispose(disposing);
				(window?.Target as Window)?.Dispose();
				window.Target = null;
			}
			// this is here and not in window to not have any reference to the window from game-events
			void SceneChange(GameScenes scene)
			{
				if (((int)Scenes & (1 << (int)scene)) == 0)
					Dispose();
			}
			void CloseWindow(Button button)
			{
				var wnd = window?.Target as Window;
				if (wnd != null) wnd.Close();
				else Dispose();
			}
		}

		public string Name
		{
			get => Frame.Name;
			set => Frame.Name = value;
		}
		private FramePanel frame;
#if DEBUG
		public FramePanel Frame => frame;
#else
		protected FramePanel Frame => frame;
#endif
		public Panel Content { get; private set; }

		public Window() : this(Layout.Vertical) { }
		public Window(string title) : this(Layout.Vertical) => Title = title;
		public Window(string title, Layout layout) : this(layout) => Title = title;
		public Window(Layout layout, string title) : this(layout) => Title = title;
		public Window(Layout layout)
		{
			frame = new FramePanel(this);
			Content = frame.Content;
			Content.Layout = layout;
		}
		protected Window(FramePanel frame)
		{
			this.frame = frame;
			Content = frame.Content;
		}

		~Window() => Dispose(false);
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			// this is probably unnecessary (you cannot call Dispose unless you have reference
			// and the desctructor cannot be called when there is one, but for sure...)
			var frame = Interlocked.Exchange(ref this.frame, null);
			if (frame == null)
				return;
			if (disposing)
				frame.Dispose();
			// use the collector to call frame.Dispose() from main thread (Unity could complain otherwise)
			else Collector.Add(frame);
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

		class WindowClosedEvent : UnityEvent<Window> { }
		readonly WindowClosedEvent closed = new WindowClosedEvent();
		public Event<Window> Closed
			=> new Event<Window>(closed);
		public virtual void Close()
		{
			Hide();
			closed.Invoke(this);
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

		public Panel AddPanel(Layout layout)
			=> Content.AddPanel(layout);
		public Panel AddHorizontal()
			=> Content.AddHorizontal();
		public Panel AddVertical()
			=> Content.AddVertical();
		public Label AddLabel(string text)
			=> Content.AddLabel(text);
		public TextBox AddText(string text)
			=> Content.AddText(text);
		public TextBox AddTextBox(string text)
			=> Content.AddTextBox(text);
		public Button AddButton(string text)
			=> Content.AddButton(text);
		public Button AddButton(string text, UnityAction<Button> click)
			=> Content.AddButton(text, click);
		public Button AddToggle(string text)
			=> Content.AddToggle(text);
		public Button AddToggle(string text, UnityAction<Button> click)
			=> Content.AddToggle(text, click);
		public Button AddExclusive(string text)
			=> Content.AddExclusive(text);
		public Button AddExclusive(string text, UnityAction<Button> click)
			=> Content.AddExclusive(text, click);

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

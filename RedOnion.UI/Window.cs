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
				Spacing = 0f;
				Header = Add(new Panel
				{
					Layout = Layout.Horizontal,
					FlexWidth = 1f, Padding = 3f
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
					Padding = 3f
				});

				GameObject.AddComponent<Components.DragHandler>();
				GameEvents.onGameSceneLoadRequested.Add(SceneChange);
				Close.Click += CloseWindow;
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
				DebugLog("SceneChange({0}), Scenes = {1}", scene, Scenes);
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
		protected FramePanel Frame => frame;
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

		[Description("Dispose the window (and destroy its game object). Call this when you no longer need the window (e.g. from `Close` event).")]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		~Window() => Dispose(false);
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

		[Description("Show the window (`Visible = true`).")]
		public void Show()
		{
			if (Frame == null)
				throw new ObjectDisposedException(Name);
			Frame.Visible = true;
		}
		[Description("Hide the window (`Visible = false`).")]
		public void Hide()
		{
			if (Frame == null)
				throw new ObjectDisposedException(Name);
			Frame.Visible = false;
		}
		[Description("Window is set to be visible/active."
			+ " [`GameObject.activeSelf`](https://docs.unity3d.com/ScriptReference/GameObject-activeSelf.html),"
			+ " [`GameObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)")]
		public bool Active
		{
			get => Frame.Active;
			set => Frame.Active = value;
		}
		[Description("Window is visible (and all parents are)."
			+ " [`GameObject.activeInHierarchy`](https://docs.unity3d.com/ScriptReference/GameObject-activeInHierarchy.html),"
			+ " [`GameObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)")]
		public bool Visible
		{
			get => Frame.Visible;
			set => Frame.Visible = value;
		}

		[Description("The window was closed (by the 'X' button or by call to `Close`).")]
		public event Action<Window> Closed;
		[Description("Hide and close the window (invokes `Closed` event).")]
		public virtual void Close()
		{
			Hide();
			Closed?.Invoke(this);
		}

		[Description("Text in the title of the window.")]
		public string Title
		{
			get => Frame.Title.Text;
			set => Frame.Title.Text = value;
		}

		[Description("Transparency / alpha channel for the whole window (0.0 .. 1.0). Zero means invisible, one for opaque.")]
		public float Alpha
		{
			get => Frame.Group.alpha;
			set => Frame.Group.alpha = value;
		}

		[Description("Minimal width of the window. Default is 160, cannot be set below 80.")]
		public float MinWidth
		{
			get => Frame.MinWidth;
			set => Frame.MinWidth = Mathf.Max(80f, value);
		}
		[Description("Minimal height of the window. Default is 120, cannot be set below 40.")]
		public float MinHeight
		{
			get => Frame.MinHeight;
			set => Frame.MinHeight = Mathf.Max(40f, value);
		}

		public E Add<E>(E element) where E : Element
			=> Content.Add(element);
		public E Remove<E>(E element) where E : Element
			=> Content.Add(element);

		[Description("Add new element onto this panel.")]
		public Element Add(Element element)
			=> Content.Add(element);
		[Description("Remove element from this panel.")]
		public Element Remove(Element element)
			=> Content.Remove(element);

		public void Add(params Element[] elements)
			=> Content.Add(elements);
		public void Remove(params Element[] elements)
			=> Content.Remove(elements);

		[Description("Add new panel with specified layout.")]
		public Panel AddPanel(Layout layout)
			=> Content.AddPanel(layout);
		[Description("Add new panel with horizontal layout.")]
		public Panel AddHorizontal()
			=> Content.AddHorizontal();
		[Description("Add new panel with vertical layout.")]
		public Panel AddVertical()
			=> Content.AddVertical();

		[Description("Add new label with specified text.")]
		public Label AddLabel(string text)
			=> Content.AddLabel(text);

		public TextBox AddText(string text)
			=> Content.AddText(text);
		[Description("Add new label with specified text.")]
		public TextBox AddTextBox(string text)
			=> Content.AddTextBox(text);

		[Description("Add new button with specified text.")]
		public Button AddButton(string text)
			=> Content.AddButton(text);
		[Description("Add new button with specified text and click-action.")]
		public Button AddButton(string text, Action<Button> click)
			=> Content.AddButton(text, click);

		[Description("Add new toggle-button with specified text.")]
		public Button AddToggle(string text)
			=> Content.AddToggle(text);
		[Description("Add new toggle-button with specified text and click-action.")]
		public Button AddToggle(string text, Action<Button> click)
			=> Content.AddToggle(text, click);

		[Description("Add new exclusive toggle-button (radio button) with specified text.")]
		public Button AddExclusive(string text)
			=> Content.AddExclusive(text);
		[Description("Add new exclusive toggle-button with specified text and click-action.")]
		public Button AddExclusive(string text, Action<Button> click)
			=> Content.AddExclusive(text, click);

		[Description("Position of the anchor of the window."
			+ " (The anchor is by default in the center of the window and the window is placed in the center of the screen, which is \\[0,0\\].)"
			+ " Note that Y-coordinate is inverted from Unity to be top-down (where Unity is bottom-up).")]
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
		[Description("The X-coordinate of the position of the anchor of the window."
			+ " (The anchor is by default in the center of the window and 0 is in the center of the screen"
			+ " - you may use `unity.screen.width` to get the limits.)")]
		public float X
		{
			get => Frame.RectTransform.anchoredPosition.x;
			set => Position = new Vector2(value, Y);
		}
		[Description("The Y-coordinate of the position of the anchor of the window (inverted from Unity to be top-down, not bottom-up)."
			+ " (The anchor is by default in the center of the window and 0 is in the center of the screen"
			+ " - you may use `unity.screen.height` to get the limits.)")]
		public float Y
		{
			get => -Frame.RectTransform.anchoredPosition.y;
			set => Position = new Vector2(X, value);
		}
		[Description("The size of the window (or rather `RectTransform.sizeDelta`)."
			+ " Setting this value above curent `MinWidth`/`MinHeight` sets the integrated `ContentSizeFitter` to `Unconstrained`,"
			+ " which makes the window size be exactly what was specified."
			+ " Set to `PreferredSize` otherwise, to use the contained elements for size calculations.")]
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
		[Description("`Size.x`")]
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
		[Description("`Size.y`")]
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

		[Description("Background color of the frame.")]
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
		[Description("Background color of the header/title.")]
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
		[Description("Foreground color of the title.")]
		public Color TitleColor
		{
			get => Frame.Title.TextColor;
			set => Frame.Title.TextColor = value;
		}
		[Description("Background color of the content panel.")]
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
		[Description("Layout (how child elements of content panel are placed).")]
		public Layout Layout
		{
			get => Content.Layout;
			set => Content.Layout = value;
		}
		[Description("The combined inner padding and spacing (6 floats in total, all set to `3f` by default).")]
		public LayoutPadding LayoutPadding
		{
			get => Content.LayoutPadding;
			set => Content.LayoutPadding = value;
		}
		[Description(
@"This currently controls layout's `childAlignment` and
`childForceExpandWidth/Height`, but plan is to use custom `LayoutComponent`.
You can try `Anchors.Fill` (to make all inner elements fill their cell)
or `Anchors.MiddleLeft/MiddleCenter/UpperLeft...`")]
		public Anchors ChildAnchors
		{
			get => Content.ChildAnchors;
			set => Content.ChildAnchors = value;
		}
		[Description("Inner padding (border left empty inside this panel).")]
		public Padding InnerPadding
		{
			get => Content.InnerPadding;
			set => Content.InnerPadding = value;
		}
		[Description("Inner spacing (between elements).")]
		public Vector2 InnerSpacing
		{
			get => Content.InnerSpacing;
			set => Content.InnerSpacing = value;
		}
		[Description("`InnerPadding.All` - one number if all are the same, or NaN.")]
		public float Padding
		{
			get => Content.Padding;
			set => Content.Padding = value;
		}
		[Description("Spacing - one number if both are the same, or NaN.")]
		public float Spacing
		{
			get => Content.Spacing;
			set => Content.Spacing = value;
		}
	}
}

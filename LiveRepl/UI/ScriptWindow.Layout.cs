using System;
using Kerbalui.Gui;
using LiveRepl.UI.Panes;
using UnityEngine;

namespace LiveRepl.UI
{
	public partial class ScriptWindow
	{
		const int windowID = 0;
		public string Title = "Live REPL";

		public bool editorVisible = true;
		public bool replVisible = true;

		public const float windowHeight=600;
		public const float titleHeight = 20;
		public const float contentPaneHeight = windowHeight-titleHeight;
		public const float editorPaneWidth = 500;
		public const float centerPaneWidth = 100;
		public const float replPaneWidth = 400;
		public const float completionPaneWidth = 150;

		float WindowWidth()
		{
			float width=centerPaneWidth;
			if (editorVisible || replVisible)
			{
				width+=completionPaneWidth;
				if (editorVisible)
				{
					width+=editorPaneWidth;
				}
				if (replVisible)
				{
					width+=replPaneWidth;
				}
			}
			return width;
		}

		float WindowX()
		{
			float x=baseWindowPos.x;
			if (!editorVisible)
			{
				x+=editorPaneWidth;
			}
			return x;
		}

		Vector2 baseWindowPos=new Vector2(100,100);

		Rect windowRect;
		Rect titleBarRect;

		ContentPane contentPane;

		public void RecalculateRects()
		{
			windowRect=WindowRect();
			titleBarRect=TitleBarRect();
			contentPane.SetRect(new Rect(0, titleHeight, windowRect.width, windowRect.height-titleHeight));
		}

		Rect WindowRect()
		{
			return new Rect(WindowX(), baseWindowPos.y, WindowWidth(), windowHeight);
		}

		Rect TitleBarRect()
		{
			return new Rect(0, 0, WindowWidth(), titleHeight);
		}

		void InitLayout()
		{
			windowRect=WindowRect();
			contentPane=new ContentPane(this);
			RecalculateRects();
		}

		public void Update()
		{
			SetOrReleaseInputLock();
			windowRect=GUI.Window(windowID, windowRect, WindowFunction, Title);
		}

		void WindowFunction(int id)
		{
			GUI.DragWindow(titleBarRect);
			contentPane.Update();
		}

		public void SetOrReleaseInputLock()
		{
			if (windowRect.Contains(Event.current.mousePosition))
			{
				if (!inputIsLocked)
				{
					//Debug.Log("Input is now locked");
					inputIsLocked = true;
					InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
				}
			}
			else
			{
				if (inputIsLocked)
				{
					//Debug.Log("Input is no longer locked");
					inputIsLocked = false;
					InputLockManager.ClearControlLocks();
				}
			}
		}
	}
}

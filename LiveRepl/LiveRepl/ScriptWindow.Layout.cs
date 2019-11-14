using System;
using Kerbalui.Types;
using LiveRepl.Decorators;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow : Window
	{
		public const float windowHeight=600;
		public const float editorGroupWidth = 500;
		public const float centerGroupWidth = 100;
		public const float replGroupWidth = 400;
		public const float completionGroupWidth = 150;

		public const float startingX = 100;
		public const float startingY = 100;

		public ScriptWindowParts uiparts;

		void InitLayout()
		{
			uiparts=new ScriptWindowParts(this);
			AssignContent(uiparts.contentGroup);

			EditorVisible = bool.Parse(SavedSettings.LoadSetting("editorVisible", "true"));
			ReplVisible = bool.Parse(SavedSettings.LoadSetting("replVisible", "true"));
			rect.x = float.Parse(SavedSettings.LoadSetting("WindowPositionX", startingX.ToString()));
			rect.y = float.Parse(SavedSettings.LoadSetting("WindowPositionY",startingY.ToString()));
			rect.width=WindowWidth();
			rect.height=windowHeight;
		}

		public override void SetRect(Rect rect)
		{
			rect.width=WindowWidth();
			rect.height=windowHeight;

			base.SetRect(rect);
		}

		float WindowWidth()
		{
			float width=completionGroupWidth+centerGroupWidth;
			if (EditorVisible || ReplVisible)
			{
				if (EditorVisible)
				{
					width+=editorGroupWidth;
				}
				if (ReplVisible)
				{
					width+=replGroupWidth;
				}
			}
			return width;
		}



		private bool editorVisible = true;
		public bool EditorVisible
		{
			get => editorVisible;
			set
			{
				if (value!=editorVisible)
				{
					if (editorVisible)
					{
						rect.x+=editorGroupWidth;
						rect.width-=editorGroupWidth;
					}
					else
					{
						rect.x-=editorGroupWidth;
						rect.width+=editorGroupWidth;
					}

					uiparts.scriptDisabledEditorGroup.Active=editorVisible=value;
					needsResize=true;
				}
			}
		}

		private bool replVisible = true;
		public bool ReplVisible
		{
			get => replVisible;
			set
			{
				if (value!=replVisible)
				{
					if (replVisible)
					{
						rect.width-=replGroupWidth;
					}
					else
					{
						rect.width+=replGroupWidth;
					}

					uiparts.replGroup.Active=replVisible=value;
					needsResize=true;
				}
			}
		}
	}
}

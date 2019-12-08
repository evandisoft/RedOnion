using System;
using Kerbalui;
using Kerbalui.Types;
using LiveRepl.Decorators;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow : Window
	{
		public static float WindowHeight => baseWindowHeight * KerbaluiSettings.UI_SCALE;
		public static float EditorGroupWidth => baseEditorGroupWidth*KerbaluiSettings.UI_SCALE;
		public static float CenterGroupWidth => baseCenterGroupWidth*KerbaluiSettings.UI_SCALE;
		public static float ReplGroupWidth => baseReplGroupWidth*KerbaluiSettings.UI_SCALE;
		public static float CompletionGroupWidth => baseCompletionGroupWidth*KerbaluiSettings.UI_SCALE;

		public const float baseWindowHeight = 600;
		public const float baseEditorGroupWidth = 500;
		public const float baseCenterGroupWidth = 100;
		public const float baseReplGroupWidth = 400;
		public const float baseCompletionGroupWidth = 150;

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
			rect.width=WindowWidth;
			rect.height=WindowHeight;

			GameEvents.OnGameSettingsApplied.Add(GameSettingsChangeHandler);
		}

		void GameSettingsChangeHandler()
		{
			KerbaluiSettings.ApplySettingsChange();
			//titleStyle.fontSize=(int)(KerbaluiSettings.DefaultFontsize*GameSettings.UI_SCALE);
			needsResize=true;
		}

		public override void SetRect(Rect newRect)
		{
			newRect.width=WindowWidth;
			newRect.height=WindowHeight;

			base.SetRect(newRect);
		}

		float WindowWidth
		{
			get
			{
				float width=CenterGroupWidth+CompletionGroupWidth;
				if (EditorVisible)
				{
					width+=EditorGroupWidth;
				}
				if (ReplVisible)
				{
					width+=ReplGroupWidth;
				}
				return width;
			}
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
						//rect.x+=editorGroupWidth;
						rect.width-=EditorGroupWidth;
					}
					else
					{
						//rect.x-=editorGroupWidth;
						rect.width+=EditorGroupWidth;
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
						rect.x+=ReplGroupWidth;
						rect.width-=ReplGroupWidth;
					}
					else
					{
						rect.x-=ReplGroupWidth;
						rect.width+=ReplGroupWidth;
					}

					uiparts.replGroup.Active=replVisible=value;
					needsResize=true;
				}
			}
		}
	}
}

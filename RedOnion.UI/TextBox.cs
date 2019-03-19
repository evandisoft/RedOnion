using RedOnion.UI.Components;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class TextBox : Element
	{
		protected class InputField : UUI.InputField
		{
			readonly string lockID = "RedOnion.InputField";
			bool locked = false;
			public override void OnSelect(BaseEventData eventData)
			{
				InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, lockID);
				locked = true;
				base.OnSelect(eventData);
			}
			public override void OnDeselect(BaseEventData eventData)
			{
				InputLockManager.RemoveControlLock(lockID);
				locked = false;
				base.OnDeselect(eventData);
			}
			protected override void OnDestroy()
			{
				if (locked)
				{
					InputLockManager.RemoveControlLock(lockID);
					locked = false;
				}
				base.OnDestroy();
			}

			public override float minWidth => 40f;
			public override float preferredWidth => 100f;
		}
		protected class InputBox : Element
		{
			public InputField Input { get; private set; }
			public Label Label { get; private set; }
			public InputBox() : base("InputBox")
			{
				Input = GameObject.AddComponent<InputField>();
				Label = Add(new Label()
				{
					FlexWidth = 1,
					FlexHeight = 1,
					TextColor = Skin.textField.normal.textColor
				});
				Input.textComponent = Label.Core;
			}
			protected override void Dispose(bool disposing)
			{
				if (!disposing || GameObject == null)
					return;
				Input = null;
				Label.Dispose();
				Label = null;
				base.Dispose(true);
			}
		}
		protected InputBox Core { get; private set; }
		protected BackgroundImage Image { get; private set; }

		public TextBox(string name = null)
			: base(name)
		{
			Image = GameObject.AddComponent<BackgroundImage>();
			Image.sprite = Skin.textField.normal.background;
			Core = Add(new InputBox());
			Layout = Layout.Horizontal;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			Core.Dispose();
			Core = null;
			base.Dispose(true);
		}

		public string Text
		{
			get => Core.Input.text ?? "";
			set => Core.Input.text = value ?? "";
		}
		public Color TextColor
		{
			get => Core.Label.TextColor;
			set => Core.Label.TextColor = value;
		}
		public TextAnchor TextAlign
		{
			get => Core.Label.TextAlign;
			set => Core.Label.TextAlign = value;
		}
	}
}

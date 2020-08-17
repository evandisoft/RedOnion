using RedOnion.UI.Components;
using System;
using System.ComponentModel;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[Description("Line of editable text (or multi-line text editor).")]
	public class TextBox : Element
	{
		protected internal InputField Core { get; private set; }
		protected internal Label Label { get; private set; }
		protected BackgroundImage Image { get; private set; }

		[Description("Create new text box/field without a text.")]
		public TextBox()
		{
			Image = GameObject.AddComponent<BackgroundImage>();
			Image.sprite = Skin.textField.normal.background;
			Image.type = UUI.Image.Type.Sliced;
			Core = GameObject.AddComponent<InputField>();
			Core.TextBox = this;
			Label = Add(new Label()
			{
				Anchors = Anchors.Fill,
				TextColor = Skin.textField.normal.textColor,
				TextAlign = TextAnchor.UpperLeft
			});
			Core.textComponent = Label.Core;
		}
		[Description("Create new text box/field with specified text.")]
		public TextBox(string text) : this() => Text = text;

		protected override void Dispose(bool disposing)
		{
			if (!disposing || RootObject == null)
				return;
			Core = null;
			Label.Dispose();
			Label = null;
			Image = null;
			base.Dispose(true);
		}

		[Description("Allow multiple lines of text.")]
		public bool MultiLine
		{
			get => Core.multiLine;
			set => Core.lineType = value
				? UUI.InputField.LineType.MultiLineNewline
				: UUI.InputField.LineType.SingleLine;
		}
		[Description("The text in the box.")]
		public string Text
		{
			get => Core.text ?? "";
			set => Core.text = value ?? "";
		}
		[Description("Foreground color of the text.")]
		public Color TextColor
		{
			get => Label.TextColor;
			set => Label.TextColor = value;
		}
		[Description("How to align text within the box.")]
		public TextAnchor TextAlign
		{
			get => Label.TextAlign;
			set => Label.TextAlign = value;
		}
		[Description("Size of the font used for the text.")]
		public int FontSize
		{
			get => Label.FontSize;
			set => Label.FontSize = value;
		}
		[Description("Style of the font used for the text.")]
		public FontStyle FontStyle
		{
			get => Label.FontStyle;
			set => Label.FontStyle = value;
		}

		[Description("Executed when the box is entered (became active - with a cursor).")]
		public event Action<TextBox> Enter
		{
			add => Core.Selected += value;
			remove => Core.Selected -= value;
		}
		[Description("Executed when leaving the box (no longer having focus/cursor).")]
		public event Action<TextBox> Exit
		{
			add => Core.Deselected += value;
			remove => Core.Deselected -= value;
		}
		[Description("Executed when the text changes.")]
		public event Action<TextBox, string> Changed
		{
			add => Core.Changed += value;
			remove => Core.Changed -= value;
		}
		[Description("Executed when the text is submitted/confirmed. This is to be examined further, could be ENTER in single-line.")]
		public event Action<TextBox, string> Submitted
		{
			add => Core.Submitted += value;
			remove => Core.Submitted += value;
		}
		[Description("Position of the caret (index of the character).")]
		public int CaretPosition
		{
			get => Core.caretPosition;
			set => Core.caretPosition = value;
		}
		[Description("Position of the start of the selection.")]
		public int SelectionStart
		{
			get => Core.selectionAnchorPosition;
			set => Core.selectionAnchorPosition = value;
		}
		[Description("Position of the end of the selection.")]
		public int SelectionEnd
		{
			get => Core.selectionFocusPosition;
			set => Core.selectionFocusPosition = value;
		}
		[Description("Maximal number of characters that can be entered.")]
		public int CharacterLimit
		{
			get => Core.characterLimit;
			set => Core.characterLimit = value;
		}
		[Description("Box has focus.")]
		public bool Focused
		{
			get => Core.isFocused;
			set
			{
				if (value) Core.ActivateInputField();
				else Core.DeactivateInputField();
			}
		}
		[Description("Box cannot be edited.")]
		public bool ReadOnly
		{
			get => Core.readOnly;
			set => Core.readOnly = value;
		}
	}
}

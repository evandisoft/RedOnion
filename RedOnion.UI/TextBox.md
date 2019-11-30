## TextBox

**Base Class:** [Element](Element.md)

Line of editable text (or multi-line text editor).


**Constructors:**
- `TextBox()` - Create new text box/field without a text.
- `TextBox()`: text string
  - Create new text box/field with specified text.

**Instance Properties:**
- `MultiLine`: bool - Allow multiple lines of text.
- `Text`: string - The text in the box.
- `TextColor`: Color - Foreground color of the text.
- `TextAlign`: TextAnchor - How to align text within the box.
- `FontSize`: int - Size of the font used for the text.
- `FontStyle`: FontStyle - Style of the font used for the text.
- `CaretPosition`: int - Position of the caret (index of the character).
- `SelectionStart`: int - Position of the start of the selection.
- `SelectionEnd`: int - Position of the end of the selection.
- `CharacterLimit`: int - Maximal number of characters that can be entered.
- `Focused`: bool - Box has focus.
- `ReadOnly`: bool - Box cannot be edited.

**Instance Events:**
- `Enter`: Action\[TextBox\] - Executed when the box is entered (became active - with a cursor).
- `Exit`: Action\[TextBox\] - Executed when leaving the box (no longer having focus/cursor).
- `Changed`: Action\[TextBox, string\] - Executed when the text changes.
- `Submitted`: Action\[TextBox, string\] - Executed when the text is submitted/confirmed. This is to be examined further, could be ENTER in single-line.

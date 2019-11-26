## Panel

`UI.Panel` is the basis for more complex layout. You will usually nest few panels with alternating
`Layout.Horizontal` and `Layout.Vertical` (do not forget to assign its `Layout` property,
it is set to `Layout.None` when the panel is created).

- `Panel()` - Create new panel with layout set to `Layout.None`.
- `Panel()`, layout Layout
  - Create new panel with specified layout.
- `Parent`: [Element](Element.md) - Parent element (inside which this element is).
- `Tag`: Object - Tag for general usage.
- `Name`: string - Optional name of the element/control. Returns type name if not assigned (null).
- `Active`: bool - Element is set to be visible/active. [`GameObject.activeSelf`](https://docs.unity3d.com/ScriptReference/GameObject-activeSelf.html), [`GameObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
- `Visible`: bool - Element is visible (and all parents are). [`GameObject.activeInHierarchy`](https://docs.unity3d.com/ScriptReference/GameObject-activeInHierarchy.html), [`GameObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
- `Width`: float - Current width, redirects to PreferWidth when assigning.
- `Height`: float - Current height, redirects to PreferWidth when assigning.
- `MinWidth`: float - Minimal width if set to non-negative number (reads `float.NaN` otherwise, which means that the minimal width is not set - assigning negative number will have same result).
- `MinHeight`: float - Minimal height (same negative/`float.NaN` logic as above and for many below).
- `PreferWidth`: float - Preferred width if set (the layout will use this if possible).
- `PreferHeight`: float - Preferred height if set (the layout will use this if possible).
- `FlexWidth`: float - Flexible width if inside horizontal/vertical layout.
- `FlexHeight`: float - Flexible height if inside horizontal/vertical layout.
- `Color`: Color - Background color.
- `Layout`: Layout - Layout (how child elements are placed).
- `LayoutPadding`: LayoutPadding - The combined inner padding and spacing (6 floats in total, all set to `3f` by default).
- `ChildAnchors`: Anchors - This currently controls layout's `childAlignment` and
`childForceExpandWidth/Height`, but plan is to use custom `LayoutComponent`.
You can try `Anchors.Fill` (to make all inner elements fill their cell)
or `Anchors.MiddleLeft/MiddleCenter/UpperLeft...`
- `InnerPadding`: Padding - Inner padding (border left empty inside this panel).
- `InnerSpacing`: Vector2 - Inner spacing (between elements).
- `Padding`: float - `InnerPadding.All` - one number if all are the same, or NaN.
- `Spacing`: float - Spacing - one number if both are the same, or NaN.
- `[name string]`: [Element](Element.md) - Get contained element by name. (Direct children only.)
- `Count`: int - Get number of child elements.
- `Add()`: [Element](Element.md), element [Element](Element.md)
  - Add new element onto this panel.
- `Remove()`: [Element](Element.md), element [Element](Element.md)
  - Remove element from this panel.
- `AddPanel()`: Panel, layout Layout
  - Add new panel with specified layout.
- `AddHorizontal()`: Panel - Add new panel with horizontal layout.
- `AddVertical()`: Panel - Add new panel with vertical layout.
- `AddLabel()`: Label, text string
  - Add new label with specified text.
- `AddTextBox()`: TextBox, text string
  - Add new label with specified text.
- `AddButton()`: Button, text string
  - Add new button with specified text.
- `AddButton()`: Button, text string, click Action\[Button\]
  - Add new button with specified text and click-action.
- `AddToggle()`: Button, text string
  - Add new toggle-button with specified text.
- `AddToggle()`: Button, text string, click Action\[Button\]
  - Add new toggle-button with specified text and click-action.
- `AddExclusive()`: Button, text string
  - Add new exclusive toggle-button (radio button) with specified text.
- `AddExclusive()`: Button, text string, click Action\[Button\]
  - Add new exclusive toggle-button with specified text and click-action.
- `AddExclusive2()`: Button, text string
  - Add new toggle-button exclusive in parent of this panel with specified text.
- `AddExclusive2()`: Button, text string, click Action\[Button\]
  - Add new toggle-button exclusive in parent of this panel with specified text and click-action.
- `IndexOf()`: int, item [Element](Element.md)
  - Get index of child element (-1 if not found).
- `Contains()`: bool, item [Element](Element.md)
  - Test if element is contained in this panel.

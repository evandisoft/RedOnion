## Simple

**Base Class:** [Element](Element.md)

**Derived:** [Panel](Panel.md)

`UI.Simple` is the base class for [`UI.Panel`](Panel.md) without the child tracking and enumerating functionality.
It is meant mainly for internal usage, for implementation of other composite controls, not for direct user/script usage.


**Instance Properties:**
- `Color`: Color - Background color.
- `Layout`: [Layout](Layout.md) - Layout (how child elements are placed).
- `LayoutPadding`: [LayoutPadding](LayoutPadding.md) - The combined inner padding and spacing (6 floats in total, set to `0f, 3f, 0f, 0f, 3f, 0f` by default).
- `ChildAnchors`: [Anchors](Anchors.md) - This currently controls layout's `childAlignment` and
`childForceExpandWidth/Height`, but plan is to use custom `LayoutComponent`.
You can try `Anchors.Fill` (to make all inner elements fill their cell)
or `Anchors.MiddleLeft/MiddleCenter/UpperLeft...`
- `InnerPadding`: [Padding](Padding.md) - Inner padding (border left empty inside this panel).
- `InnerSpacing`: Vector2 - Inner spacing (between elements).
- `Padding`: float - `InnerPadding.All` - one number if all are the same, or NaN.
- `Spacing`: float - Spacing - one number if both are the same, or NaN.

**Instance Methods:**
- `Add()`: [Element](Element.md), element [Element](Element.md)
  - Add new element onto this panel.
- `Remove()`: [Element](Element.md), element [Element](Element.md)
  - Remove element from this panel.
- `AddPanel()`: [Panel](Panel.md), layout [Layout](Layout.md)
  - Add new panel with specified layout.
- `AddHorizontal()`: [Panel](Panel.md) - Add new panel with horizontal layout.
- `AddVertical()`: [Panel](Panel.md) - Add new panel with vertical layout.
- `AddLabel()`: [Label](Label.md), text string
  - Add new label with specified text.
- `AddTextBox()`: [TextBox](TextBox.md), text string
  - Add new label with specified text.
- `AddButton()`: [Button](Button.md), text string
  - Add new button with specified text.
- `AddButton()`: [Button](Button.md), text string, click Action\[[Button](Button.md)\]
  - Add new button with specified text and click-action.
- `AddToggle()`: [Button](Button.md), text string
  - Add new toggle-button with specified text.
- `AddToggle()`: [Button](Button.md), text string, click Action\[[Button](Button.md)\]
  - Add new toggle-button with specified text and click-action.
- `AddExclusive()`: [Button](Button.md), text string
  - Add new exclusive toggle-button (radio button) with specified text.
- `AddExclusive()`: [Button](Button.md), text string, click Action\[[Button](Button.md)\]
  - Add new exclusive toggle-button with specified text and click-action.
- `AddExclusive2()`: [Button](Button.md), text string
  - Add new toggle-button exclusive in parent of this panel with specified text.
- `AddExclusive2()`: [Button](Button.md), text string, click Action\[[Button](Button.md)\]
  - Add new toggle-button exclusive in parent of this panel with specified text and click-action.

## Button

**Base Class:** [Element](Element.md)

Clickable button (or toggle-button).


**Constructors:**
- `Button()` - Create new button without a text.
- `Button()`: text string
  - Create new button with specified text.
- `Button()`: text string, click Action\[Button\]
  - Create new button with specified text and click-action.

**Instance Properties:**
- `Enabled`: bool - Enabled/Disabled (for clicks, Unity: `interactable`).
- `Toggleable`: bool - Toggleable button (false by default).
- `Pressed`: bool - Button is pressed (mainly for toggleable).
- `Exclusive`: bool - Toggleable button is exclusive (other toggleable buttons get released when this one is pressed).
- `ExclusiveLevel`: int - Depth (parent-chain) of the exclusivity logic.
- `Text`: string - Text of the button.
- `IconTexture`: Texture - \[`Unsafe`\] Icon on the button.
- `IconSprite`: Sprite - \[`Unsafe`\] Sprite of the icon on the button.
- `LayoutPadding`: [LayoutPadding](LayoutPadding.md) - The combined inner padding and spacing (6 floats in total, set to `8f, 6f, 8f, 4f, 6f, 4f` by default).
- `InnerPadding`: [Padding](Padding.md) - Inner padding (border left empty inside this button).
- `InnerSpacing`: Vector2 - Inner spacing (between text and icon).
- `Padding`: float - `InnerPadding.All` - one number if all are the same, or NaN.
- `Spacing`: float - Spacing - one number if both are the same, or NaN.

**Instance Events:**
- `Click`: Action\[Button\] - Executed when clicked.

**Instance Methods:**
- `Press()`: void - Press the button.
- `Toggle()`: void - Toggle the button.
- `Press()`: void, action bool
  - Press the button optionally not executing the `Click` action.
- `Toggle()`: void, action bool
  - Toggle the button optionally not executing the `Click` action.

## LayoutPadding

Inner padding and spacing (6 values).


**Constructors:**
- `LayoutPadding()`: all float
  - Set all values to the one specified.
- `LayoutPadding()`: horizontal float, vertical float
  - Set `left = xgap = right = horizontal` and `top = ygap = bottom = vertical`.
- `LayoutPadding()`: left float, xgap float, right float, top float, ygap float, bottom float
  - Specify all the values.
- `LayoutPadding()`: padding [Padding](Padding.md), spacing Vector2
  - Combine `padding` (4 floats - around the content) and `spacing` (2D vector, between elements).

**Instance Fields:**
- `left`: float - Padding on the left side.
- `xgap`: float - Horizontal spacing between elements.
- `right`: float - Padding on the right side.
- `top`: float - Padding above the content.
- `ygap`: float - Vertical spacing between elements.
- `bottom`: float - Padding below the content.

**Instance Properties:**
- `All`: float - One value for all (if same or setting), `NaN` if not.
- `Horizontal`: float - Value of `left`, `xgap` and `right` if same, `NaN` if not.
- `Vertical`: float - value of `top`, `ygap` and `bottom` if same, `NaN` if not.
- `Padding`: [Padding](Padding.md) - The padding (`left, right, top, bottom`).
- `Spacing`: Vector2 - The spacing (`xgap, ygap`).

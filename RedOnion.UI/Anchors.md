## Anchors

Anchor definition for positioning UI element within a cell.


**Constructors:**
- `Anchors()`: left float, right float, top float, bottom float
  - Create anchors by specifying all four values.
- `Anchors()`: anchor TextAnchor
  - Create anchors from `TextAnchor`.

**Instance Fields:**
- `left`: float - Fraction (0..1) of container's width to anchor left side of the element to.
- `right`: float - Fraction (0..1) of container's width to anchor right side of the element to.
- `top`: float - Fraction (0..1) of container's height to anchor top side of the element to.
- `bottom`: float - Fraction (0..1) of container's height to anchor bottom side of the element to.

**Static Fields:**
- `TopLeft`: Anchors - Position the element in top-left corner.
- `TopRight`: Anchors - Position the element in top-right corner.
- `BottomLeft`: Anchors - Position the element in bottom-left corner.
- `BottomRight`: Anchors - Position the element in bottom-right corner.
- `Fill`: Anchors - Fill the entire cell.
- `FillLeft`: Anchors - Fill the left side of the cell (top-down, anchor to left, keep the width).
- `FillRight`: Anchors - Fill the right side of the cell (top-down, anchor to right, keep the width).
- `FillTop`: Anchors - Fill the top of the cell (left-right, anchor to top, keep the height).
- `FillBottom`: Anchors - Fill the bottom of the cell (left-right, anchor to bottom, keep the height).
- `Middle`: Anchors - Position the element in the middle/center of the cell.
- `MiddleLeft`: Anchors - Position the element in the middle of left side.
- `MiddleRight`: Anchors - Position the element in the middle of right side.
- `MiddleTop`: Anchors - Position the element in the middle of the top.
- `MiddleBottom`: Anchors - Position the element in the middle of the bottom.
- `Horizontal`: Anchors - Stretch the element horizontally and place it in the middle/center (vertically).
- `Vertical`: Anchors - Stretch the element vertically and place it in the middle/center (horizontally).
- `Invalid`: Anchors - Marker for invalid / unused / default anchors.
- `UpperLeft`: Anchors - `TopLeft`, name taken from `TextAnchor`.
- `UpperCenter`: Anchors - `MiddleTop`, name taken from `TextAnchor`.
- `UpperRight`: Anchors - `TopRight`, name taken from `TextAnchor`.
- `MiddleCenter`: Anchors - `Middle`, name taken from `TextAnchor`.
- `LowerLeft`: Anchors - `BottomLeft`, name taken from `TextAnchor`.
- `LowerCenter`: Anchors - `MiddleLeft`, name taken from `TextAnchor`.
- `LowerRight`: Anchors - `BottomRight`, name taken from `TextAnchor`.

**Instance Methods:**
- `ToTextAnchor()`: TextAnchor - Convert to `TextAnchor`.

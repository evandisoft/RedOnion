## \[`WIP`\] ScrollBox

**Base Class:** [Panel](Panel.md)

Scrollable panel


**Types:**
- `Scroll`: [ScrollBox.Scroll](ScrollBox.Scroll.md)

**Instance Properties:**
- `ViewPort`: GameObject - \[`Unsafe`\] Game object of the view port.
- `Horizontal`: [ScrollBox.Scroll](ScrollBox.Scroll.md) - Horizontally scrollable.
- `Vertical`: [ScrollBox.Scroll](ScrollBox.Scroll.md) - Vertically scrollable.
- `Elastic`: bool - Elastic drag (can be dragged outside of normal bounds). Setting this to `false` switches to clamped mode (if previously `true`).
- `Clamped`: bool - Clamped mode (cannot be dragged outside of bounds). Setting this to `false` switches to unrestricted mode (if previously `true`).

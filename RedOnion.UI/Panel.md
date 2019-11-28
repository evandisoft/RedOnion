## Panel

**Base Class:** [Simple](Simple.md)

`UI.Panel` is the basis for more complex layout. You will usually nest few panels with alternating
`Layout.Horizontal` and `Layout.Vertical` (do not forget to assign its `Layout` property,
it is set to `Layout.None` when the panel is created).


**Constructors:**
- `Panel()` - Create new panel with layout set to `Layout.None`.
- `Panel()`: layout [Layout](Layout.md)
  - Create new panel with specified layout.

**Instance Properties:**
- `[name string]`: [Element](Element.md) - Get contained element by name. (Direct children only.)
- `[index int]`: [Element](Element.md) - Get child element by index.
- `Count`: int - Get number of child elements.

**Instance Methods:**
- `IndexOf()`: int, item [Element](Element.md)
  - Get index of child element (-1 if not found).
- `Contains()`: bool, item [Element](Element.md)
  - Test if element is contained in this panel.

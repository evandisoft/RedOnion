## PartSet.1

Read-only list (or set). Enumerable (can be used in `foreach`).
Used e.g. for parts and all lists and sets you are not allowed to modify.

- `ship`: [Ship](../API/Ship.md) - Ship (vessel/vehicle) this list of parts belongs to.
- `count`: int - Number of elements in the list (or set).
- `[index]`: Part - Get element by index. Will throw exception if index is out of range.
- `indexOf()`: int, item Part
  - Get index of element. -1 if not found.
- `contains()`: bool, item Part
  - Test wether the list (or set) contains specified element.

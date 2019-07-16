## EngineSet

Read-only list (or set). Enumerable (can be used in `foreach`).
Used e.g. for parts and all lists and sets you are not allowed to modify.

- `Count`: Int32 - Number of elements in the list (or set).
- `[index]`: [Engine](Engine.md) - Get element by index. Will throw exception if index is out of range.
- `Contains()`: Boolean, item [Engine](Engine.md)
  - Test wether the list (or set) contains specified element.
- `IndexOf()`: Int32, item [Engine](Engine.md)
  - Get index of element. -1 if not found.

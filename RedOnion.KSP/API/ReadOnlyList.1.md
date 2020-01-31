## ReadOnlyList.1

**Derived:** [PartSet.1](../Parts/PartSet.1.md), [Stages](../Parts/Stages.md), [ResourceList](../Parts/ResourceList.md), [PropellantList](../Parts/PropellantList.md)

Read-only list (or set). Enumerable (can be used in `foreach`).
Used e.g. for parts and all lists and sets you are not allowed to modify.


**Instance Properties:**
- `version`: int - Version counter. Each update of the collection increments this. Can be used to detect changes mid-enumeration.
- `count`: int - Number of elements in the list (or set).
- `[index int]`: T - Get element by index. Will throw exception if index is out of range.

**Instance Methods:**
- `indexOf()`: int, item T
  - Get index of element. -1 if not found.
- `contains()`: bool, item T
  - Test wether the list (or set) contains specified element.

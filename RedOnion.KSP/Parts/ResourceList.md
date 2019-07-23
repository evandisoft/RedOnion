## ResourceList

List of resources (read-only). Can either belong to single part or to list/set of parts.

- `[index]`: [Resource](Resource.md) - Get resource by name. Returns null for non-existent resource.
- `count`: Int32 - Number of elements in the list (or set).
- `getAmountOf()`: Double, name String
  - Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.
- `getMaxAmountOf()`: Double, name String
  - Get maximum storable amount of resource by name. Returs zero for non-existent resources.
- `getPartCountOf()`: Int32, name String
  - Get number of parts that can store the named resource. Returns zero for non-existent resources.
- `indexOf()`: Int32, item [Resource](Resource.md)
  - Get index of element. -1 if not found.
- `contains()`: Boolean, item [Resource](Resource.md)
  - Test wether the list (or set) contains specified element.

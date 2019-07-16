## ResourceList

List of resources (read-only). Can either belong to single part or to list/set of parts.

- `[index]`: Resource - Get resource by name. Returns null for non-existent resource.
- `Count`: Int32 - Number of elements in the list (or set).
- `GetAmountOf()`: Double, name String
  - Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.
- `GetMaxAmountOf()`: Double, name String
  - Get maximum storable amount of resource by name. Returs zero for non-existent resources.
- `GetPartCountOf()`: Int32, name String
  - Get number of parts that can store the named resource. Returns zero for non-existent resources.
- `IndexOf()`: Int32, item Resource
  - Get index of element. -1 if not found.
- `Contains()`: Boolean, item Resource
  - Test wether the list (or set) contains specified element.

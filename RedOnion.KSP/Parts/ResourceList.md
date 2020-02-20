## ResourceList

**Base Class:** [ReadOnlyList](../API/ReadOnlyList.1.md)\[[Resource](Resource.md)\]

List of resources (read-only). Can either belong to single part or to list/set of parts.


**Instance Properties:**
- `[name string]`: [Resource](Resource.md) - Get resource by name. Returns null for non-existent resource.
- `[id [ResourceID](ResourceID.md)]`: [Resource](Resource.md) - Get resource by ID (hash of the name). Returns null for non-existent resource.

**Instance Methods:**
- `getAmountOf()`: double, name string
  - Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.
- `getMaxAmountOf()`: double, name string
  - Get maximum storable amount of resource by name. Returs zero for non-existent resources.
- `getPartCountOf()`: int, name string
  - Get number of parts that can store the named resource. Returns zero for non-existent resources.
- `getAmountOf()`: double, id [ResourceID](ResourceID.md)
  - Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.
- `getMaxAmountOf()`: double, id [ResourceID](ResourceID.md)
  - Get maximum storable amount of resource by name. Returs zero for non-existent resources.
- `getPartCountOf()`: int, id [ResourceID](ResourceID.md)
  - Get number of parts that can store the named resource. Returns zero for non-existent resources.
- `getAmountOf()`: double, names IEnumerable\[string\]
  - Get total amount of resources (in part or set/list) by list of names.
- `getAmountOf()`: double, ids IEnumerable\[[ResourceID](ResourceID.md)\]
  - Get total amount of resources (in part or set/list) by list of IDs.

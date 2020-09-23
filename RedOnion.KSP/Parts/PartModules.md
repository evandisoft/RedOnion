## \[`Unsafe`\] PartModules

List of modules of a part.


**Instance Properties:**
- `part`: [Part](PartBase.md) - The part this list belongs to.
- `count`: int - Number of modules.
- `[index int]`: PartModule - Get module at specified index.
- `[persistentId uint]`: PartModule - Get module by persistent ID.
- `[className string]`: PartModule - Get module by class name (first or null).
- `[type Type]`: PartModule - Get module of specified type (first or null, includes sub-classes).

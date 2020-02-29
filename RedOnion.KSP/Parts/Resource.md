## Resource

Resource like LiquidFuel etc.


**Instance Properties:**
- `list`: [ResourceList](ResourceList.md) - The list this resource is part of.
- `name`: string - Name of the resource (e.g. LiquidFuel).
- `id`: [ResourceID](ResourceID.md) - Identificator of the resource (for resource library; hash of the name).
- `partCount`: int - Number of parts (in parent list/set) able to contain this resource.
- `amount`: double - Current amount of the resource.
- `maxAmount`: double - Maximal amount of the resource that can be stored.

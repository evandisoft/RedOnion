## ShipPartSet

Read-only list (or set). Enumerable (can be used in `foreach`).
Used e.g. for parts and all lists and sets you are not allowed to modify.

- `ship`: [Ship](../API/Ship.md) - Ship (vessel/vehicle) this list of parts belongs to.
- `root`: [Part](PartBase.md) - Root part.
- `nextDecoupler`: [Decoupler](Decoupler.md) - One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)
- `nextDecouplerStage`: int - Stage number of the nearest decoupler or -1. (`NextDecoupler?.Stage ?? -1`)
- `decouplers`: ReadOnlyList`1 - List of all decouplers, separators, launch clamps and docks with staging enabled. (Docking ports without staging enabled not included.)
- `dockingports`: ReadOnlyList`1 - List of all docking ports (regardless of staging).
- `engines`: [EngineSet](EngineSet.md) - All engines (regardless of state).
- `sensors`: ReadOnlyList`1 - All sensors.
- `count`: int - Number of elements in the list (or set).
- `[index]`: [Part](PartBase.md) - Get element by index. Will throw exception if index is out of range.
- `indexOf()`: int, item [Part](PartBase.md)
  - Get index of element. -1 if not found.
- `contains()`: bool, item [Part](PartBase.md)
  - Test wether the list (or set) contains specified element.

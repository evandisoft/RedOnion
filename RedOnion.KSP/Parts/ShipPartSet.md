## ShipPartSet

Read-only list (or set). Enumerable (can be used in `foreach`).
Used e.g. for parts and all lists and sets you are not allowed to modify.

- `Ship`: [Ship](../API/Ship.md) - Ship (vessel/vehicle) this list of parts belongs to.
- `Root`: [Part](PartBase.md) - Root part.
- `NextDecoupler`: [Decoupler](Decoupler.md) - One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)
- `NextDecouplerStage`: Int32 - Stage number of the nearest decoupler or -1. (`NextDecoupler?.Stage ?? -1`)
- `Decouplers`: ReadOnlyList`1 - List of all decouplers, separators, launch clamps and docks with staging enabled. (Docking ports without staging enabled not included.)
- `DockingPorts`: ReadOnlyList`1 - List of all docking ports (regardless of staging).
- `Engines`: [EngineSet](EngineSet.md) - All engines (regardless of state).
- `Sensors`: ReadOnlyList`1 - All sensors.
- `Count`: Int32 - Number of elements in the list (or set).
- `[index]`: [Part](PartBase.md) - Get element by index. Will throw exception if index is out of range.
- `Contains()`: Boolean, item [Part](PartBase.md)
  - Test wether the list (or set) contains specified element.
- `IndexOf()`: Int32, item [Part](PartBase.md)
  - Get index of element. -1 if not found.

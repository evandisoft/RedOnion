## Ship

Active vessel

- `Native`: Vessel - Native `Vessel` for unrestricted access to KSP API. Same as `FlightGlobals.ActiveVessel` if accessed through global `ship`.
- `Parts`: ShipPartSet - All parts of this ship/vessel/vehicle.
- `Root`: [Part](../Parts/PartBase.md) - Root part (same as `Parts.Root`).
- `NextDecoupler`: [Decoupler](../Parts/Decoupler.md) - One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)
- `NextDecouplerStage`: Int32 - Stage number of the nearest decoupler or -1. (Same as `Parts.NextDecouplerStage`.)
- `Decouplers`: ReadOnlyList`1 - List of all decouplers, separators, launch clamps and docks with staging. (Docking ports without staging enabled not included.)
- `DockingPorts`: ReadOnlyList`1 - List of all docking ports (regardless of staging).
- `Engines`: ReadOnlyList`1 - All engines (regardless of state).
- `Sensors`: ReadOnlyList`1 - All sensors.

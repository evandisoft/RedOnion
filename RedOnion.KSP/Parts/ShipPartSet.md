## ShipPartSet

Collection of all the parts in one ship/vessel.


**Instance Properties:**
- `root`: [Part](PartBase.md) - Root part.
- `nextDecoupler`: [Decoupler](Decoupler.md) - One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)
- `nextDecouplerStage`: int - Stage number of the nearest decoupler or -1. (`NextDecoupler?.Stage ?? -1`)
- `decouplers`: [ReadOnlyList](../API/ReadOnlyList.1.md)\[[Decoupler](Decoupler.md)\] - List of all decouplers, separators, launch clamps and docks with staging enabled. (Docking ports without staging enabled not included.)
- `dockingports`: [ReadOnlyList](../API/ReadOnlyList.1.md)\[[DockingPort](DockingPort.md)\] - List of all docking ports (regardless of staging).
- `engines`: [EngineSet](EngineSet.md) - All engines (regardless of state).
- `sensors`: [ReadOnlyList](../API/ReadOnlyList.1.md)\[[Sensor](Sensor.md)\] - All sensors.

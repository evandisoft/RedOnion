## ShipPartSet

**Base Class:** [PartSet](PartSet.1.md)\[[Part](PartBase.md)\]

Collection of all the parts in one ship/vessel.


**Instance Properties:**
- `root`: [Part](PartBase.md) - Root part.
- `nextDecoupler`: [DecouplerBase](DecouplerBase.md) - One of the decouplers that will get activated by nearest stage.
- `nextDecouplerStage`: int - Stage number of the nearest decoupler or -1. (`nextDecoupler?.stage ?? -1`)
- `decouplers`: [ReadOnlyList](../API/ReadOnlyList.1.md)\[[DecouplerBase](DecouplerBase.md)\] - List of all decouplers, separators, launch clamps and docks with staging enabled. (Docking ports without staging enabled not included.)
- `dockingports`: [ReadOnlyList](../API/ReadOnlyList.1.md)\[[DockingPort](DockingPort.md)\] - List of all docking ports (regardless of staging).
- `engines`: [EngineSet](EngineSet.md) - All engines (regardless of state).
- `sensors`: [ReadOnlyList](../API/ReadOnlyList.1.md)\[[Sensor](Sensor.md)\] - All sensors.
- `stages`: [Stages](Stages.md) - \[`WIP`\] Parts per stage (by `decoupledin+1`).

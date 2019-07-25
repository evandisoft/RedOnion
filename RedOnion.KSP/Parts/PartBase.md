## Part

Part of the ship (vehicle/vessel).

- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `native`: Part - Native `Part` - KSP API.
- `parent`: Part - Parent part (this part is attached to).
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: Int32 - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: Int32 - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `istype()`: Boolean, name String
  - Method to test the type of the part (e.g. `.IsType("LaunchClamp")`)

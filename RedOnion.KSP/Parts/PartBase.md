## Part

Part of the ship (vehicle/vessel).

- `native`: Part - Native `Part` - KSP API.
- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `parent`: Part - Parent part (this part is attached to).
- `children`: [PartChildren](PartChildren.md) - Parts attached to this part.
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: int - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: int - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `name`: string - Name of the part (assigned by KSP).
- `title`: string - Title of the part (assigned by KSP).
- `istype()`: bool, name string
  - Method to test the type of the part (e.g. `.IsType("LaunchClamp")`)
- `explode()`: void - Explode the part.

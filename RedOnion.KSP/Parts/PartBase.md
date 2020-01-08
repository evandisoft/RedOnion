## Part

**Derived:** [Decoupler](Decoupler.md), [Engine](Engine.md), [Sensor](Sensor.md)

Part of the ship (vehicle/vessel).


**Instance Properties:**
- `native`: Part - \[`Unsafe`\] Native `Part` - KSP API.
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
- `position`: [Vector](../API/Vector.md) - Position of the part (relative to CoM of active ship/vessel).

**Instance Methods:**
- `istype()`: bool, name string
  - Method to test the type of the part (e.g. `.istype("LaunchClamp")`). Note that ROS has `is` operator and Lua has `isa` function that can be used togehter with `types.engine` etc.
- `explode()`: void - Explode the part.

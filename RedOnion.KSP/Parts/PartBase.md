## Part

**Derived:** [Engine](Engine.md), [Sensor](Sensor.md), [LinkPart](LinkPart.md)

Part of the ship (vehicle/vessel).


**Instance Properties:**
- `native`: Part - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_part.html)
- `type`: [PartType](PartType.md) - \[`WIP`\] Type of the part.
- `science`: [Science](Science.md) - Science available through this part, `null` if none.
- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `parent`: Part - Parent part (this part is attached to).
- `children`: [PartChildren](PartChildren.md) - Parts attached to this part.
- `values`: [PartValues](PartValues.md) - Custom values and tags attached to this part.
- `tags`: [PartValues](PartValues.md) - Custom values and tags attached to this part. (alias to `values`)
- `decoupler`: [LinkPart](LinkPart.md) - Decoupler that will decouple this part when staged.
- `stage`: int - Stage number as provided by KSP API. (`native.inverseStage` - activating stage for engines, decouplers etc.)
- `decoupledin`: int - Stage number where this part will be decoupled or -1. (`decoupler?.stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `name`: string - Name of the part (assigned by KSP).
- `title`: string - Title of the part (assigned by KSP).
- `position`: [Vector](../API/Vector.md) - Position of the part (relative to CoM of active ship/vessel).
- `mass`: double - Mass of the part including resources.
- `resourceMass`: double - Mass of the resources contained.

**Instance Methods:**
- `istype()`: bool, name string
  - Method to test the type of the part (e.g. `.istype("LaunchClamp")`). Note that ROS has `is` operator and Lua has `isa` function that can be used togehter with `types.engine` etc. Another classification is through `type` property.
- `explode()`: void - Explode the part.

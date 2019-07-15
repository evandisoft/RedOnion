## Part

Part of the ship (wehicle/vessel).

- `Ship`: [Ship](../API/Ship.md) - Ship (wehicle/vessel) this part belongs to.
- `Native`: Part - Native `Part` - KSP API.
- `Parent`: Part - Parent part (this part is attached to).
- `Decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `Stage`: Int32 - Stage number as provided by KSP API. (`Native.inverseStage`)
- `DecoupledIn`: Int32 - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `IsType()`: Boolean, name String
  - Method to test the type of the part (e.g. `.IsType("LaunchClamp")`)

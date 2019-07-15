## Decoupler

Decoupler, separator, launch clamp or docking port.

- `Ship`: [Ship](../API/Ship.md) - Ship (wehicle/vessel) this part belongs to.
- `Native`: Part - Native `Part` - KSP API.
- `Parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `Decoupler`: Decoupler - Decoupler that will decouple this part when staged.
- `Stage`: Int32 - Stage number as provided by KSP API. (`Native.inverseStage`)
- `DecoupledIn`: Int32 - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `IsType()`: Boolean, name String
  - Accepts `decoupler` and `separator`. (Case insensitive)

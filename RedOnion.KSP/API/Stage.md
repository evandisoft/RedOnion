## Stage

Used to activate next stage and/or get various information about stage(s). Returns true on success, if used as function. False if stage was not ready.

- `number`: int - Stage number.
- `ready`: bool - Whether ready for activating next stage or not.
- `parts`: PartSet`1 - Parts that will be separated by next decoupler
- `engines`: [EngineSet](../Parts/EngineSet.md) - Active engines (regardless of decouplers). `Engines.Resources` reflect total amounts of fuels inside boosters with fuel that cannot flow (like solid fuel).
- `xparts`: PartSet`1 - Active engines and all accessible tanks upto next decoupler. `xparts.resources` reflect total amounts of fuels accessible to active engines, but only in parts that will be separated by next decoupler. This includes liquid fuel and oxidizer and can be used for automated staging, Especially so called Asparagus and any design throwing off tanks (with or without engiens).
- `solidfuel`: double - Amount of solid fuel available in active engines. Shortcut to `engines.resources.getAmountOf("SolidFuel")`.
- `liquidfuel`: double - Amount of liquid fuel available in tanks of current stage to active engines. Shortcut to `xparts.resources.getAmountOf("LiquidFuel")`.
- `fuel`: double - Total amount of fuel avialable for active engines in current stage.
- `activate()`: bool - Activate next stage (can simply call stage() instead)
- `burnTime()`: double, deltaV double
  - Estimate burn time for given delta-v.

## Stage

Used to activate next stage and/or get various information about stage(s). Returns true on success, if used as function. False if stage was not ready.

- `number`: Int32 - Stage number.
- `ready`: Boolean - Whether ready for activating next stage or not.
- `parts`: PartSet`1 - Parts that will be separated by next decoupler
- `engines`: [EngineSet](../Parts/EngineSet.md) - Active engines (regardless of decouplers). `Engines.Resources` reflect total amounts of fuels inside boosters with fuel that cannot flow (like solid fuel).
- `crossparts`: PartSet`1 - Active engines and all accessible tanks upto next decoupler. `CrossParts.Resources` reflect total amounts of fuels accessible to active engines, but only in parts that will be separated by next decoupler. This includes liquid fuel and oxidizer and can be used for automated staging, Especially so called Asparagus and any design throwing off tanks (with or without engiens).
- `solidfuel`: Double - Amount of solid fuel available in active engines. Shortcut to `Engines.Resources.GetAmountOf("SolidFuel")`.
- `liquidfuel`: Double - Amount of liquid fuel available in tanks of current stage to active engines. Shortcut to `CrossParts.Resources.GetAmountOf("LiquidFuel")`.
- `fuel`: Double - Total amount of fuel avialable in active engines.
- `activate()`: Boolean - Activate next stage (can simply call stage() instead)

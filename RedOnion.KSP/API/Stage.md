## Stage

Used to activate next stage and/or get various information about stage(s). Returns true on success, if used as function. False if stage was not ready.


**Static Properties:**
- `number`: int - Stage number.
- `ready`: bool - Whether ready for activating next stage or not.
- `pending`: bool - True when current stage number is the same as number of stages (LED is flashing).
- `parts`: [PartSet](../Parts/PartSet.1.md)\[[Part](../Parts/PartBase.md)\] - Parts that will be separated by next decoupler
- `engines`: [EngineSet](../Parts/EngineSet.md) - Active engines (regardless of decouplers). `Engines.Resources` reflect total amounts of fuels inside boosters with fuel that cannot flow (like solid fuel).
- `xparts`: [PartSet](../Parts/PartSet.1.md)\[[Part](../Parts/PartBase.md)\] - All accessible tanks upto next decoupler that can contain propellants. `xparts.resources` reflect total amounts of fuels accessible to active engines, but only in parts that will be separated by next decoupler. This includes liquid fuel and oxidizer and can be used for automated staging, Especially so called Asparagus and any design throwing off tanks (with or without engines).
- `solidfuel`: double - Amount of solid fuel available in active engines. Similar to `engines.resources.getAmountOf("SolidFuel")` but ignores engines/boosters not separated in next stage. Useful when using central booster with smaller side-boosters (decoupled first). Stock-only version.
- `solidlike`: double - \[`WIP`\] Amount of solid-like fuel available in active engines. Similar to `engines.resources.getAmountOf(engines.propellants.namesOfSolid)` but ignores engines/boosters not separated in next stage. Useful when using central booster with smaller side-boosters (decoupled first). Universal version (should be compatible with mods - e.g. Karbonite).
- `liquidfuel`: double - Amount of liquid fuel available in tanks of current stage to active engines. Shortcut to `xparts.resources.getAmountOf("LiquidFuel")`. Stock-only version.
- `liquidlike`: double - \[`WIP`\] Amount of liquid fuel available in tanks of current stage to active engines. Similar to `xparts.resources.getAmountOf(engines.propellants.namesOfLiquid)` but also ignores flameout engines.
- `fuel`: double - \[`WIP`\] Total amount of fuel avialable for active engines in current stage. Designed with Ion and Monopropellant engines as well as mods in mind, use `solidfuel + liquidfuel` as fallback, if this does not work.
- `nofuel`: bool - \[`WIP`\] Indicator for staging - `fuel == 0.0`. Note that it could be better to use `fuel < 0.1` instead.

**Static Methods:**
- `activate()`: bool - Activate next stage (can call the stage object as a function as well (stage() instead of stage.activate())
- `burntime()`: [TimeDelta](TimeDelta.md), deltaV double
  - \[`WIP`\] Estimate burn time for given delta-v.

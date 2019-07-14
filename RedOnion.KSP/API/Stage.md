## Stage

Used to activate next stage and/or get various information about stage(s). Returns true on success, if used as function. False if stage was not ready.

- `activate()`: Boolean - Activate next stage (can simply call stage() instead)
- `number`: Int32 - Stage number.
- `ready`: Boolean - Whether ready for activating next stage or not.
- `parts`: PartSet`1 - Parts that belong to this stage, upto next decoupler
- `crossParts`: PartSet`1 - Active engines and all accessible tanks upto next decoupler
- `engines`: PartSet`1 - Active engines
- `solidFuel`: Double - Amount of solid fuel in active engines. Shortcut to Engines.Resources.GetAmountOf("SolidFuel")
- `liquidFuel`: Double - Amount of liquid fuel in tanks of current stage. Shortcut to CrossParts.Resources.GetAmountOf("LiquidFuel")

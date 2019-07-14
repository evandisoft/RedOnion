## Stage

Used to activate next stage and/or get various information about stage(s).
Returns true on success, if used as function. False if stage was not ready.

- `number`: int - Stage number.
- `ready`: bool - Whether ready for activating next stage or not.
- `parts`: PartSet - Parts that belong to this stage, upto next decoupler
- `crossParts`: PartSet - Active engines and all accessible tanks upto next decoupler
- `engines`: PartSet - List of active engines
- `solidFuel`: double - Amount of solid fuel in active engines
- `liquidFuel`: double - Amount of liquid fuel in tanks of current stage

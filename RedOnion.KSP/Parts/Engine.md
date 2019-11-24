## Engine

Engine of a ship (vehicle/vessel).

- `multiModule`: MultiModeEngine - (Unsafe) KSP API. Module of multi-mode engine, if present (null otherwise).
- `firstModule`: ModuleEngines - (Unsafe) KSP API. Module of first engine.
- `secondModule`: ModuleEngines - (Unsafe) KSP API. Module of second engine, if present (null otherwise).
- `firstIsActive`: bool - Running primary engine (or the only one).
- `secondIsActive`: bool - Running secondary engine.
- `activeModule`: ModuleEngines - (Unsafe) KSP API. Active engine module.
- `gimbalModule`: ModuleGimbal - (Unsafe) KSP API. Gimbal module, if present (null otherwise).
- `multiMode`: bool - Is multi-mode engine (or not).
- `hasGimbal`: bool - Has gimbal module.
- `thrust`: double - Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).
- `thrustPercentage`: double - Thrust limiter in percents.
- `native`: Part - (Unsafe) Native `Part` - KSP API.
- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `children`: [PartChildren](PartChildren.md) - Parts attached to this part.
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: int - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: int - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `name`: string - Name of the part (assigned by KSP).
- `title`: string - Title of the part (assigned by KSP).
- `istype()`: bool, name string
  - Accepts `engine`. (Case insensitive)
- `getIsp()`: double, atm double
  - Get specific impulse [kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure).
- `getThrust()`: double, atm double, throttle double
  - Get thrust [kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle). Ignores `thrustPercentage`.
- `explode()`: void - Explode the part.

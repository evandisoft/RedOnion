## Engine

Engine of a ship (vehicle/vessel).

- `multiModule`: MultiModeEngine - KSP API. Module of multi-mode engine, if present (null otherwise).
- `firstModule`: ModuleEngines - KSP API. Module of first engine.
- `secondModule`: ModuleEngines - KSP API. Module of second engine, if present (null otherwise).
- `firstIsActive`: bool - Running primary engine (or the only one).
- `secondIsActive`: bool - Running secondary engine.
- `activeModule`: ModuleEngines - KSP API. Active engine module.
- `gimbalModule`: ModuleGimbal - KSP API. Gimbal module, if present (null otherwise).
- `multiMode`: bool - Is multi-mode engine (or not).
- `hasGimbal`: bool - Has gimbal module.
- `thrust`: double - Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).
- `thrustPercentage`: double - Thrust limiter in percents.
- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `native`: Part - Native `Part` - KSP API.
- `parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: int - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: int - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `istype()`: bool, name string
  - Accepts `sensor`. (Case insensitive)
- `getIsp()`: double, atm double
  - Get specific impulse [kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure).
- `getThrust()`: double, atm double, throttle double
  - Get thrust [kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle). Ignores `thrustPercentage`.

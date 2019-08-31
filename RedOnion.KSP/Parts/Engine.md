## Engine

Engine of a ship (vehicle/vessel).

- `multiModule`: MultiModeEngine - KSP API. Module of multi-mode engine, if present (null otherwise).
- `firstModule`: ModuleEngines - KSP API. Module of first engine.
- `secondModule`: ModuleEngines - KSP API. Module of second engine, if present (null otherwise).
- `firstIsActive`: Boolean - Running primary engine (or the only one).
- `secondIsActive`: Boolean - Running secondary engine.
- `activeModule`: ModuleEngines - KSP API. Active engine module.
- `gimbalModule`: ModuleGimbal - KSP API. Gimbal module, if present (null otherwise).
- `multiMode`: Boolean - Is multi-mode engine (or not).
- `hasGimbal`: Boolean - Has gimbal module.
- `thrust`: Single - Current thrust (at current pressure, with current `thrustPercentage` and current throttle).
- `thrustPercentage`: Single - Thrust limiter in percents.
- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `native`: Part - Native `Part` - KSP API.
- `parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: Int32 - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: Int32 - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `istype()`: Boolean, name String
  - Accepts `sensor`. (Case insensitive)
- `getThrust()`: Single, atm Single, throttle Single
  - Get thrust at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle). Ignores `thrustPercentage`.

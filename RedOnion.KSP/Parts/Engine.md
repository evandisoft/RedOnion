## Engine

Engine of a ship (wehicle/vessel).

- `MultiModule`: MultiModeEngine - KSP API. Module of multi-mode engine, if present (null otherwise).
- `FirstModule`: ModuleEngines - KSP API. Module of first engine.
- `SecondModule`: ModuleEngines - KSP API. Module of second engine, if present (null otherwise).
- `FirstIsActive`: Boolean - Running primary engine (or the only one).
- `SecondIsActive`: Boolean - Running secondary engine.
- `ActiveModule`: ModuleEngines - KSP API. Active engine module.
- `GimbalModule`: ModuleGimbal - KSP API. Gimbal module, if present (null otherwise).
- `MultiMode`: Boolean - Is multi-mode engine (or not).
- `HasGimbal`: Boolean - Has gimbal module.
- `Thrust`: Single - Avalable thrust at current pressure, ignoring the limiter (`ThrustPercentage` and current throttle).
- `ThrustPercentage`: Single - Thrust limiter in percents.
- `Ship`: [Ship](../API/Ship.md) - Ship (wehicle/vessel) this part belongs to.
- `Native`: Part - Native `Part` - KSP API.
- `Parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `Decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `Stage`: Int32 - Stage number as provided by KSP API. (`Native.inverseStage`)
- `DecoupledIn`: Int32 - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `Resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `State`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `IsType()`: Boolean, name String
  - Accepts `sensor`. (Case insensitive)
- `GetThrust()`: Single, atm Single, throttle Single
  - Get thrust at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle). Ignores `ThrustPercentage`.

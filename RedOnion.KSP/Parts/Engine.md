## Engine

**Base Class:** [Part](PartBase.md)

Engine of a ship (vehicle/vessel).


**Instance Properties:**
- `multiModule`: MultiModeEngine - \[`Unsafe`\] KSP API. Module of multi-mode engine, if present (null otherwise).
- `firstModule`: ModuleEngines - \[`Unsafe`\] KSP API. Module of first engine.
- `secondModule`: ModuleEngines - \[`Unsafe`\] KSP API. Module of second engine, if present (null otherwise).
- `firstIsActive`: bool - Running primary engine (or the only one).
- `secondIsActive`: bool - Running secondary engine.
- `activeModule`: ModuleEngines - \[`Unsafe`\] KSP API. Active engine module.
- `gimbalModule`: ModuleGimbal - \[`Unsafe`\] KSP API. Gimbal module, if present (null otherwise).
- `multiMode`: bool - Is multi-mode engine (or not).
- `hasGimbal`: bool - Has gimbal module.
- `thrust`: double - Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).
- `thrustPercentage`: double - Thrust limiter in percents.

**Instance Methods:**
- `istype()`: bool, name string
  - Accepts `engine`. (Case insensitive)
- `getIsp()`: double, atm double
  - Get specific impulse [kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure).
- `getThrust()`: double, atm double, throttle double
  - Get thrust [kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle). Ignores `thrustPercentage`.

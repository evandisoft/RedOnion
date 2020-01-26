## EngineSet

**Base Class:** [PartSet](PartSet.1.md)\[[Engine](Engine.md)\]

Read-only set of engines.


**Instance Properties:**
- `propellants`: [PropellantList](PropellantList.md) - \[`WIP`\] Propellantes consumed by the engines.
- `anyOperational`: bool - Whether any engine in the set is operational.
- `allOperational`: bool - Whether all the engines in the set are operational.
- `anyFlameout`: bool - Wheter any engine in the set flamed out.
- `allFlameout`: bool - Wheter all engines in the set flamed out.
- `thrust`: double - Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).

**Instance Methods:**
- `getThrust()`: double, atm float, throttle float
  - Get thrust [kN] of all operational engines at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle).
- `getIsp()`: double, atm double
  - Get average specific impulse [kN] of operational engines at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure).
- `burnTime()`: [TimeDelta](../API/TimeDelta.md), deltaV double, mass double
  - Estimate burn time for given delta-v (assuming it can be done without staging).

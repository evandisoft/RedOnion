## EngineSet

Read-only set of engines.

- `anyOperational`: bool - Whether any engine in the set is operational.
- `allOperational`: bool - Whether all the engines in the set are operational.
- `anyFlameout`: bool - Wheter any engine in the set flamed out.
- `allFlameout`: bool - Wheter all engines in the set flamed out.
- `thrust`: double - Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).
- `ship`: [Ship](../API/Ship.md) - Ship (vessel/vehicle) this list of parts belongs to.
- `getThrust()`: double, atm float, throttle float
  - Get thrust [kN] of all operational engines at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle).
- `getIsp()`: double, atm double
  - Get average specific impulse [kN] of operational engines at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure).
- `burnTime()`: double, deltaV double
  - Estimate burn time for given delta-v (assuming it can be done without staging).
- `count`: int - Number of elements in the list (or set).
- `[index]`: [Engine](Engine.md) - Get element by index. Will throw exception if index is out of range.
- `indexOf()`: int, item [Engine](Engine.md)
  - Get index of element. -1 if not found.
- `contains()`: bool, item [Engine](Engine.md)
  - Test wether the list (or set) contains specified element.

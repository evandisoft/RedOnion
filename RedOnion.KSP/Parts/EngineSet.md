## EngineSet

Read-only set of engines.

- `anyOperational`: Boolean - Whether any engine in the set is operational.
- `allOperational`: Boolean - Whether all the engines in the set are operational.
- `anyFlameout`: Boolean - Wheter any engine in the set flamed out.
- `allFlameout`: Boolean - Wheter all engines in the set flamed out.
- `thrust`: Double - Current thrust (at current pressure, with current `thrustPercentage` and current throttle).
- `count`: Int32 - Number of elements in the list (or set).
- `[index]`: [Engine](Engine.md) - Get element by index. Will throw exception if index is out of range.
- `getThrust()`: Double, atm Single, throttle Single
  - Get thrust of all operational engines at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure) and throttle (default 1 = full throttle).
- `indexOf()`: Int32, item [Engine](Engine.md)
  - Get index of element. -1 if not found.
- `contains()`: Boolean, item [Engine](Engine.md)
  - Test wether the list (or set) contains specified element.

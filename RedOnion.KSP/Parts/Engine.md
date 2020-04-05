## Engine

**Base Class:** [Part](PartBase.md)

Engine of a ship (vehicle/vessel).


**Instance Properties:**
- `multiModule`: MultiModeEngine - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_multi_mode_engine.html). Module of multi-mode engine, if present (null otherwise).
- `firstModule`: ModuleEngines - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_module_engines.html). Module of first engine.
- `secondModule`: ModuleEngines - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_module_engines.html). Module of second engine, if present (null otherwise).
- `firstIsActive`: bool - Running primary engine (or the only one).
- `secondIsActive`: bool - Running secondary engine.
- `activeModule`: ModuleEngines - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_module_engines.html). Active engine module.
- `gimbalModule`: ModuleGimbal - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_module_gimbal.html). Gimbal module, if present (null otherwise).
- `multiMode`: bool - Is multi-mode engine (or not).
- `hasGimbal`: bool - Has gimbal module.
- `operational`: bool - Whether engine is operational (ignited and not flameout).
- `ignited`: bool - Wheter engine is ignited.
- `flameout`: bool - Wheter engine flamed out.
- `isp`: double - Current ISP (Specific impulse). \[seconds]
- `vacuumIsp`: double - Vacuum ISP. \[seconds]
- `seaLevelIsp`: double - Sea-level ISP. \[seconds]
- `thrust`: double - Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).
- `thrustPercentage`: double - Thrust limiter in percents.
- `propellants`: [PropellantList](PropellantList.md) - \[`WIP`\] List of propellants used by the engine (by currently active mode).
- `propellants1`: [PropellantList](PropellantList.md) - \[`WIP`\] List of propellants used by first mode.
- `propellants2`: [PropellantList](PropellantList.md) - \[`WIP`\] List of propellants used by second mode (null for single-mode engines).
- `booster`: bool - \[`WIP`\] Indicator that the engines is (probably) solid rocket booster (contains propellant that does not flow).

**Instance Methods:**
- `istype()`: bool, name string
  - Accepts `engine`. (Case insensitive)
- `activate()`: void - Activate the engine.
- `shutdown()`: void - Shutdown / deactivate the engine.
- `getIsp()`: double, atm double
  - Get specific impulse \[kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, default NaN = current pressure).
- `getThrust()`: double, atm double, throttle double
  - Get thrust \[kN] at atmospheric pressure (0 = vacuum, 1 = Kerbin sea-level pressure, default NaN = current pressure) and throttle (default 1 = full throttle). Ignores `thrustPercentage`.

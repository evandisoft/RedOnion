## SpaceBody

Celestial body. (`SpaceBody` selected not to conflict with KSP `CelestialBody`.)


**Types:**
- `Atmosphere`: [Atmosphere](SpaceBody.Atmosphere.md)

**Instance Properties:**
- `native`: CelestialBody - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_celestial_body.html)
- `name`: string - Name of the body.
- `body`: SpaceBody - Celestial body this body is orbiting.
- `position`: [Vector](Vector.md) - Position of the body (relative to active ship).
- `velocity`: [Vector](Vector.md) - Current orbital velocity (relative to parent body, zero for Sun/Kerbol).
- `bodies`: [ReadOnlyList](ReadOnlyList.1.md)\[SpaceBody\] - Orbiting celestial bodies.
- `radius`: double - Radius of the body [m].
- `mass`: double - Mass of the body [kg].
- `gravParameter`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `mu`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `atmosphere`: [Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body.
- `atm`: [Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body. (Alias to `atmosphere`)
- `orbit`: [OrbitInfo](OrbitInfo.md) - Orbit parameters. Null for Sun/Kerbol.
- `period`: [TimeDelta](TimeDelta.md) - Period of current orbit in seconds. Alias to `orbit.period`. `NaN/none` for Sun/Kerbol.
- `timeToAp`: [TimeDelta](TimeDelta.md) - Eta to apoapsis in seconds. Alias to `orbit.timeToAp`. `NaN/none` for Sun/Kerbol.
- `timeToPe`: [TimeDelta](TimeDelta.md) - Eta to periapsis in seconds. Alias to `orbit.timeToPe`. `NaN/none` for Sun/Kerbol.
- `timeAtAp`: [TimeStamp](TimeStamp.md) - Time at apoapsis. Alias to `orbit.timeAtAp`. `NaN/none` for Sun/Kerbol.
- `timeAtPe`: [TimeStamp](TimeStamp.md) - Time at periapsis. Alias to `orbit.timeAtPe`. `NaN/none` for Sun/Kerbol.
- `eccentricity`: double - Eccentricity of the orbit. \[0, +inf) `NaN` for Sun/Kerbol.
- `inclination`: double - Inclination of the orbit. `NaN` for Sun/Kerbol.
- `semiMajorAxis`: double - Semi-major axis of the orbit. `NaN` for Sun/Kerbol.
- `semiMinorAxis`: double - Semi-minor axis of the orbit. `NaN` for Sun/Kerbol.
- `apoapsis`: double - Height above ground of highest point of the orbit. `NaN` for Sun/Kerbol.
- `periapsis`: double - Height above ground of lowest point of the orbit. `NaN` for Sun/Kerbol.
- `apocenter`: double - Highest distance between center of orbited body and any point of the orbit. `NaN` for Sun/Kerbol.
- `pericenter`: double - Lowest distance between center of orbited body and any point of the orbit. `NaN` for Sun/Kerbol.
- `trueAnomaly`: double - Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis. `NaN` for Sun/Kerbol.
- `meanAnomaly`: double - Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit. `NaN` for Sun/Kerbol.
- `lan`: double - Longitude of ascending node. `NaN` for Sun/Kerbol.
- `argumentOfPeriapsis`: double - Argument of periapsis. Angle from ascending node to periapsis. `NaN` for Sun/Kerbol.
- `aop`: double - Argument of periapsis. Angle from ascending node to periapsis. `NaN` for Sun/Kerbol.

**Instance Methods:**
- `positionAt()`: [Vector](Vector.md), time [TimeStamp](TimeStamp.md)
  - Predicted position at specified time.
- `velocityAt()`: [Vector](Vector.md), time [TimeStamp](TimeStamp.md)
  - Predicted velocity at specified time.
- `timeAtTrueAnomaly()`: [TimeStamp](TimeStamp.md), trueAnomaly double
  - Get time at true anomaly (absolute time of angle from direction of periapsis). `NaN/none` for Sun/Kerbol.
- `timeToTrueAnomaly()`: [TimeDelta](TimeDelta.md), trueAnomaly double
  - Get time to true anomaly (relative time of angle from direction of periapsis). [0, period) `NaN/none` for Sun/Kerbol.

## SpaceBody

Celestial body. (`SpaceBody` selected not to conflict with KSP `CelestialBody`.)


**Types:**
- `Atmosphere`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md)

**Instance Properties:**
- `native`: CelestialBody - \[`Unsafe`\] KSP API. Native `CelestialBody`.
- `name`: string - Name of the body.
- `body`: SpaceBody - Celestial body this body is orbiting.
- `position`: [Vector](Vector.md) - Position of the body (relative to active ship).
- `velocity`: [Vector](Vector.md) - Current orbital velocity (relative to parent body, zero for Sun/Kerbol).
- `bodies`: [ReadOnlyList](ReadOnlyList.1.md)\[SpaceBody\] - Orbiting celestial bodies.
- `radius`: double - Radius of the body [m].
- `mass`: double - Mass of the body [kg].
- `gravParameter`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `mu`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `atmosphere`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body.
- `atm`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body. (Alias to `atmosphere`)
- `orbit`: [OrbitInfo](OrbitInfo.md) - \[`WIP`\] Orbit parameters. Null for Sun/Kerbol.
- `period`: [TimeDelta](TimeDelta.md) - Period of current orbit in seconds. Alias to `orbit.period`.
- `timeToAp`: [TimeDelta](TimeDelta.md) - Eta to apoapsis in seconds. Alias to `orbit.timeToAp`.
- `timeToPe`: [TimeDelta](TimeDelta.md) - Eta to periapsis in seconds. Alias to `orbit.timeToPe`.
- `timeAtAp`: [TimeStamp](TimeStamp.md) - Time at apoapsis. Alias to `orbit.timeAtAp`.
- `timeAtPe`: [TimeStamp](TimeStamp.md) - Time at periapsis. Alias to `orbit.timeAtPe`.
- `eccentricity`: double - Eccentricity of the orbit. \[0, +inf)
- `inclination`: double - Inclination of the orbit.
- `semiMajorAxis`: double - Semi-major axis of the orbit.
- `semiMinorAxis`: double - Semi-minor axis of the orbit.
- `apoapsis`: double - Height above ground of highest point of the orbit.
- `periapsis`: double - Height above ground of lowest point of the orbit.
- `apocenter`: double - Highest distance between center of orbited body and any point of the orbit.
- `pericenter`: double - Lowest distance between center of orbited body and any point of the orbit.
- `trueAnomaly`: double - Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis.
- `meanAnomaly`: double - Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.
- `lan`: double - Longitude of ascending node.
- `argumentOfPeriapsis`: double - Argument of periapsis. Angle from ascending node to periapsis.
- `aop`: double - Argument of periapsis. Angle from ascending node to periapsis.

**Instance Methods:**
- `positionAt()`: [Vector](Vector.md), time [TimeStamp](TimeStamp.md)
  - \[`WIP`\] Predicted position at specified time.
- `velocityAt()`: [Vector](Vector.md), time [TimeStamp](TimeStamp.md)
  - \[`WIP`\] Predicted velocity at specified time.
- `timeAtTrueAnomaly()`: [TimeStamp](TimeStamp.md), trueAnomaly double
  - \[`WIP`\] Get time at true anomaly (absolute time of angle from direction of periapsis).
- `timeToTrueAnomaly()`: [TimeDelta](TimeDelta.md), trueAnomaly double
  - \[`WIP`\] Get time to true anomaly (relative time of angle from direction of periapsis). [0, period)

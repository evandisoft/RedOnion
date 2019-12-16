## SpaceBody

Celestial body. (`SpaceBody` selected not to conflict with KSP `CelestialBody`.)


**Types:**
- `Atmosphere`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md)

**Instance Properties:**
- `native`: CelestialBody - \[`Unsafe`\] KSP API. Native `CelestialBody`.
- `name`: string - Name of the body.
- `body`: SpaceBody - Celestial body this body is orbiting.
- `position`: [Vector](Vector.md) - Position of the body (relative to active ship).
- `velocity`: [Vector](Vector.md) - Current orbital velocity.
- `bodies`: [ReadOnlyList](ReadOnlyList.1.md)\[SpaceBody\] - Orbiting celestial bodies.
- `radius`: double - Radius of the body [m].
- `mass`: double - Mass of the body [kg].
- `gravParameter`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `mu`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `atmosphere`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body.
- `atm`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body. (Alias to `atmosphere`)
- `orbit`: Orbit - \[`Unsafe`\] \[`WIP`\] KSP API. Orbit parameters. May get replaced by safe wrapper in the future.
- `eccentricity`: double - Eccentricity of the orbit.
- `semiMajorAxis`: double - Semi-major axis of the orbit.
- `semiMinorAxis`: double - Semi-minor axis of the orbit.
- `inclination`: double - Inclination of the orbit.
- `apoapsis`: double - Height above ground of highest point of the orbit.
- `periapsis`: double - Height above ground of lowest point of the orbit.
- `apocenter`: double - Highest distance between center of orbited body and any point of the orbit.
- `pericenter`: double - Lowest distance between center of orbited body and any point of the orbit.
- `timeToAp`: double - Eta to apoapsis in seconds.
- `timeToPe`: double - Eta to periapsis in seconds.
- `period`: double - Period of current orbit in seconds.
- `trueAnomaly`: double - Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis.
- `meanAnomaly`: double - Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.
- `lan`: double - Longitude of ascending node.
- `argumentOfPeriapsis`: double - Argument of periapsis.

**Instance Methods:**
- `positionAt()`: [Vector](Vector.md), time double
  - \[`WIP`\] Predicted position at specified time.
- `velocityAt()`: [Vector](Vector.md), time double
  - \[`WIP`\] Predicted velocity at specified time.
- `timeAtTrueAnomaly()`: double, trueAnomaly double
  - \[`WIP`\] Get time at true anomaly (absolute time).
- `timeToTrueAnomaly()`: double, trueAnomaly double
  - \[`WIP`\] Get time to true anomaly (relative time).

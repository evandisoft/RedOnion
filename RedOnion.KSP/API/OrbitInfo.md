## \[`WIP`\] OrbitInfo

Orbit/patch parameters.


**Instance Properties:**
- `native`: Orbit - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_orbit.html): Orbit parameters.
- `body`: [SpaceBody](SpaceBody.md) - Orbited body.
- `encounter`: bool - \[`WIP`\] This orbit-patch ends by encounter with another celestial body.
- `escape`: bool - \[`WIP`\] This orbit-patch ends by escaping the SOI.
- `next`: OrbitInfo - Next patch if there is some transition (null otherwise).
- `startTime`: [TimeStamp](TimeStamp.md) - Time of start of this patch, if it is continuation, `time.now` otherwise (can be one tick old - 0.02s in the past).
- `endTime`: [TimeStamp](TimeStamp.md) - Time of end of this patch, if there is transition. `endTime = startTime + period` for current orbit without a transition.
- `period`: [TimeDelta](TimeDelta.md) - Period of the orbit in seconds.
- `timeToAp`: [TimeDelta](TimeDelta.md) - Eta to apoapsis in seconds. `timeAtAp - time.now`
- `timeToPe`: [TimeDelta](TimeDelta.md) - Eta to periapsis in seconds. `timeAtPe - time.now`
- `timeAtAp`: [TimeStamp](TimeStamp.md) - Time at apoapsis. `timeToAp + time.now`
- `timeAtPe`: [TimeStamp](TimeStamp.md) - Time at periapsis. `timeToPe + time.now`
- `eccentricity`: double - Eccentricity of current orbit. \[0, +inf)
- `inclination`: double - Inclination of current orbit. \[0, 180)
- `semiMajorAxis`: double - Semi-major axis of current orbit.
- `semiMinorAxis`: double - Semi-minor axis of current orbit.
- `apoapsis`: double - Height above ground of highest point of current orbit. `apocenter - body.radius`
- `periapsis`: double - Height above ground of lowest point of current orbit. `pericenter - body.radius`
- `apocenter`: double - Highest distance between center of orbited body and any point of current orbit. `(1 + eccentricity) * semiMajorAxis`
- `pericenter`: double - Lowest distance between center of orbited body and any point of current orbit. `(1 - eccentricity) * semiMajorAxis`
- `trueAnomaly`: double - Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis.
- `meanAnomaly`: double - Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.
- `lan`: double - Longitude of ascending node.
- `argumentOfPeriapsis`: double - Argument of periapsis. Angle from ascending node to periapsis.
- `aop`: double - Argument of periapsis. Angle from ascending node to periapsis.

**Instance Methods:**
- `positionAt()`: [Vector](Vector.md), time [TimeStamp](TimeStamp.md)
  - Predicted position at specified time. Does not include the movement of celestial bodies. See [orbit.png](orbit.png).
- `velocityAt()`: [Vector](Vector.md), time [TimeStamp](TimeStamp.md)
  - Predicted velocity at specified time. Does not include the movement of celestial bodies.

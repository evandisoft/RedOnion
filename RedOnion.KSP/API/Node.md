## \[`WIP`\] Node

Maneuver node.


**Constructors:**
- `Node()`: time double, prograde double, normal double, radial double
  - Create new maneuver node for active ship, specifying time and optionally prograde, normal and radial delta-v (unspecified are zero).
- `Node()`: time double, deltav [Vector](Vector.md)
  - Create new maneuver node for active ship, specifying time and burn vector. See [`deltav` property](#deltav) for more details.

**Instance Properties:**
- `native`: ManeuverNode - \[`Unsafe`\] KSP API.
- `ship`: [Ship](Ship.md) - Ship the node belongs to.
- `time`: double - Planned time for the maneuver.
- `eta`: double - Seconds until the maneuver.
- `deltav`: [Vector](Vector.md) - Direction and amount of velocity change needed (aka burn-vector). Note that the vector is relative to the SOI where the node is (not where the ship currently is). That means that the vector will be relative to the current position of the Mun not relative to future position of the Mun (when the node is for example  at the periapsis = closest enounter with the Mun and is retrograde with the right amount for circularization, but the ship is currently still in Kerbin's SOI). Therefore [`ship.orbitAt(time).velocityAt(time)`](Ship.md#orbitAt) shall be used rather than [`ship.velocityAt(time)`](Ship.md#velocityAt) when planning nodes in different SOI (both work the same when in same SOI).
- `nativeDeltaV`: Vector3d - \[`Unsafe`\] KSP API. Setting it also calls `patchedConicSolver.UpdateFlightPlan()`.
- `prograde`: double - Amount of velocity change in prograde direction.
- `normal`: double - Amount of velocity change in normal direction.
- `radial`: double - Amount of velocity change in radial-out direction.

**Static Properties:**
- `next`: Node - Next maneuver node of active ship. Null if none or in wrong scene.

**Instance Methods:**
- `remove()`: void - Remove/delete the node.
- `delete()`: void - Remove/delete the node.

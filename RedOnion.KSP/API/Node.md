## \[`WIP`\] Node

Maneuver node.


**Constructors:**
- `Node()`: time double, prograde double, normal double, radial double
  - Create new maneuver node for active ship, specifying time.
- `Node()`: time double, deltav [Vector](Vector.md)
  - Create new maneuver node for active ship.

**Instance Properties:**
- `native`: ManeuverNode - \[`Unsafe`\] KSP API.
- `ship`: [Ship](Ship.md) - Ship the node belongs to.
- `time`: double - Planned time for the maneuver.
- `eta`: double - Seconds until the maneuver.
- `deltav`: [Vector](Vector.md) - Direction and amount of velocity change needed.
- `nativeDeltaV`: Vector3d - \[`Unsafe`\] KSP API. Setting it also calls `patchedConicSolver.UpdateFlightPlan()`.
- `prograde`: double - Amount of velocity change in prograde direction.
- `normal`: double - Amount of velocity change in normal direction.
- `radial`: double - Amount of velocity change in radial-out direction.

**Static Properties:**
- `next`: Node - Next maneuver node of active ship. Null if none or in wrong scene.

**Instance Methods:**
- `remove()`: void - Remove/delete the node.
- `delete()`: void - Remove/delete the node.

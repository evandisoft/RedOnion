## \[`WIP`\] Node

Maneuver node.


**Instance Properties:**
- `native`: ManeuverNode - \[`Unsafe`\] KSP API.
- `ship`: [Ship](Ship.md) - Ship the node belongs to.
- `time`: double - Planned time for the maneuver.
- `eta`: double - Seconds until the maneuver.
- `deltav`: [Vector](Vector.md) - Direction and amount of velocity change needed.
- `prograde`: double - Amount of velocity change in prograde direction.
- `normal`: double - Amount of velocity change in normal direction.
- `radial`: double - Amount of velocity change in radial-out direction.

**Static Properties:**
- `next`: Node - Next maneuver node of active ship. Null if none or in wrong scene.

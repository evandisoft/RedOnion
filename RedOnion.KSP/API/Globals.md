## Globals

Global variables, objects and functions common to all scripting languages.


**Types:**
- `autorun`: [AutoRun](AutoRun.md) - An api for setting which scripts will be ran when an engine is reset.
- `ui`: [UI](../Namespaces/UI.md) - User Interface.
- `ksp`: [KSP](../Namespaces/KSP.md) - \[`Unsafe`\] Shortcuts to KSP API + some tools.
- `unity`: [Unity](../Namespaces/Unity.md) - \[`Unsafe`\] Shortcuts to Unity API.
- `types`: [Types](../Namespaces/Types.md) - Types to be used with ROS: `is` operator; Lua: `isa` function.
- `stage`: [Stage](Stage.md) - Staging logic.
- `time`: [Time](Time.md) - Current time and related functions.
- `node`: [Node](Node.md) - Maneuver node.
- `player`: [Player](Player.md) - \[`WIP`\] User/player controls.
- `user`: [Player](Player.md) - \[`WIP`\] User/player controls.
- `PID`: [PID](PID.md) - PID regulator (alias to `system.pid` in ROS).
- `app`: [App](App.md) - \[`WIP`\] Safe API for KSP Application Launcher (toolbar/buttons). WIP

**Static Fields:**
- `native`: NamespaceInstance - \[`Unsafe`\] Namespace Mappings (import of native types by namespace). More info [here](../ReflectionUtil/NamespaceInstance.md)
- `assembly`: GetMappings - \[`Unsafe`\] Assembly Mappings (import of native types by assembly). More info [here](../ReflectionUtil/GetMappings.md)

**Static Properties:**
- `vector()`: [Vector](Vector.md) - Function for creating 3D vector / coordinate.
- `ship`: [Ship](Ship.md) - Active vessel (in flight only, null otherwise).
- `autopilot`: [Autopilot](Autopilot.md) - Autopilot for active vessel. (`null` if no ship)
- `bodies`: [Bodies](Bodies.md) - A collection of space/celestial bodies. (Safe API)
- `target`: Object - \[`WIP`\] Target of active ship. Null if none.
- `altitude`: double - Alias to `ship.altitude`. (`NaN` if no ship.)
- `apoapsis`: double - Alias to `ship.apoapsis`. (`NaN` if no ship.)
- `periapsis`: double - Alias to `ship.periapsis`. (`NaN` if no ship.)
- `body`: [SpaceBody](SpaceBody.md) - Orbited body (redirects to `ship.body`, `null` if no ship).
- `atmosphere`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of orbited body (redirects to `ship.body.atmosphere`, `atmosphere.none` if no ship).

## Globals

Global variables, objects and functions.

- `bodies`: [Bodies](Bodies.md) - A collection of space/celestial bodies. (Safe API)
- `reflect`: [Reflect](Reflect.md) - All the reflection stuff and namespaces.
- `vector()`: [Vector](Vector.md) - Function for creating 3D vector / coordinate.
- `ship`: [Ship](Ship.md) - Active vessel (in flight only, null otherwise).
- `stage()`: [Stage](Stage.md) - Staging logic.
- `autopilot`: [Autopilot](Autopilot.md) - Autopilot for active vessel. (`null` if no ship)
- `player`: Player - User/player controls.
- `user`: Player - User/player controls.
- `altitude`: double - Alias to `ship.altitude`. (`NaN` if no ship.)
- `apoapsis`: double - Alias to `ship.apoapsis`. (`NaN` if no ship.)
- `periapsis`: double - Alias to `ship.periapsis`. (`NaN` if no ship.)
- `body`: SpaceBody - Orbited body (redirects to `ship.body`, `null` if no ship).
- `atmosphere`: [Atmosphere](SpaceBody+Atmosphere.md) - Atmosphere parameters of orbited body (redirects to `ship.body.atmosphere`, `atmosphere.none` if no ship).
- `autorun`: [AutoRun](AutoRun.md) - An api for setting which scripts will be ran when an engine is reset.
- `app`: [App](App.md) - Safe API for KSP Application Launcher (toolbar/buttons).
- `native`: NamespaceInstance - Namespace Mappings
- `assembly`: GetMappings - Reflected/imported stuff by assembly name.
- `time`: [Time](Time.md) - Current time and related functions.
- `pid`: [PID](PID.md) - PID regulator (alias to `system.pid` in ROS).
- `pidloop`: [PID](PID.md) - PID regulator (alias to `pid`).
- `ui`: UI_Namespace - User Interface.
- `ksp`: [KSP](../Namespaces/KSP.md) - Shortcuts to (unsafe) KSP API + some tools.
- `unity`: Unity_Namespace - Shortcuts to (unsafe) Unity API.

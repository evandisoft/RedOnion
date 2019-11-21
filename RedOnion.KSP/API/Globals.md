## Globals

Global variables, objects and functions.

- `autorun`: [AutoRun](AutoRun.md) - An api for setting which scripts will be ran when an engine is reset.
- `ui`: UI_Namespace - User Interface.
- `ksp`: [KSP](../Namespaces/KSP.md) - Shortcuts to (unsafe) KSP API + some tools.
- `unity`: Unity_Namespace - Shortcuts to (unsafe) Unity API.
- `native`: NamespaceInstance - Namespace Mappings (import of native types by namespace).
- `assembly`: GetMappings - Assembly Mappings (import of native types by assembly.
- `time`: [Time](Time.md) - Current time and related functions.
- `stage`: Type - Staging logic.
- `PID`: [PID](PID.md) - PID regulator (alias to `system.pid` in ROS).
- `app`: [App](App.md) - Safe API for KSP Application Launcher (toolbar/buttons). WIP
- `vector()`: [Vector](Vector.md) - Function for creating 3D vector / coordinate.
- `ship`: [Ship](Ship.md) - Active vessel (in flight only, null otherwise).
- `autopilot`: [Autopilot](Autopilot.md) - Autopilot for active vessel. (`null` if no ship)
- `player`: Player - User/player controls.
- `user`: Player - User/player controls.
- `bodies`: [Bodies](Bodies.md) - A collection of space/celestial bodies. (Safe API)
- `altitude`: double - Alias to `ship.altitude`. (`NaN` if no ship.)
- `apoapsis`: double - Alias to `ship.apoapsis`. (`NaN` if no ship.)
- `periapsis`: double - Alias to `ship.periapsis`. (`NaN` if no ship.)
- `body`: SpaceBody - Orbited body (redirects to `ship.body`, `null` if no ship).
- `atmosphere`: [Atmosphere](SpaceBody+Atmosphere.md) - Atmosphere parameters of orbited body (redirects to `ship.body.atmosphere`, `atmosphere.none` if no ship).

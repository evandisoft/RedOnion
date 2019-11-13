## Globals

Global variables, objects and functions.

- `autorun`: [AutoRun](AutoRun.md) - An api for setting which scripts will be ran when an engine is reset.
- `bodies`: [Bodies](Bodies.md) - A collection of space/celestial bodies. (Safe API)
- `getbody`: [BodiesDictionary](../UnsafeAPI/BodiesDictionary.md) - A map of planet names to planet bodies. (Unsafe API)
- `reflect`: [Reflect](Reflect.md) - All the reflection stuff and namespaces. (Lua also has `import`.)
- `native`: [Reflect](Reflect.md) - Alias to `reflect` because of the namespaces.
- `vector()`: [Vector](Vector.md) - Function for creating 3D vector / coordinate.
- `V()`: [Vector](Vector.md) - Alias to Vector Function for creating 3D vector / coordinate.
- `time()`: [Time](Time.md) - Current time and related functions.
- `ship`: [Ship](Ship.md) - Active vessel (in flight only, null otherwise).
- `stage()`: [Stage](Stage.md) - Staging logic.
- `autopilot`: [Autopilot](Autopilot.md) - Autopilot for active vessel.
- `player`: Player - User/player controls.
- `user`: Player - User/player controls.
- `altitude`: double - Alias to `ship.altitude`
- `apoapsis`: double - Alias to `ship.apoapsis`.
- `periapsis`: double - Alias to `ship.periapsis`.
- `body`: SpaceBody - Orbited body (redirects to `ship.body`).
- `atmosphere`: Atmosphere - Atmosphere parameters of orbited body (redirects to `ship.body.atmosphere`).
- `app`: [App](App.md) - Safe API for KSP Application Launcher (toolbar/buttons).
- `assembly`: GetMappings - Reflected/imported stuff by assembly name.
- `pid`: [PID](PID.md) - PID regulator (alias to `system.pid` in ROS).
- `pidloop`: [PID](PID.md) - PID regulator (alias to `pid`).
- `ui`: UI_Namespace - User Interface.
- `ksp`: [KSP](../Namespaces/KSP.md) - Shortcuts to (unsafe) KSP API + some tools.
- `unity`: Unity_Namespace - Shortcuts to (unsafe) Unity API.
- `window`: [Window](../../RedOnion.UI/Window.md) - UI.Window
- `anchors`: Anchors - UI.Anchors
- `padding`: Padding - UI.Padding
- `layoutPadding`: LayoutPadding - UI.LayoutPadding
- `layout`: Layout - UI.Layout
- `panel`: Panel - UI.Panel
- `label`: Label - UI.Label
- `button`: Button - UI.Button
- `textBox`: TextBox - UI.TextBox

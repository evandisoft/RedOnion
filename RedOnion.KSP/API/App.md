## App

Safe API for KSP Application Launcher (toolbar/buttons). WIP

- `scenes`: AppScenes - Scenes in which to have the button.
- `center`: AppScenes - The button should be visible in Space Center.
- `flight`: AppScenes - The button should be visible in flight (does not include map view).
- `map`: AppScenes - The button should be visible in map view.
- `VAB`: AppScenes - The button should be visible in VAB.
- `SPH`: AppScenes - The button should be visible in SPH.
- `editor`: AppScenes - The button should be visible in VAB and SPH.
- `always`: AppScenes - The button should always be visible.
- `auto`: AppScenes - The button should only be visible in current scene (flight|map or VAB|SPH used if appropriate).
- `ready`: bool - Application launcher is ready for use.
- `defaultIcon`: Texture2D - Default Red Onion Icon.
- `add()`: [App.Button](App.Button.md), scenes AppScenes, iconPath string, onTrue Callback, onFalse Callback, onHover Callback, onHoverOut Callback, onEnable Callback, onDisable Callback
  - Add new app launcher button. Keep the returned object in a variable, the button would eventually be removed otherwise.
- `add()`: [App.Button](App.Button.md), onTrue Callback, onFalse Callback, onHover Callback, onHoverOut Callback, onEnable Callback, onDisable Callback, scenes AppScenes, texture Texture
  - Add new app launcher button. Keep the returned object in a variable, the button would eventually be removed otherwise.

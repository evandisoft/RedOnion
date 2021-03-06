# Planned Features
- New user-centric system for managing scripts. Will be a file-selection dialog or the completion area will do this.
- Editor/Repl reimplementation in new library. Hopefully allowing multiple editors/repls open at a time.
- Lots of new user interfaces for various features built on new gui library.
- Ingame testing framework.
- Ingame debugging.
- Provide editors that can be used for editing files in contexts unrelated to our scripting engines.
- More UI library features.

# Next Release
Just had a release (0.5.2)

# Current
# 0.5
## 0.5.2
### API
- fixed part tags (e.g. "noauto", required "noauto=" or "noauto x=y" before)
- reworked autopilot override + "pylink" for better 2D/3D control
- [Science API](https://evandisoft.github.io/RedOnion/RedOnion.KSP/API/Science), science.ros (supports DMagic's Orbital Science and DMModuleScienceAnimateGeneric)
- [OS/Process API](https://evandisoft.github.io/RedOnion/RedOnion.KSP/API/OperatingSystem) (early version of MunOS API for scripts)

### UI
- UI: ScollBox and Scrollbar (early versions, some improvements shall come soon)

### Lua
- Fixed lua completion bug for array literals.

### ROS
- Built-in functions and objects (added collections, documentation)
- Fixed break in (nested) foreach (for var e in list)

## 0.5.1
### New Dependency:
- ModuleManager required for part-values/tags to work

### API:
- user can now override/correct the autopilot (`autopilot.userFactor`)
- universal staging logic (mono, ion, LH2, ...), resources and propellants
- `OrbitInfo`, `orbitAt` and changes to `positionAt` and `velocityAt`
- `TimeStamp` and `TimeDelta`

### Scripts:
- launch.ros and control.ros: decouplers marked with `noauto` tag won't be auto-staged
- control.ros: auto-staging disabled until throttling or executing node
- control.ros: srf-retro landing assist and hohmann improved, can now circularize in different SOI
- lua tutorial scripts removed: This was done to remove clutter. You can copy in the tutorial code if you want the tutorials back.

### ROS Changes:
- fixed shadowing by loop variable (`var i; for var i...`)
- string comparision is case-insensitive, new identity operator (`===`) added for case-sensitive compare
- comparing anything to string first converts it to string (including enums)
- strings now have `.format`, `.substring`, `.equals`, `.compare` and `.contains` methods (case-sensitive)

## 0.5.0
### MunOS:
- Initial version of our system for running multiple scripts at a time, called [MunOS](https://evandisoft.github.io/RedOnion/MunOS/Multithreading).

### API:
- autopilot.disable now also resets killRot to false (was often locking roll in control.ros)
- api for maneuver nodes, sas, rcs and some other orbit-related properties

### Lua:
- Fixed bug where the `new` function was causing errors when it recieved no argument except the type to instantiate.

### Scripts:
- control.ros: maneuver executor, circularize, Hohmann planner, inclination match
- control.ros: srf-retro throttle limiter (below 10km) as landing helper (full throttle makes the ship hover)
- launch.ros: fixed the wobble at the end of circularization

# 0.4
## 0.4.8
### ROS Changes:
- ROS: fixed problem with variables with same name
- launch.ros: pitch-down logic for tilted/off-center engines (Dynawing)
- target and ship.target - works for bodies, ships and parts (docks)
- control.ros: targeting, rendezvous and docking assist

### LiveRepl Changes:
- Changed Kerbalui to use GetControlID to get a unique controlid for each window.
- Fixed issue where LiveRepl was making modifications to the fontsize of the default GUI.Label style instead of making those changes to a copy of that style.
- Hitting run will only save the file if it has been modified in the editor. Otherwise it loads the file from disk into the editor. This will allow you to more conveniently use an outside editor.
- Scene changes, including revert, now terminate any windows or vecdraws.

## 0.4.7
### General Changes:
- time.warp.ready improved
- added GameSettings and GameEvents into KSP namespace

### ROS Changes:
- launch.ros and control.ros now use try..finally
- fixed delay in repl for native events in ROS

### LiveRepl Changes:
- Fixed bug where LiveRepl cleared all Input ControlLocks instead of just the one it had set.
- Stopped using the ControlTypes.KEYBOARDINPUT lock. Now keyboard input (except camera control) is only locked to the window when the window is focused. Still have to lock ControlTypes.CAMERACONTROLS when mouse is in the window bounds, as otherwise scrolling the mouse zooms KSP in and out regardless of the mouse being over a window and that window being focused.

## 0.4.6
### RedOnionScript (ROS) Changes:
- [Try/Catch/Finally](https://evandisoft.github.io/RedOnion/RedOnion.ROS/Docs/Errors) for handling errors. Kerbalua has Lua's [pcall](https://www.lua.org/manual/5.2/manual.html#pdf-pcall).

### LiveRepl Changes:
- Fixed an issue where the global keybindings of LiveRepl could be activated when the input is not locked to the window. (input is locked when the mouse is inside the window bounds)
- Made LiveRepl use KSP's UI_SCALE setting. Hard for me to test this thoroughly as I don't have a monitor with a high enough resolution, but from what I can see it is working. I could add an additional scaling factor in some other setting later, if desired, to make it even larger. One thing I did not find how to scale was the scrollbar. But I was able to scale the part of the scrollbar that accepts mouse dragging.
- CompletionArea resets the scroll position each time the completion results list gets updated.
### Lua Changes:
- Lua [reflection api](https://evandisoft.github.io/RedOnion/RedOnion.KSP/MoonSharp/MoonSharpAPI/Reflection) was a bit confusing. I'm changing the terminology to `type` and `runtime type` instead of `static` and `type`. So what was once a `static` will now be called a `type`, and what was once a `type` will now be called a `runtime type`. (`runtime types` are for reflection whereas `types` are for accessing `static members` of clr classes or passing as the first argument to `new` to create a new object.)
- Removed `dofile`, `loadfile`, and `loadfilesafe` as they do not use the Scripts directory as the base directory and cannot be configured to do so. Versions of these (at least `dofile`) will be implemented in the future.

[require](https://www.lua.org/manual/5.2/manual.html#6.3) can be used because it allowed me to specify the base path. However, `require` will only run something the first time you call it on some `filepath`. To make it run that file again again you have to do `packages[filepath]=nil` first.

## 0.4.5 - 0.4.4
- My apologies about 0.4.4. I believe I dropped in an unchanged zip.
- Fixed a problem with Lua engine not resetting.
- Also the terminate button now will destroy ui's and vectors that were created by a script.

## 0.4.3
- Undo/Redo for LiveRepl Editor. Stores at least 50 of the last changes.
- Fixed bug where Lua was outputting strings twice
- Renamed KerbaluaMoonSharp assembly ([our modified version of MoonSharp](https://evandisoft.github.io/RedOnion/Kerbalua/MunSharp)) to MunSharp.
- Fixed MoonSharp issue where some classes could not be automatically registered because of having members that hide base class members without overriding them.
- Improved format for [CommonAPI](https://evandisoft.github.io/RedOnion/RedOnion.KSP/API/Globals) docs.
- Removed OtherDLLs folder and switched to using nuget for the MunSharp.dll, so we can potentially make more changes in the future.
- [Tutorial](https://evandisoft.github.io/RedOnion/Kerbalua/BasicParts) showing how to iterate over the parts and modules of a ship.
- Lua constructor improved. (Firda found an already implemented, but undocumented version in MoonSharp)

## 0.4.2
- Bug in ship.parts relating to explode functionality fixed.
- [Tutorial](https://evandisoft.github.io/RedOnion/Kerbalua/MajorMalfunctionNative) of how to fallback to native functionality.
- [Tutorial](https://evandisoft.github.io/RedOnion/Kerbalua/SelfDestruct) for selfDestruct.lua
- [Tutorial](https://evandisoft.github.io/RedOnion/Kerbalua/UIBasics) for using the ui in Lua code (uibasics.lua)
- More docs.
- Fixed a problem with `new(ui.Button,"buttontext",function() end)` where the `new` function  wasn't properly converting a lua function to the Action\<Button\> that the ui.Button constructor requires.
- Removed randomPartDestruct.ros as its code was outdated.
- Fixed bug with autoloading the last scriptname.

## 0.4.1
- Fixed bug where outputting a lot of things quickly (with something like `while true do print(1) end` takes more and more memory and is very slow.
- Fixed a bug where misformatted lua code would run anyway and just repeat an error message.
- Intellisense works for enums.
- Fixed a bug where completion on CommonAPI properties (like the globals `body`) wasn't working properly.

## 0.4.0
### General Changes:
- Updated for KSP 1.8.1
- Automatic documentation for our Common API
- In general much better documentation.
- Better Autopilot.

### LiveRepl Changes:
- Completing the Scriptname Input Area with a file name now loads the related file automatically
- LiveRepl can be dragged while executing a file.
- LiveRepl Overhaul: Much simpler UI code.
- Scriptname Input Area now uses a TextField instead of TextArea so that newlines are automatically disallowed.
- Tabs removed. They were kinda clunky. The new way the Scriptname Input Area works makes it less necessary.
- Clicking Scriptname Input Area now empties it so you can easily click on completion area to select the scriptname you want. And it automatically loads that script into the editor when you select it.
- Loading a file automatically selects the appropriate engine based on the extension
- Saving when  Scriptname Input Area is empty, now creates an "untitled.X" where X is the current engines proper extension.
- You can now set the font that will be used for the Repl, Output Area, and Editor. All other text uses the default font.

### ROS Changes
- ROS redesign. Allowing pause and continue features (something similar to the way the Lua example code uses coroutine.yield())

### Kerbalua Changes
- Removed Kerbalua specific global `import`. It is now available as the global `native` to be consistent with ROS.
- Removed auto conversions from function to CLR Actions and CLR Funcs. Reason is that the execution system doesn't allow script code sent into the CLR to yield in the normal way. This affects attempts to use functions like CLR's `List.Foreach`. Using foreach on a large collection is not interruptible. Any calls to CLR code are not interruptible so they must finish quickly, and in general `List.Foreach` will not.

[Old Versions (0.3.3 and older)](https://evandisoft.github.io/RedOnion/OldChangeLog)

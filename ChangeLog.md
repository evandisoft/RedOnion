# Planned Features
- New user-centric system for managing scripts. Will be a file-selection dialog or the completion area will do this.
- Editor/Repl reimplementation in new library. Hopefully allowing multiple editors/repls open at a time.
- Lots of new user interfaces for various features built on new gui library.
- Ingame testing framework.
- Ingame debugging.
- Provide editors that can be used for editing files in contexts unrelated to our scripting engines.
- More UI library features.

## 0.5.0:
- Working on some sort of system for running multiple scripts at a time. May not initially have a UI that can provide this feature to the user in full. That will probably come later.

# Next Release
### LiveRepl Changes:
- Fixed a mistake where LiveRepl was clearing all input locks instead of just the one it was using.
- Stopped using the ControlTypes.KEYBOARDINPUT lock. Now keyboard input is only locked when the window is focused, not determined by whether the mouse is in the window bounds. Still have to lock ControlTypes.CAMERACONTROLS for scroll events (when mouse is in the window boudns).

# Current
# 0.4

## 0.4.6
### RedOnionScript (ROS) Changes:
- [Try/Catch/Finally](RedOnion.ROS/Docs/Errors.md) for handling errors. Kerbalua has Lua's [pcall](https://www.lua.org/manual/5.2/manual.html#pdf-pcall).

### LiveRepl Changes:
- Fixed an issue where the global keybindings of LiveRepl could be activated when the input is not locked to the window. (input is locked when the mouse is inside the window bounds)
- Made LiveRepl use KSP's UI_SCALE setting. Hard for me to test this thoroughly as I don't have a monitor with a high enough resolution, but from what I can see it is working. I could add an additional scaling factor in some other setting later, if desired, to make it even larger. One thing I did not find how to scale was the scrollbar. But I was able to scale the part of the scrollbar that accepts mouse dragging.
- CompletionArea resets the scroll position each time the completion results list gets updated.
### Lua Changes:
- Lua [reflection api](RedOnion.KSP/MoonSharp/MoonSharpAPI/Reflection.md) was a bit confusing. I'm changing the terminology to `type` and `runtime type` instead of `static` and `type`. So what was once a `static` will now be called a `type`, and what was once a `type` will now be called a `runtime type`. (`runtime types` are for reflection whereas `types` are for accessing `static members` of clr classes or passing as the first argument to `new` to create a new object.)
- Removed `dofile`, `loadfile`, and `loadfilesafe` as they do not use the Scripts directory as the base directory and cannot be configured to do so. Versions of these (at least `dofile`) will be implemented in the future.

[require](https://www.lua.org/manual/5.2/manual.html#6.3) can be used because it allowed me to specify the base path. However, `require` will only run something the first time you call it on some `filepath`. To make it run that file again again you have to do `packages[filepath]=nil` first.

## 0.4.5 - 0.4.4
- My apologies about 0.4.4. I believe I dropped in an unchanged zip.
- Fixed a problem with Lua engine not resetting.
- Also the terminate button now will destroy ui's and vectors that were created by a script.

## 0.4.3
- Undo/Redo for LiveRepl Editor. Stores at least 50 of the last changes.
- Fixed bug where Lua was outputting strings twice
- Renamed KerbaluaMoonSharp assembly ([our modified version of MoonSharp](Kerbalua/MunSharp.md)) to MunSharp.
- Fixed MoonSharp issue where some classes could not be automatically registered because of having members that hide base class members without overriding them.
- Improved format for [CommonAPI](RedOnion.KSP/API/Globals.md) docs.
- Removed OtherDLLs folder and switched to using nuget for the MunSharp.dll, so we can potentially make more changes in the future.
- [Tutorial](Kerbalua/BasicParts.md) showing how to iterate over the parts and modules of a ship.
- Lua constructor improved. (Firda found an already implemented, but undocumented version in MoonSharp)

## 0.4.2
- Bug in ship.parts relating to explode functionality fixed.
- [Tutorial](Kerbalua/MajorMalfunctionNative.md) of how to fallback to native functionality.
- [Tutorial](Kerbalua/SelfDestruct.md) for selfDestruct.lua
- [Tutorial](Kerbalua/UIBasics.md) for using the ui in Lua code (uibasics.lua)
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

[Old Versions (0.3.3 and older)](OldChangeLog.md)
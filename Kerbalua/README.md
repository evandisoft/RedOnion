Our [Lua](https://www.lua.org/manual/5.2/) engine, **Kerbalua**, uses a modified version of [MoonSharp](http://www.moonsharp.org/) to create an in-game Lua scripting environment for KSP. 

Since MoonSharp is a lua implementation that can interact with CLR/C# objects, and since KSP is implemented in C#, our engine can interact with any aspect of the the [KSP Api](https://kerbalspaceprogram.com/api/annotated.html) ingame. We can provide, in in-game scripting, nearly any functionality a modmaker would normally have to access with C#.

## How to make and use modules:
It can be hard to do much without confusion if you are putting all your files in one folder. In the current way Kerbalua works, your files are in the Scripts directory. But you can put folders in there and add files in them if you want to make less clutter. Currently, there is no way to use LiveRepl to navigate to those folders, but you can put files in them which can be loaded as modules into scripts.

For more info on Lua Modules, see [this section of the Lua manual](https://www.lua.org/manual/5.2/manual.html#6.3) 

To edit those library files you can use an external file editor.

## Lua Missing Features:
If you really want any of these features, let me (Evan/evandisoft) know and I'll consider trying to implement a version that works with our project.

- In general what MoonSharp doesn't provide is not available here.
- **No `debug.sethook`** - Some debug functionality could be added in the future.
- **No implementation of coroutines** - This is due to using coroutines to automatically yield. I cannot expose MoonSharp's implementation of coroutines because it would interfere with my use of them. I believe I could implement a replacement implementation, but not for now.
- Removed `dofile`, `loadfile`, and `loadfilesafe` as they do not use the Scripts directory as the base directory and cannot be configured to do so. Versions of these (at least `dofile`) will be implemented in the future.

[require](https://www.lua.org/manual/5.2/manual.html#6.3) can be used, as I was able to specify the paths that are to be used to search for libs. `require` will only run something the first time you call it on some filepath. To make it run that file again again you have to do `packages[filepath]=nil`.

## API
In addition to the [Common API](../RedOnion.KSP/API/Globals.md), Kerbalua has some functionality specific to it: [Kerbalua API](../RedOnion.KSP/MoonSharp/MoonSharpAPI/MoonSharpGlobals.md).

For basic information about Lua there is lots of documentation available online. The version MoonSharp uses is 5.2.

Here are some tutorials specific to KSP functionality.

### Totorials/Guides
- [BasicParts](BasicParts.md) - A little script for iterating over the parts and modules of a ship.
- [MajorMalfunction](MajorMalfunction.md) - A script that causes 8 random parts to "malfunction".
- [MajorMalfunctionNative](MajorMalfunctionNative.md) - A version of MajorMalfunction that falls back to the native KSP API.
- [SelfDestruct](SelfDestruct.md) - A script that calls `explode` on all of the parts of the ship, starting from the parts furthest from the root.
- [UIBasics](UIBasics.md) - A script that adds a few buttons that print out the buttons text when clicked.
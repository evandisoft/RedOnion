Our [Lua](https://www.lua.org/manual/5.2/) engine, **Kerbalua**, uses a modified version of [MoonSharp](http://www.moonsharp.org/) to create an in-game Lua scripting environment for KSP. 

Since MoonSharp is a lua implementation that can interact with CLR/C# objects, and since KSP is implemented in C#, our engine can interact with any aspect of the the [KSP Api](https://kerbalspaceprogram.com/api/annotated.html) ingame. We can provide, in in-game scripting, nearly any functionality a modmaker would normally have to access with C#.

### Lua Missing Features:
If you really want any of these features, let me (Evan/evandisoft) know and I'll consider trying to implement a version that works with our project.

- In general what MoonSharp doesn't provide is not available here.
- **No `debug.sethook`** - Some debug functionality could be added in the future.
- **No implementation of coroutines** - This is due to using coroutines to automatically yield. I cannot expose MoonSharp's implementation of coroutines because it would interfere with my use of them. I believe I could implement a replacement implementation, but not for now.
- Removed `dofile`, `loadfile`, and `loadfilesafe` because they use base paths that are far outside the KSP install location, and I think that would be unexpected. `require`'s load path can be configured and I have configured it to start in the
Scripts directory. I might implement a version of `dofile` in the future that has a base path that is appropriate.

If you need to reload a lib that was loaded with `require("packagepath")`, you can do `packages["packagepath"]=nil` and then
`require("packagepath")` will execute it again rather than using a cached version of the resulting module.

To create libs you can create a folder in the Scripts directory. You can call it "lib", and then put scripts you want to use
as libraries in that folder. If you make a lib called "test.lua". You would load it with `require("lib/test")`. Any directory in the scripts folder won't show up in the file list in the completion area but files inside of them can be loaded with `require(dirname/filenameWithoutExtension)`. You use `require` without the `.lua` part of the filename.

### API
In addition to the [Common API](../RedOnion.KSP/Globals.md), Kerbalua has some functionality [specific to it](../RedOnion.KSP/MoonSharp/MoonSharpAPI/MoonSharpGlobals.md).

For basic information about Lua there is lots of documentation available online. The version MoonSharp uses is 5.2.

Here are some tutorials specific to KSP functionality.

### Totorials/Guides
- [BasicParts](BasicParts.md) - A little script for iterating over the parts and modules of a ship.
- [MajorMalfunction](MajorMalfunction.md) - A script that causes 8 random parts to "malfunction".
- [MajorMalfunctionNative](MajorMalfunctionNative.md) - A version of MajorMalfunction that falls back to the native KSP API.
- [SelfDestruct](SelfDestruct.md) - A script that calls `explode` on all of the parts of the ship, starting from the parts furthest from the root.
- [UIBasics](UIBasics.md) - A script that adds a few buttons that print out the buttons text when clicked.
Our [Lua](https://www.lua.org/manual/5.2/) engine, **Kerbalua**, uses a modified version of [MoonSharp](http://www.moonsharp.org/) to create an in-game Lua scripting environment for KSP. 

Since MoonSharp is a lua implementation that can interact with CLR/C# objects, and since KSP is implemented in C#, our engine can interact with any aspect of the the [KSP Api](https://kerbalspaceprogram.com/api/annotated.html) ingame. We can provide, in in-game scripting, nearly any functionality a modmaker would normally have to access with C#.

### Lua Missing Features:
- In general what MoonSharp doesn't provide is not available here.
- **No `debug.sethook`** - Some debug functionality could be added in the future.
- **No implementation of coroutines** - This is due to using coroutines to automatically yield. I cannot expose MoonSharp's implementation of coroutines because it would interfere with my use of them. I believe I could implement a replacement implementation, but not for now.

### API
In addition to the [Common API](../RedOnion.KSP/Globals.md), Kerbalua has some functionality [specific to it](../RedOnion.KSP/MoonSharp/MoonSharpAPI/MoonSharpGlobals.md).

### Totorials/Guides
- [BasicParts](BasicParts.md) - A little script for iterating over the parts and modules of a ship.
- [MajorMalfunction](MajorMalfunction.md) - A script that causes 8 random parts to "malfunction".
- [MajorMalfunctionNative](MajorMalfunctionNative.md) - A version of MajorMalfunction that falls back to the native KSP API.
- [SelfDestruct](SelfDestruct.md) - A script that calls `explode` on all of the parts of the ship, starting from the parts furthest from the root.
- [UIBasics](UIBasics.md) - A script that adds a few buttons that print out the buttons text when clicked.
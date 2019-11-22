Our [Lua engine](https://www.lua.org/manual/5.2/), **Kerbalua**, uses [MoonSharp](http://www.moonsharp.org/) to create an in-game Lua scripting environment for KSP. 

Since MoonSharp is a lua implementation that can interact with CLR/C# objects, and since KSP is implemented in C#, our engine can interact with any aspect of the the [KSP Api](https://kerbalspaceprogram.com/api/annotated.html) ingame. We can provide, in in-game scripting, nearly any functionality a modmaker would normally have to access with C#.

### Lua Missing Features:
- In general what MoonSharp doesn't provide is not available here.
- **No `debug.sethook`** - Some debug functionality could be added in the future.
- **No implementation of coroutines** - This is due to using coroutines to automatically yield. I cannot expose MoonSharp's implementation of coroutines because it would interfere with my use of them. I believe I could implement a replacement implementation, but not for now.

### API
In addition to the [Common API](../RedOnion.KSP/Globals.md), Kerbalua has some functionality [specific to it](../RedOnion.KSP/MoonSharp/MoonSharpAPI/MoonSharpGlobals.md).

Here is a [Tutorial](Tutorial.md) showing a working script in Lua that shows some of the features of our API.
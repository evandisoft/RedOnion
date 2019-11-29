All scripting is managed in our main user interface, called [LiveRepl](LiveRepl/Readme.md).

## Managing Scripts
Scripts are currently stored in GameData/RedOnion/Scripts. 

Scripts that we bundle as example scripts appear in a zipfile, GameData/RedOnion/Scripts.zip, and will appear in the list of scripts in-game, but any scripts in Scripts.zip will be hidden by any scripts you create of the same name. If you modified an example script and want to access the unmodified version, you can rename your modified version or delete it and the example script will show up again.

## Scripting Languages
We have two scripting languages, [**Kerbalua**](Kerbalua/README.md), and [**ROS** (Red Onion Script)](RedOnion.ROS/README.md). **Kerbalua** is a Lua implementation, while **ROS** is a custom language. **ROS** is designed to make programming easier by requiring very little syntax (examples: python-like indentation to mark program structure, and function calls/definitions without parenthesis).

Scripts are currently stored in GameData/RedOnion/Scripts,
our own scripts are packed inside GameData/RedOnion/Scripts.zip.
You can override our scripts simply by opening them in REPL
and saving the modified version (which will become a file outside of the zip).

## Limitations
- Calls to long running CLR code is not interruptible.
- You cannot safely pass a function to something like
`List.Foreach(fn)` because we cannot interrupt the `Foreach` call. So the entire iteration would have to occur in one KSP FixedUpdate and it would pause the game to complete. Our scripting languages have functionality for iterating over CLR collections which is interruptible, so you will have to use that instead. There's a convenient way to iterate over dotnet collections in both [Kerbalua](Kerbalua/BasicParts.md) and [ROS](RedOnion.ROS/README.md#statements)
- The CLR objects in the [KSP API](https://kerbalspaceprogram.com/api/annotated.html) that you might access using `ksp.` or `native.`, may be poorly documented. Modders had to expend a lot of effort to figure out how they work and how to avoid problems.

## Scripting Links

[Common API](RedOnion.KSP/API/Globals.md) - An API of useful functionality that is consistent between Lua and ROS. Entries marked with \[Unsafe\] lead to objects from KSP, Unity, or just general CLR objects that one should be careful with. Some objects in this API, like the global [ship](RedOnion.KSP/API/Ship.md), have been created to act as safe versions of native objects. In this case, ship is a safe version of KSP's [Vessel](https://kerbalspaceprogram.com/api/class_vessel.html). The corresponding native Vessel object can be accessed using ship's `native` field.

[Red Onion Script (ROS)](RedOnion.ROS/README.md) - A powerful in-game scripting engine taking inspiration from several popular languages (Ruby,Python,Javascript,etc)

[Kerbalua](Kerbalua/README.md) - A Lua scripting engine implemented using MoonSharp.

[Red Onion UI](RedOnion.UI/README.md) - A WIP UI Library, intended to be used by modders, users, and ourselves.
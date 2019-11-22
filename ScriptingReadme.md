## Scripting Languages
We have two scripting languages, [**Kerbalua**](Kerbalua/README.md), and [**ROS** (Red Onion Script)](RedOnion.ROS/README.md). **Kerbalua** is a Lua implementation, while **ROS** is a custom language. **ROS** is designed to make programming easier by requiring very little syntax (examples: python-like indentation to mark program structure, and function calls/definitions without parenthesis).

Scripts are currently stored in GameData/RedOnion/Scripts,
our own scripts are packed inside GameData/RedOnion/Scripts.zip.
You can override our scripts simply by opening them in REPL
and saving the modified version (which will become a file outside of the zip).

## Limitations
- Calls to long running clr code is not interruptible.
- You cannot safely pass a function to something like
`List.Foreach(fn)` because we cannot interrupt the `Foreach` call. So the entire iteration would have to occur in one KSP FixedUpdate and it would pause the game to complete. Our scripting languages have functionality for iterating over CLR collections which is interruptible, so you will have to use that instead.

## Scripting Links

[Common API](RedOnion.KSP/API/Globals.md) - An API of useful functionality that is consistent between Lua and ROS.

[Red Onion Script (ROS)](RedOnion.ROS/README.md) - A powerful in-game scripting engine taking inspiration from several popular languages (Ruby,Python,Javascript,etc)

[Kerbalua](Kerbalua/README.md) - A Lua scripting engine implemented using MoonSharp.

[Red Onion UI](RedOnion.UI/README.md) - A WIP UI Library, intended to be used by modders, users, and ourselves.
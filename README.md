# RedOnion and Kerbalua

A joint effort of Evan Dickinson and Lukáš Fireš to create
unrestricted scripted environment inside Kerbal Space Program
for all players and even modders wishing to control the game,
vessels, anything, with own script.
 
## Differences between ROS and LUA

[**LUA**](https://www.lua.org/)
[(MoonSharp)](https://github.com/xanathar/moonsharp)
is well known scripting language
and will offer more comfort and safety, while
[**ROS** (Red Onion Script)](RedOnion.Script/README.md)
aims to provide unrestricted (and potentionally dangerous)
access to every public method of KSP and Unity.

**ROS** was created anew and may contain bugs,
**LUA** may have some limitations
but should be much safer to use.

We have implemented a tentative [common API](CommonScriptApi.md) for both engines.

This API is subject to change. Even major changes. We are still in exploration mode.

Currently Scripts are stored in GameData/RedOnion/Scripts. These are the scripts we are messing with while trying out the game. If you make some scripts of your own, you will lose them when you update the game unless you save them somewhere else and copy them back in.

The Script folder situation is going to be changed dramatically in the future and we will make it easy for you to have your scripts saved somewhere that is not going to be overwritten.

## Contributing
We welcome people of all skill levels to contribute or give feedback, ask questions, etc.

Here is more information about [contributing](Contributing.md).
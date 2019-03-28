# RedOnion and Kerbalua

A joint effort of Evan Dickinson and Lukáš Fireš to create
unrestricted scripted environment inside Kerbal Space Program
for all players and even modders wishing to control the game,
vessels, anything, with own script.
 
## Differences between ROS and LUA

[**Lua**](https://github.com/evandisoft/RedOnion/blob/master/Kerbalua/README.md)
is well known scripting language
and will offer more comfort and safety, while
[**ROS** (Red Onion Script)](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.Script/README.md)
aims to provide unrestricted (and potentionally dangerous)
access to every public method of KSP and Unity.

**ROS** was created anew and may contain bugs,
**LUA** may have some limitations
but should be much safer to use.

We have implemented a tentative [common API](https://github.com/evandisoft/RedOnion/blob/master/CommonScriptApi.md) for both engines.

Note:
WE ARE PROVIDING ACCESS TO POWERFUL KSP FEATURES:
MANY WILL LEAD TO ERRORS IN ONE OR MORE OF THE SCRIPTING ENGINES.

This is a WIP, and as time goes on we will develop a more robust system.

This API is subject to change. Even major changes. We are still in exploration mode.

Currently Scripts are stored in GameData/RedOnion/Scripts. These are the scripts we are messing with while trying out the game (Use them at your own risk). If you make some scripts of your own, you will lose them when you update this mod unless you save them somewhere else and copy them back in.

The Script folder situation is going to be changed dramatically in the future and we will make it easy for you to have your scripts saved somewhere that is not going to be overwritten.

## Contributing
We welcome people of all skill levels to contribute or give feedback, ask questions, etc.

Here is more information about [contributing](https://github.com/evandisoft/RedOnion/blob/master/Contributing.md).

## Documentation

[RedOnionScript (ROS)](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.Script/README.md) - A powerful in-game scripting engine taking inspiration from several popular languages (Ruby,Python,Javascript,etc)

[Lua Engine](https://github.com/evandisoft/RedOnion/blob/master/Kerbalua/README.md) - A Lua scripting engine.

[RedOnionUI](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.UI/README.md) - A WIP UI Library, intended to be used by modders, users, and ourselves.

[CommonScriptApi](https://github.com/evandisoft/RedOnion/blob/master/CommonScriptApi.md) - Our tentative, WIP API shared by Lua and ROS for interacting with the game in various ways. May be changed dramatically in the future.

## Videos
Demonstration videos for this mod are on [this channel](https://www.youtube.com/channel/UChduoYTVOtAH0NA-Lj8EiKA).

## Discussion
Feedback can be left on our forum [thread](https://forum.kerbalspaceprogram.com/index.php?/topic/183050-wip-redonion-020-unrestricted-in-game-scripting-has-repl-editor-and-intellisense-lua-and-a-custom-jsruby-like-language-implemented-tested-on-ksp-161/), on this repository as a new issue, or as a comment on any of our videos.

## Releases
Releases are hosted at [spacedock](https://spacedock.info/mod/2116/Red%20Onion) and [curseforge](https://kerbal.curseforge.com/projects/redonion).

# RedOnion and Kerbalua

A joint effort of Evan Dickinson and Lukáš Fireš to create
unrestricted scripted environment inside Kerbal Space Program
for all players and even modders wishing to control the game,
vessels, anything, with own script.

**Warning: Do not use scripts from untrusted sources!**
This is not a sandbox, any script has the power to do almost anything.
We plan to eventually implement a switch to limit the power
(disable what is now marked `[Unsafe]` in the code),
but our goal now is to:

- Do whatever users wish to do to the game environment to have fun.
- Help modders develop and debug their mods using this mod.
- Help anybody to explore KSP API, their own or other's mods.

**We may, in the future, create an optional sandbox. But that will come later.**

 
## Differences between ROS and LUA

[**Lua**](https://github.com/evandisoft/RedOnion/blob/master/Kerbalua/README.md)
is well known scripting language
and may offer more comfort and safety, while
[**ROS** (Red Onion Script)](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.Script/README.md)
aims to make coding easier.
**ROS** was created anew and may contain bugs,
**LUA** may lack some features but should be more stable.

Scripts are currently stored in GameData/RedOnion/Scripts,
our own scripts are packed inside GameData/RedOnion/Scripts.zip.
You can override our scripts simply by opening them in REPL
and saving the modified version (which will become a file outside of the zip).

## Documentation

[Common Globals](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.KSP/API/Globals.md) - Objects and functions accessible by both LUA and ROS.

[Common Script Api](https://github.com/evandisoft/RedOnion/blob/master/CommonScriptApi.md) - Previous documentation of the common features (deprecated).

[Red Onion Script (ROS)](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.ROS/README.md) - A powerful in-game scripting engine taking inspiration from several popular languages (Ruby,Python,Javascript,etc)

[Lua Engine](https://github.com/evandisoft/RedOnion/blob/master/Kerbalua/README.md) - A Lua scripting engine.

[Red Onion UI](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.UI/README.md) - A WIP UI Library, intended to be used by modders, users, and ourselves.

[Troubleshooting](https://github.com/evandisoft/RedOnion/blob/master/TroubleShooting.md) - Some possible issues and solutions

## Videos
Demonstration videos for this mod are on [this channel](https://www.youtube.com/channel/UChduoYTVOtAH0NA-Lj8EiKA).

## Releases
Releases are hosted at [spacedock](https://spacedock.info/mod/2116/Red%20Onion) and [curseforge](https://kerbal.curseforge.com/projects/redonion).

## Upcoming Features

[Next Release](https://github.com/evandisoft/RedOnion/blob/master/ChangeLog.md#next-release)

[Planned](https://github.com/evandisoft/RedOnion/blob/master/ChangeLog.md#planned-features)

## Feedback
Feedback can be left on our forum [thread](https://forum.kerbalspaceprogram.com/index.php?/topic/183050-wip-redonion-020-unrestricted-in-game-scripting-has-repl-editor-and-intellisense-lua-and-a-custom-jsruby-like-language-implemented-tested-on-ksp-161/), on this repository as a new issue, or as a comment on any of our videos.

## Contributing

We welcome people of all skill levels to contribute or give feedback, ask questions, etc.

Here is more information about [contributing](https://github.com/evandisoft/RedOnion/blob/master/Contributing.md).
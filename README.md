# RedOnion and Kerbalua

A joint effort of Evan Dickinson and Lukáš Fireš to create
unrestricted scripted environment inside Kerbal Space Program
for all players and even modders wishing to control the game,
vessels, anything, with own script.

**Warning: Do not use scripts from untrusted sources!**
This is not a sandbox, any script has the power to do almost anything.
We plan to eventually implement a switch to limit the power
(disable what is marked `[Unsafe]` in the code),
but our goal now is to:

- Allow users to do whatever they wish to do to the game environment to have fun.
- Help modders develop and debug their mods using this mod.
- Help anybody to explore KSP API, their own or other's mods
  (read the license of each mod, we expose only `public` members).

 
## Differences between ROS and Lua

[**Lua**](Kerbalua/README.md)
is well known scripting language
and may offer more comfort and safety, while
[**ROS** (Red Onion Script)](RedOnion.ROS/README.md)
aims to make coding easier.
**ROS** was created anew and may contain bugs,
**Lua** may lack some features but should be more stable.

Scripts are currently stored in GameData/RedOnion/Scripts,
our own scripts are packed inside GameData/RedOnion/Scripts.zip.
You can override our scripts simply by opening them in REPL
and saving the modified version (which will become a file outside of the zip).

## Documentation

[Common Globals](RedOnion.KSP/API/Globals.md) - Objects and functions accessible by both Lua and ROS.

[Red Onion Script (ROS)](RedOnion.ROS/README.md) - A powerful in-game scripting engine taking inspiration from several popular languages (Ruby,Python,Javascript,etc)

[Lua Engine](Kerbalua/README.md) - A Lua scripting engine.

[Red Onion UI](RedOnion.UI/README.md) - A WIP UI Library, intended to be used by modders, users, and ourselves.

[Troubleshooting](TroubleShooting.md) - Some possible issues and solutions

**Outdated:**

[Common Script Api](CommonScriptApi.md) - Previous documentation of the common features. Should soon become part of [Common Globals: KSP](RedOnion.KSP/API/Globals.md)

## Videos

Demonstration videos for this mod are on [this channel](https://www.youtube.com/channel/UChduoYTVOtAH0NA-Lj8EiKA).

## Releases

Releases are hosted at [spacedock](https://spacedock.info/mod/2116/Red%20Onion) and [curseforge](https://kerbal.curseforge.com/projects/redonion).

## Upcoming Features

[Next Release](ChangeLog.md#next-release)

[Planned](ChangeLog.md#planned-features)

## Feedback

Feedback can be left on our forum [thread](https://forum.kerbalspaceprogram.com/index.php?/topic/183050-wip-redonion-020-unrestricted-in-game-scripting-has-repl-editor-and-intellisense-lua-and-a-custom-jsruby-like-language-implemented-tested-on-ksp-161/), on this repository as a new issue, or as a comment on any of our videos.

## Contributing

We welcome people of all skill levels to contribute or give feedback, ask questions, etc.

Here is more information about [contributing](Contributing.md).

[![Dynawing-Launch.jpg](https://i.postimg.cc/CMskNBBk/Dynawing-Launch.jpg)](https://postimg.cc/HjjrWLtk)

# RedOnion and Kerbalua

A joint effort of Evan Dickinson and Lukáš Fireš to create
unrestricted scripted environment inside Kerbal Space Program
for all players and even modders wishing to control the game,
vessels, anything, with own script.

## Features:
1. Repl/Editor with intellisense.
1. Scripting engines that can interact with any loaded CLR object
1. Lua scripting language called Kerbalua built on MoonSharp.
1. Powerful custom language/engine (RedOnionScript) built by Lukáš Fireš.
1. API's for doing things more easily or more safely, including Autopilot API, UI api, and more.

## Goals:
1. Provide ability to produce mods in Kerbalua/RedOnionScript and modify them without restarting the game.
1. Provide ability to debug mod code/scripts without restarting the game.
1. Provide ability to write user scripts, like automatic control of your ship.
1. Provide ability to interact with live game objects in a repl.
1. Provide powerful general editing capabilities inside KSP
1. Provide powerful API for modmakers/Users

License: [MIT](https://github.com/evandisoft/RedOnion/LICENSE)

Github: [here](https://github.com/evandisoft/RedOnion/.)

**Warning: Do not use scripts from untrusted sources!**
This is not a sandbox, any script has the power to do almost anything a modder could do in C#.
We plan to eventually implement a switch to limit the power
(disable what is marked `[Unsafe]` in the code),
but our goal now is to:

- Allow users to do whatever they wish to do to the game environment to have fun.
- Help modders develop and debug their mods using this mod.
- Help anybody to explore KSP API, their own or other's mods
  (read the license of each mod, we expose only `public` members directly).

## Releases

Your scripts are in GameData/RedOnion/Scripts. **I'm not sure whether CKAN deletes and replaces the entire RedOnion directory. Save your scripts somewhere else before updating to a new version, to be safe**. Then copy back in the ones you want to use. (If the example programs don't run, check out that section in [TroubleShooting](https://github.com/evandisoft/RedOnion/TroubleShooting.md))

Latest Release (0.4.0), available on:
- [CKAN](https://github.com/KSP-CKAN/CKAN)
- [Spacedock](https://spacedock.info/mod/2116/Red%20Onion)
- [CurseForge](https://www.curseforge.com/kerbal/ksp-mods/redonion)

## Documentation

- [Scripting](https://github.com/evandisoft/RedOnion/ScriptingReadme.md) - Documentation related to scripting.

- [LiveRepl](https://github.com/evandisoft/RedOnion/LiveRepl/Readme.md) - The main user interface for the project. Where scripts can be loaded, written, and executed.

- [Troubleshooting](https://github.com/evandisoft/RedOnion/TroubleShooting.md) - Some possible issues and solutions.

- [Development](https://github.com/evandisoft/RedOnion/DevelopmentReadme.md) - Main page for development documentation (project structure, implementation explanations, etc).

## Videos are listed [here](https://github.com/evandisoft/RedOnion/Videos.md)

## Upcoming Features

- [Next Release](https://github.com/evandisoft/RedOnion/ChangeLog.md#next-release)

- [Planned](https://github.com/evandisoft/RedOnion/ChangeLog.md#planned-features)

## Feedback

All feedback is appreciated. Feedback can be left on our forum [thread](https://forum.kerbalspaceprogram.com/index.php?/topic/183050-wip-redonion-020-unrestricted-in-game-scripting-has-repl-editor-and-intellisense-lua-and-a-custom-jsruby-like-language-implemented-tested-on-ksp-161/), as a new [issue](https://github.com/evandisoft/RedOnion/issues), or as a comment on any of our [videos](https://github.com/evandisoft/RedOnion/Videos.md).

## Contributing

We welcome people of all skill levels to contribute or give feedback, ask questions, etc.

Here is more information about [contributing](https://github.com/evandisoft/RedOnion/Contributing.md).
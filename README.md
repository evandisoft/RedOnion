[![Dynawing-Launch.jpg](https://i.postimg.cc/CMskNBBk/Dynawing-Launch.jpg)](https://postimg.cc/HjjrWLtk)

# RedOnion and Kerbalua

A joint effort of Evan Dickinson and Lukáš Fireš to create
unrestricted scripted environment inside Kerbal Space Program
for all players and even modders wishing to control the game,
vessels, anything, with own script.

## Current Features:
1. Repl/Editor with intellisense.
1. Scripting engines that can interact with Classes from any loaded CLR assembly. 
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
1. Help document the KSP API for modmakers.
1. Help people learn to program in a fun way.

License: [MIT](https://github.com/evandisoft/RedOnion/blob/master/LICENSE)

Github: [here](https://github.com/evandisoft/RedOnion)

[Github Pages](https://evandisoft.github.io/RedOnion) is where the most up-to-date information is.

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

**Your scripts are in GameData/RedOnion/Scripts. If you delete this folder to update to a new version, it will destroy your scripts.** I believe that CKAN does not delete them when uninstalling the mod or updating it, but you might want to make a copy of the folder just in case. We provide example scripts in a zip file, which will appear in the list of scripts in-game but wont be in the Scripts folder. If the example programs don't run, check out that section in [TroubleShooting](TroubleShooting.md)

Latest Releases are available at:
- [CKAN](https://github.com/KSP-CKAN/CKAN)
- [Spacedock](https://spacedock.info/mod/2116/Red%20Onion)
- [CurseForge](https://www.curseforge.com/kerbal/ksp-mods/redonion) ([Note about using Twitch Client "Install" button](TwitchClientInstallsNote.md))

ChangeLog: [here](ChangeLog.md#current)

## Documentation

- [Scripting](ScriptingReadme.md) - Documentation related to scripting.

- [LiveRepl](LiveRepl/Readme.md) - The main user interface for the project. Where scripts can be loaded, written, and executed.

- [Troubleshooting](TroubleShooting.md) - Some possible issues and solutions.

- [Development](DevelopmentReadme.md) - Main page for development documentation (project structure, implementation explanations, etc).

## Videos are listed [here](Videos.md)

## Upcoming Features

- [Next Release](ChangeLog.md#next-release)

- [Planned](ChangeLog.md#planned-features)

## Feedback

All feedback is appreciated. Feedback can be left on our forum [thread](https://forum.kerbalspaceprogram.com/index.php?/topic/189983-18x-redonion-unrestricted-in-game-scripting-v-040/), as a new [issue](https://github.com/evandisoft/RedOnion/issues), or as a comment on any of our [videos](Videos.md).

## Contributing

We welcome people of all skill levels to contribute or give feedback, ask questions, etc.

Here is more information about [contributing](https://github.com/evandisoft/RedOnion/blob/gh-pages/Contributing.md).

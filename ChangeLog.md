# Planned Features
- New user-centric system for managing scripts. Will be a file-selection dialog or the completion area will do this.
- Better autopilot (locking to a heading instead of a raw direction). Make Autopilot work with control surfaces.
- Automatic documentation
- ROS redesign

# Next Release
- Import System: `List=Import.System.Collections.Generic.List`. Using the Import system you can interact with any loaded
libraries (included loaded mods) written in C#. You should check out the licenses of those libraries prior to writing any code that depends on them. However, many mods have very permissive licenses.
- Less bad autopilot. Works sometimes ok with control surfaces.
- Lua no longer requires or allows "=" at start to return a value to the repl. Will automatically return the value of a lone expression entered at the repl.
- Repl/Editor saves it's position, Repl visibility status, and Editor visibility status.
- Lua has a function for constructing types into instances. Called new().

# 0.2:
## 0.2.1:
- Misc Changes

## 0.2.0:
### General:
- Improved error handling and reporting (line number and line content of where error/exception occured)

### ROS:
- Fixed some engine and parser bugs (functions, lambdas and loops)
- Enhanced reflection, especially for IEnumerable and List (new for var e in list)

### Documentation:
- Added [CommonScriptApi.md](https://github.com/evandisoft/RedOnion/blob/master/CommonScriptApi.md): Documents some of the API for interacting with the game that is available to both Lua and ROS.
- Updated [README.md](README.md)
- Updated Docs for [ROS](https://github.com/evandisoft/RedOnion/blob/master/RedOnion.Script/README.md) and [UI lib](RedOnion.UI/README.md)

### Lua:
- Wrapper table class called, ProxyTable, created. Can be used to act in place of classes that MoonSharp cannot properly reflect.

### Scripts:
- [majorMalfunction.lua](https://github.com/evandisoft/RedOnion/blob/master/GameData/RedOnion/Scripts/majorMalfunction.lua): A fun script that causes random parts to explode.
- [selfDestruct.lua](https://github.com/evandisoft/RedOnion/blob/master/GameData/RedOnion/Scripts/selfDestruct.lua): A script that explodes all the parts starting from the parts furthest from the root.
- [testFlightGui.ros](https://github.com/evandisoft/RedOnion/blob/master/GameData/RedOnion/Scripts/testFlightGui.ros): A script that creates a simple UI for interacting with the autopilot.

### Videos:
- Demo of majorMalfunction.lua and selfDestruct.lua: https://www.youtube.com/watch?v=xzAghlB2NLw
- ROS, Autopilot and UI using testFlightGui.ros: https://www.youtube.com/watch?v=CDBNb6jR_Cc 

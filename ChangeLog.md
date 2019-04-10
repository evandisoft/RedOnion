# Planned Features
- New user-centric system for managing scripts. Will be a file-selection dialog or the completion area will do this.
- Better autopilot (locking to a heading instead of a raw direction). Make Autopilot work with control surfaces.
- Automatic documentation
- ROS redesign

# Next Release
- Import System: `List=Import.System.Collections.Generic.List`. Using the Import system you can interact with any loaded
libraries (included loaded mods) written in C#. You should check out the licenses of those mods/libraries prior to writing any code that depends on them. However, many mods have very permissive licenses. This feature organizes all types in the namespace they are found in C#.

Currently, import system only works for Lua, but in ROS you can do:
```
var list=reflect.new("System.Collections.ArrayList")
```
It's possible that ROS will have the Import system implemented soon.

- For Lua, All of the C# classes that were in the CommonScriptAPI will only be available through the Import system. Most are in the default namespace "". Example:
```
editor=Import.EditorLogic.fetch
ship=editor.ship
partloader=Import.PartLoader.Instance
```
KSP has a system for getting the instance of a class that sometimes involves "fetch" and sometimes uses "Instance". You have to check which one works with a given class and use that.

- Less bad autopilot. Works to some extent with control surfaces. Also can set a relative direction consisting of a heading
and pitch. This is a lot easier of a way to specify the direction you want it to go in. It is relative to the closest body.
Heading is degrees from north (clockwise), and pitch is degrees above the plane perpendicular to the vector connecting the vessel and the body.
- Lua no longer requires or allows "=" at start to return a value to the repl. Will automatically return the value of a lone expression entered at the repl. Note:  this only works for single expressions.
```
i> alist[0] -- works
```
```
i> alist=new(List)
alist[0] -- doesn't work
```
- Repl/Editor saves it's position, Repl visibility status, and Editor visibility status.
- Lua now has a function for constructing types into instances. Called new(). Can construct an instance of a class using a type imported using the new Import system.
```
i> List=Import.System.Collections.Generic.List
r> void
i> alist=new(List)
r> void
i> alist.Add(1)
r> void
i> alist[0]
r> 1
```

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

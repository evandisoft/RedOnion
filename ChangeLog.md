# Planned Features
<details><summary>Show/Hide</summary>
  
- New user-centric system for managing scripts. Will be a file-selection dialog or the completion area will do this.
- Automatic documentation.
- ROS redesign. Allowing pause and continue features (something similar to the way the Lua example code uses coroutine.yield())
- Better Autopilot. Needs to handle all torque possibilities, work better with control surfaces, and probably use PIDs. I'd also like a better relative direction system that does not go wonky near the poles for polar orbits. Example: You're approaching north pole, you've specified your ship to have a heading of due north, then suddenly you reach the north pole and now instantly your ship is headed south. I'm sure there's a better way to do this.
- Editor/Repl reimplementation in new library. Hopefully allowing multiple editors/repls open at a time.
- Ability to run multiple scripts at a time and a gui to manage them.
- Lots of new user interfaces for various features built on new gui library.
- Ingame testing framework.
- Ingame debugging.
- Provide editors that can be used for editing files in contexts unrelated to our scripting engines.
- More UI library features.
</details>

# Next Release
<details><summary>Show/Hide</summary>

Nothing yet! Just had a release!

</details>

# 0.3 
## 0.3.0
### New Import System:
`listtype=import.system.collections.generic.list`. Using the import system you can interact with any loaded
libraries (included loaded mods) written in C#. You should check out the licenses of those mods/libraries prior to writing any code that depends on them. However, many mods have very permissive licenses. This feature organizes all types in the namespace they are found in C#.

For import namespace/type accesses are case insensitive. This is to provide compatibility with ROS's general case insensitive
nature. In Lua, the returned type will still have its member accesss be case sensitive, but the type and namespace name in import is going to be case insensitive. so `listtype=import.System.Collections.Generic.List` works the same as `listtype=import.system.collections.generic.list`.

Import currently takes generic types like `List<T>` and sets their type parameters to `typeof<object>`. So `import.system.collections.generic.list` returns the Type `List<object>`. This may change in the future, or be implemented differently in ROS and Lua.

For ROS, import is named "reflect" so
```
var alist=new reflect.system.collections.generic.list
alist.add 1
alist[0]
```

For Lua, All of the C# classes that were in the CommonScriptAPI will only be available through the Import system. Most are in the default namespace "". Example:
```
editor=import.EditorLogic.fetch
ship=editor.ship
partloader=import.PartLoader.Instance
```
KSP has a system for getting the instance of a class that sometimes involves "fetch" and sometimes uses "Instance". You have to check which one works with a given class and use that.

### Less Bad Autopilot.
Now works to some extent with control surfaces. 

Also can set a relative direction consisting of a heading and pitch. Heading is degrees from north (clockwise), and pitch is degrees above the plane perpendicular to the vector connecting the vessel and the body. This is an easier way to specify the direction you want it to point. It is relative to the closest body. 

```
ctrl=RedOnion.KSP.Autopilot.FlightControl
ctrl.SetRel(90,20) -- Aims ship east with a 20 degree pitch above "horizon" (by horizon I mean the plane perpendicular to the vector connecting the vessel and the body rather than the point at which the sky meets the land.)
```

### Better Lua Repl Interaction
Lua no longer requires or allows "=" at start to return a value to the repl. 
Will automatically return the value of a lone expression entered at the repl. Note:  this only works for single expressions.
```
i> alist[0] -- works
```
```
i> alist=new(List)
alist[0] -- doesn't work
```
```
i> =alist[0] -- no longer works
```
### Repl/Editor settings
Repl/Editor saves it's position, Repl visibility status, and Editor visibility status.

### Lua Constructor
Lua now has a function for constructing types into instances. 
Called new(). Can construct an instance of a class using a type imported using the new Import system.
```
i> List=import.System.Collections.Generic.List
r> void
i> alist=new(List)
r> void
i> alist.Add(1)
r> void
i> alist[0]
r> 1
```

### Better Lua intellisense
Lua Intellisense now knows whether a reference is static or an instance variable, so it no longer lists all the instance members of a class in the context of a static class reference.

# 0.2:
<details><summary>Show/Hide</summary>
  
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

</details>

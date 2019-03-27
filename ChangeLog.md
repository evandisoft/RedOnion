# 0.2.0:
### General:
- Improved error handling and reporting (line number and line content of where error/exception occured)

### ROS:
- Fixed some engine and parser bugs (functions, lambdas and loops)
- Enhanced reflection, especially for IEnumerable and List (new for var e in list)

### Documentation:
- Added CommonScriptApi.md
- Updated Readme.md
- Updated Docs for ROS and UI

### Lua:
- Wrapper table class called, ProxyTable, created. Can be used to act in place of classes that MoonSharp cannot properly reflect.

### Scripts:
- majorMalfunction.lua: A fun script that causes random parts to explode.
- selfDestruct.lua: A script that explodes all the parts starting from the parts furthest from the root.
- testFlightGui.ros: A script that creates a simple UI for interacting with the autopilot.

## MoonSharpGlobals

Functionality that is specific to Kerbalua.


**Types:**
- `reflection`: [Reflection](Reflection.md) - \[`Unsafe`\] Unsafe, Kerbalua specific reflection stuff.

**Static Methods:**
- `new()`: Object, static DynValue, dynArgs DynValue[]
  - Create new objects given a type. For example: `new(ui.Window)`
- `sleep()`: void, seconds double
  - Causes the script to wait, for the given number of seconds. This is not a precise timing mechanism. Sleeping for 0 seconds will cause the script to wait until the next unity FixedUpdate
- `dofile()`: DynValue, filename string
  - Executes the file with the given filename. The base directory is KSPDir/GameData/RedOnion/Scripts.

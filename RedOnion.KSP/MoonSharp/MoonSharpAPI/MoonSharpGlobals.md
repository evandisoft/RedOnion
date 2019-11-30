## MoonSharpGlobals

Functionality that is specific to Kerbalua.


**Types:**
- `reflection`: [Reflection](Reflection.md) - \[`Unsafe`\] Unsafe, Kerbalua specific reflection stuff.

**Static Methods:**
- `new()`: Object, type Object, dynArgs DynValue[]
  - \[`Unsafe`\] Create new objects given a static. A static is a special MunSharp value that represents a CLR Class.You can access static members from them, including the __new member, which represents the CLR Class' constructor.We provide references to many of these. For example, ui.Window, or any CLR Class received via the native global.
- `sleep()`: void, seconds double
  - Causes the script to wait, for the given number of seconds. This is not a precise timing mechanism. Sleeping for 0 seconds will cause the script to wait until the next unity FixedUpdate

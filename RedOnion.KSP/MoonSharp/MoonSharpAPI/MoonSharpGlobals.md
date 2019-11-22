## MoonSharpGlobals

Functionality that is specific to Kerbalua.

- `reflection`: [Reflection](Reflection.md) - Unsafe, Kerbalua specific reflection stuff.
- `new()`: Object, typeStaticOrObject Object, constructorArgs DynValue[]
  - Create new objects given a type, static, or object, in lua followed by the arguments to the constructor.
- `sleep()`: void, seconds double
  - Causes the script to sleep for the given number of seconds. This is not a precise timing mechanism. Sleeping for 0 seconds will cause the script to wait until the next unity FixedUpdate

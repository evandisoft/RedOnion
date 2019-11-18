## MoonSharpReflection

List of functions that are specific to KerbaluaScript.

- `isstatic()`: bool, dynValue DynValue
  - Returns true if the argument is a static.
- `isclrtype()`: bool, dynValue DynValue
  - Returns true if the argument is a Type.
- `getstatic()`: DynValue, dynValue DynValue
  - Returns a static based on the given Type or object.
- `getclrtype()`: Object, dynValue DynValue
  - Returns the underyling clr type associated with this DynValue.

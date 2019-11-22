## Reflection

Reflection functionality specific to Moonsharp.

- `isstatic()`: bool, possbileStatic DynValue
  - Returns true if the argument is a static.
- `isclrtype()`: bool, possibleType DynValue
  - Returns true if the argument is a Type.
- `getstatic()`: DynValue, typeOrObject DynValue
  - Returns a static based on the given Type or object.
- `getclrtype()`: Object, object DynValue
  - Returns the underyling clr type associated with this object.

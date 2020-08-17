## Reflection

Reflection functionality specific to Moonsharp.


**Static Methods:**
- `istype()`: bool, possibleType DynValue
  - Returns true if the argument is a type.
- `isruntimetype()`: bool, possibleRuntimeType DynValue
  - Returns true if the argument is a runtime type. (which can be used for reflection).
- `gettype()`: DynValue, runtimeTypeOrObject DynValue
  - Returns a type given a runtime type or object.
- `getruntimetype()`: Object, objectOrType DynValue
  - Returns the runtime type associated with this object or type. runtime types can be used for reflection.
- `getdescriptor()`: IUserDataDescriptor, object DynValue
  - Returns the descriptor of the given object, null if one is not avaialble.

## NamespaceInstance


You can use `native` to access a type in the default list of assemblies.

`native` allows you to browse through all the namespaces and classes that are available
like you can in C# for C#'s `using` functionality, with the completion area showing you the
available namespaces/types in a given namespace.

For example: `native.System.Collections.Generic.List` returns the Generic type List<>.

The returned type, for generic types like List<> is List\<object\>.

(There is a bug in Kerbalua's new that can occur when you pass no arguments except the type. Until this is fixed, you can use `type.__new()` for a zero argument constructor)
In Kerbalua, you can instantiate one of these types with `new(type)`, while
in ROS, you can instantiate them with the `new` keyword: `new type`.



## GetMappings


`native` is restricted to a default list of assemblies.

To access other assemblies, you can use `assembly.assemblyname`.

`assembly.assemblyname` returns a new `NamespaceInstance` which acts like `native`
and allows you to search through the namespaces available in the assembly and retrieves
types therein.

When the assemblyname has a space in it or another special character that would not work
well with `assembly.assemblyname`, like `assembly.abc def`, you have to use
assembly\['abc def'\] instead, since the interpreter sees `assembly.abc` and `def` as two
different tokens.



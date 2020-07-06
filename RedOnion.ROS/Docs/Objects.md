## Built-in objects and functions

- [String](#string) - sequence of characters.
- [Print](#print) - output formatted string.
- [List](#list) - sequence of values.
- [Queue](#queue) - FIFO sequence of values.
- [Stack](#stack) - LIFO sequence of values.

## String

`String` is a sequence of characters, piece of text. Anything compared to a string first gets converted to a string (including enums)
and the comparision is case-insensitive (`==`, `!=`, `<`, `<=`, `>`, `>=` operators).
Constructor of `string` can also be used for conversions (`new` is optional, it works as function without it as well).
`string 123` results in `"123"`.

- `length`: int (alias `count`) - number of characters in the string.
- `substring()`: string, from:int, length:int (alias `substr`)
  - get part of the sequence starting by character at index `from`, take `length` characters or the rest of the string if this parameter is not provided.
- `compare()`: int, other:string (alias `compareTo`)
  - case-sensitive comparision, compare the result to zero to get the ordering (`"alpha".compare("beta") > 0`).
- `equals()`: bool, other:string
  - case-sensitive equality check (operator `==` is case-insensitive, `===` is case-sensitive and works like this `equals`).
- `contains()`: bool, what:string
  - search for substring in this string (case-sensitive).
- `format()`: string, ...
  - standard .NET/C# [Composite formatting](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting)
  - can be used in both *static form*: `string.format "value of {0} is {1}", "x", 2`
  and *method form*: `"value of {0} is {1}".format "x", 2`

## Print

`Print` is a functions that works the same as `string.format` (*static form* only) but also prints the result to output window of the REPL.

## List

`List` is sequence of values/objects you can append to, remove from and enumerate (`for`/`foreach`).
It is derived from .NET [List&lt;Value&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)
and its constructor can be used to copy any enumerable (collection) of any type (to e.g. get a snapshot of `ship.parts` or `ship.science`).

## Queue

`Queue` is sequence of values/objects designed for first-in-first-out buffers,
based on .NET [Queue&lt;Value&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1),
with `push` redirected to `enqueue` and `pop` redirected to `dequeue` methods.

## Stack

`Stack` is sequence of values/objects designed for last-in-first-out buffers,
based on .NET [Stack&lt;Value&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1).

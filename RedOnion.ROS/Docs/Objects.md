## Built-in objects and functions

- [String](#string) - sequence of characters.
- [Print](#print) - output formatted string.
- [List](#list) - sequence of values.
- [Queue](#queue) - FIFO sequence of values.
- [Stack](#stack) - LIFO sequence of values.
- [Dictionary](#dictionary) - collection of key-value pairs

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
- `format()`: string, format:string, ...
  - standard .NET/C# [Composite formatting](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting)
  - can be used in both *static form*: `string.format "value of {0} is {1}", "x", 2`
  and *method form*: `"value of {0} is {1}".format "x", 2`
  - note that providing no argument (after `format` argument or for *method form*) bypasess the formatting (returning the `format` string), so you do not need to take care about `{}` in that case (applies to `print` as well).

## Print

`Print` is a function that works the same as `string.format` (*static form* only) but also prints the result to output window of the REPL.

## List

`List` is sequence of values/objects you can append to, remove from and enumerate (`for`/`foreach`).
It is derived from .NET [List&lt;Value&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)
and its constructor can be used to copy any enumerable (collection) of any type (to e.g. get a snapshot of `ship.parts` or `ship.science`).

- `count`: int (alias `length`)
  - number of elements
- `empty`: bool
  - true if list is empty (count == 0).
- `[index:int]`: value
  - get or set value at specified index (can add at the end using `list[list.count] = newValue`, throws exception if outside \[0..count\] range for set, \[0..count) for get.)
- `add()`, value
  - add new value to the list/collection (at the end).
- `insert()`, index:int, value
  - insert new value at specified index.
- `removeAt()`, index:int
  - remove element at specified index.
- `clear()`
  - remove all elements from the list.
- `contains()`: bool, value
  - test existence of an element (with equal value).
- `indexOf()`: int, value
  - search for an element, return its index or -1 if not found.
- `indexOf()`: int, value, from:int, count:int
  - version limited by range.

## Queue

`Queue` is sequence of values/objects designed for first-in-first-out buffers,
based on .NET [Queue&lt;Value&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1),
with `push` redirected to `enqueue` and `pop` redirected to `dequeue` methods.

- `count`: int (alias `length`)
  - number of elements
- `empty`: bool
  - true if queue is empty (count == 0).
- `enqueue()`, value (alias `push`)
  - add new value at the end of the queue.
- `dequeue()`: value (alias `pop`)
  - remove one from the start of the queue (throws if empty).
- `peek()`: value
  - get value at the start of the queue (without removing it, throws if empty)
- `clear()`
  - remove all elements from the queue.

## Stack

`Stack` is sequence of values/objects designed for last-in-first-out buffers,
based on .NET [Stack&lt;Value&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1).

- `count`: int (alias `length`)
  - number of elements
- `empty`: bool
  - true if stack is empty (count == 0).
- `push()`, value
  - add new value at the end of the queue.
- `pop()`: value
  - remove one from the start of the queue (throws if empty).
- `peek()`: value
  - get value at the start of the queue (without removing it, throws if empty)
- `clear()`
  - remove all elements from the stack.

## Dictionary

`Dictionary` is collection of key-value pairs used to quickly find values by their keys, based on .NET [Dictionary<Value,Value>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2). This is often used to store additional data related to some identifier (like age for name or UI components describing science experiment).

- `count`: int (alias `length`)
  - number of key-value pairs
- `empty`: bool
  - true if dictionary is empty (count == 0).
- `[key]`: value
  - get or set value associated to a key, returns `null` for non-existent keys.
- `add()`, key, value
  - add new key-value pair to the collection, throws if it already exists
- `tryAdd()`: bool, key, value
  - try add new key-value pair, returns false if already present (not changing the value)
- `contains()`: bool, key (alias `containsKey`)
  - tests whether key exists in the collection (you can usually test `dict[key] == null`, but that won't work if you trully want to store `null`)
- `remove()`: bool, key
  - remove key-value pair by the key (returns false if not found)
- `clear()`
  - remove all pairs from the dictionary.
(This may be confusing and poorly written. Maybe better than nothing. Maybe not. Ask if you have any questions.)

## Parsing
Kerbalua uses [ANTLR](https://www.antlr.org/) to parse through code in order to find the context of what it is being asked to find completions for.

It does this using a special grammar that finds the last variable name prior to the cursor position and stores a list of operations that were chained from that variable.

For example, in the following code
```
native.System.Collections.Generic.
```
the operation list is 

`Get(native), Get(System), Get(Collections), Get(Generic), Get()`

The first operation is `Get(native)`, and the completion system is going to interpret it as a global variable acceout of the table, and uses it to look up the first operation.

## Completion
After the `CompletionOperations` object has been created, completion is started with the script's Global variables Table and the first operation.

The Global variables Tables becomes the first [CompletionObject](CompletionTypes/CompletionObjectReadme.md).

`Get(native)` is looked up in the table, and returned to become the new `CompletionObject`

Then that is searched for `Get(System)` and so on.

Once it gets to the end, the final `Get()` represents that the completion system should display all possibilities.

If the final operation was `Get(ab)` it would only return possiblities containing the string "ab".
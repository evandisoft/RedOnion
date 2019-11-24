## AutoRun

Used to get, set, or modify the current list of scripts that are to be autoran  whenever an engine is initialized or reset.

- `clear()`: void - Clears the list and saves the empty list.
- `scripts()`: IList\[string\] - Returns a list of the current autorun scripts
- `save()`: void, scripts IList\[string\]
  - Saves the given list of scripts as the new list of autorun scripts.
- `add()`: void, scriptname string
  - Adds a new scriptname to the list of autorun scripts
- `remove()`: void, scriptname string
  - Removes the given scriptname from the list of autorun sccripts

## AutoRun

Used to get, set, or modify the current list of scripts that are to be autoran  whenever an engine is initialized or reset.

- `scripts()`: IList`1 - Returns a list of the current autorun scripts
- `save()`: Void, scripts IList`1
  - Saves the given list of scripts as the new list of autorun scripts.
- `add()`: Void, scriptname String
  - Adds a new scriptname to the list of autorun scripts
- `remove()`: Void, scriptname String
  - Removes the given scriptname from the list of autorun sccripts

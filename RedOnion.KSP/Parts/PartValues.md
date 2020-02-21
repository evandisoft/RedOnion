## PartValues

Tags and values that can be attached to a part (in editor or flight).


**Instance Properties:**
- `[tag string]`: string - Get or set the value associated with a key ('null' for tags and non-existing).

**Instance Methods:**
- `has()`: bool, tag string
  - Test for existence of a tag or key-value pair.
- `contains()`: bool, tag string
  - Test for existence of a tag or key-value pair.
- `get()`: string, tag string
  - Get the value associated with a key ('null' for tags and non-existing).
- `set()`: void, tag string, value string
  - Set the value associated with a key (default 'null' for tags).
- `add()`: bool, tag string, value string
  - Add new tag or key-value pair (default 'null' for tags).

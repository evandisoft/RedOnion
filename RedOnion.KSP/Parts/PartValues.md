## PartValues

Tags and values that can be attached to a part (in editor or flight).


**Instance Properties:**
- `[tag string]`: string - Get or set the value associated with a key (empty string for tags and 'null' for non-existing / removing).
- `count`: int - Number of tags and key-value pairs.
- `[at int]`: string - Get or set the value associated with a key (empty string for tags and 'null' for non-existing / removing).

**Instance Methods:**
- `has()`: bool, tag string
  - Test for existence of a tag or key-value pair.
- `contains()`: bool, tag string
  - Test for existence of a tag or key-value pair.
- `get()`: string, tag string
  - Get the value associated with a key (empty string for tags, 'null' for non-existing).
- `set()`: void, tag string, value string
  - Set the value associated with a key (default empty string for tags, 'null' for removing).
- `add()`: bool, tag string, value string
  - Add new tag or key-value pair (default empty string, `null` will return false immediately).

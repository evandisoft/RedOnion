## TimeStamp

Absolute time (and date). (Like `DateTime`)


**Instance Fields:**
- `seconds`: double - The contained value - seconds since start of the game (or save).

**Static Fields:**
- `none`: TimeStamp - No time (contains `NaN`). Useful for initialization of time-stamp variables because `time.since(none) = inf`.

**Instance Properties:**
- `s`: double - Shortcut for `seconds`.
- `parts`: [TimeParts](TimeParts.md) - The time-stamp separated into parts (second, hour, ...).

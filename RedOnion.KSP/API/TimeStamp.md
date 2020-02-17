## TimeStamp

Absolute time (and date). (Like `DateTime`)


**Instance Fields:**
- `seconds`: double - The contained value - seconds since start of the game (or save).

**Static Fields:**
- `never`: TimeStamp - Infinitely in the past (`-inf`). Useful for initialization of time-stamp variables.
- `none`: TimeStamp - No time (contains `NaN`). Note that `time.since(none) = inf` but `now - none` is still `none`.

**Instance Properties:**
- `s`: double - Shortcut for `seconds`.
- `parts`: [TimeParts](TimeParts.md) - The time-stamp separated into parts (second, hour, ...).

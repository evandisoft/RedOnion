## TimeDelta

Relative time. (Like `TimeSpan`)


**Constructors:**
- `TimeDelta()`: seconds double
  - Create new time-delta/span given seconds.

**Instance Fields:**
- `seconds`: double - The contained value - total seconds.

**Static Fields:**
- `none`: TimeDelta - No time-delta/span (contains `NaN`).

**Instance Properties:**
- `parts`: [TimeParts](TimeParts.md) - The time-delta/span separated into parts (second, hour, ...).
- `minutes`: double - Total minutes (`seconds / 60`).
- `hours`: double - Total hours (`seconds / 3600`).

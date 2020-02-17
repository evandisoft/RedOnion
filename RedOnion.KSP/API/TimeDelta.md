## TimeDelta

Relative time. (Like `TimeSpan`)


**Constructors:**
- `TimeDelta()`: seconds double
  - Create new time-delta/span given seconds.

**Instance Fields:**
- `seconds`: double - The contained value - total seconds.

**Static Fields:**
- `zero`: TimeDelta - Zero time-delta/span.
- `none`: TimeDelta - No time-delta/span (contains `NaN`).
- `infinite`: TimeDelta - Infinite time-delta/span (contains `+Inf`).

**Instance Properties:**
- `s`: double - Shortcut for `seconds`.
- `parts`: [TimeParts](TimeParts.md) - The time-delta/span separated into parts (second, hour, ...).
- `minutes`: double - Total minutes (`seconds / 60`).
- `hours`: double - Total hours (`seconds / 3600`).
- `days`: double - Total days (`hours / 6` - Kerbal day has only 6 hours).
- `years`: double - Total days (`days / 426` - Kerbal year has 426 days).

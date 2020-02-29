## Time

The simulation time, in seconds, since the game or this save was started. Returns `now` when used as a function or `TimeDelta` when number is provided.


**Types:**
- `warp`: [TimeWarp](TimeWarp.md) - Time warping utilities
- `stamp`: [TimeStamp](TimeStamp.md) - Absolute time (and date). (Like `DateTime`)
- `delta`: [TimeDelta](TimeDelta.md) - Relative time. (Like `TimeSpan`)
- `span`: [TimeDelta](TimeDelta.md) - Relative time. (Like `TimeSpan`)

**Static Properties:**
- `now`: [TimeStamp](TimeStamp.md) - The simulation time, since the game or this save was started. Suitable for measuring time and printing.
- `never`: [TimeStamp](TimeStamp.md) - Infinitely in the past (`-inf`). Useful for initialization of time-stamp variables.
- `none`: [TimeStamp](TimeStamp.md) - No time (contains `NaN`). Note that `time.since(none) = inf` but `now - none` is still `none`.
- `seconds`: double - The simulation time in seconds, since the game or this save was started. For pure computation (same as `now.seconds` or `now.s`).
- `tick`: [TimeDelta](TimeDelta.md) - Time delta/span of one tick. (Script engine always runs in physics ticks.)
- `real`: double - Real time since startup in seconds. (Good for printing reports e.g. every second.)

**Static Methods:**
- `since()`: [TimeDelta](TimeDelta.md), time Object
  - Time delta/span since some previous time (`TimeStamp` or `double`). Returns `infinite` if `time` is `none`. (Use `.s` or `.seconds` on the result if you want pure `double` value).
- `sinceReal()`: double, real double
  - Real time since previous point. Returns infinity if input is `nan`.

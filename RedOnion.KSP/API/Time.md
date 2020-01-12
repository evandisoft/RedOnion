## Time

The simulation time, in seconds, since the game or this save was started. Returns `now` when used as a function or `TimeDelta` when number is provided.


**Types:**
- `warp`: [TimeWarp](TimeWarp.md) - Time warping utilities
- `stamp`: [TimeStamp](TimeStamp.md) - Absolute time (and date). (Like `DateTime`)
- `delta`: [TimeDelta](TimeDelta.md) - Relative time. (Like `TimeSpan`)
- `span`: [TimeDelta](TimeDelta.md) - Relative time. (Like `TimeSpan`)

**Static Properties:**
- `now`: [TimeStamp](TimeStamp.md) - The simulation time, since the game or this save was started. Suitable for measuring time and printing.
- `seconds`: double - The simulation time in seconds, since the game or this save was started. For pure computation (same as `now.seconds`).
- `tick`: [TimeDelta](TimeDelta.md) - Time delta/span of one tick. (Script engine always runs in physics ticks.)

**Static Methods:**
- `since()`: [TimeDelta](TimeDelta.md), time [TimeStamp](TimeStamp.md)
  - Time delta/span since some previous time. (Use `.seconds` on the result if you want pure `double` value).
- `secondsSince()`: double, time [TimeStamp](TimeStamp.md)
  - Seconds since some previous time. Returns `+Inf` if `time` is `NaN`.

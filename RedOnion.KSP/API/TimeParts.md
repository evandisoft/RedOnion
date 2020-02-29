## TimeParts

TimeSpan or TimeDelta converted to its parts - hours, minutes, seconds etc.


**Constructors:**
- `TimeParts()`: seconds double
  - Create TimeParts from seconds.

**Instance Fields:**
- `fraction`: double - Sub-seconds fraction. \[0..1) or `Inf` or `NaN`
- `negative`: bool - Time is negative.
- `second`: byte - Seconds part. \[0..59]
- `minute`: byte - Minutes part. \[0..59]
- `hour`: byte - Hour part. \[0..5]
- `day`: ushort - Day part. \[0..425]
- `year`: ushort - Year part. \[0..65535]

**Instance Properties:**
- `valid`: bool - Time is valid (`fraction` not `NaN`).
- `finite`: bool - Time is finite (`fraction` not `NaN` or `Inf`).

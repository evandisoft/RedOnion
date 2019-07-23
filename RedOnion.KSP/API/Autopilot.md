## Autopilot

Autopilot (throttle and steering) for a ship (vehicle/vessel).

- `throttle`: Single - Throttle control (0..1). NaN for releasing the control.
- `elevation`: Single - Target elevation (aka pitch, -180..+180). Values outside -90..+90 flip heading.NaN for releasing the control.
- `Disable()`: Void - Disable the autopilot, setting all values to NaN.

## Autopilot

Autopilot (throttle and steering) for a ship (vehicle/vessel).

- `throttle`: Single - Throttle control (0..1). NaN for releasing the control.
- `rawPitch`: Single - Raw pitch control (up-down, -1..+1). NaN for releasing the control.
- `rawYaw`: Single - Raw yaw control (left-right, -1..+1). NaN for releasing the control.
- `rawRoll`: Single - Raw roll control (rotation, -1..+1). NaN for releasing the control.
- `direction`: Vector3d - Target direction vector. NaN/vector.none for releasing the control.
- `heading`: Double - Target heading [0..360]. NaN for releasing the control.
- `pitch`: Double - Target pitch/elevation [-180..+180]. Values outside -90..+90 flip heading.NaN for releasing the control.
- `roll`: Double - Target roll/bank [-180..+180].NaN for releasing the control.
- `disable()`: Void - Disable the autopilot, setting all values to NaN.
- `reset()`: Void - Reset the autopilot to default settings.

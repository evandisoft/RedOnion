## Autopilot

Autopilot (throttle and steering) for a ship (vehicle/vessel).


**Instance Properties:**
- `throttle`: float - Throttle control (0..1). NaN for releasing the control.
- `rawPitch`: float - Raw pitch control (up-down, -1..+1). NaN for releasing the control.
- `rawYaw`: float - Raw yaw control (left-right, -1..+1). NaN for releasing the control.
- `rawRoll`: float - Raw roll control (rotation, -1..+1). NaN for releasing the control.
- `direction`: [Vector](Vector.md) - Target direction vector. NaN/vector.none for releasing the control.
- `heading`: double - Target heading [0..360]. NaN for releasing the control.
- `pitch`: double - Target pitch/elevation [-180..+180]. Values outside -90..+90 flip heading. NaN for releasing the control.
- `roll`: double - Target roll/bank [-180..+180]. NaN for releasing the control.
- `killRot`: bool - Fix the roll of the ship. Note that this is not SAS and currently does not allow user override.
- `sas`: bool - SAS: Stability Assist System. This is stock alternative to `killRot` which allows user override. Can be used to allow user/player to adjust roll while autopilot controls direction.
- `rcs`: bool - RCS: Reaction Control System.

**Instance Methods:**
- `disable()`: void - Disable the autopilot, setting all values to NaN.
- `reset()`: void - Reset the autopilot to default settings.

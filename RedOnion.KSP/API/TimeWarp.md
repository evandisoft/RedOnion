## TimeWarp

Time warping utilities

- `ready`: bool - Indicator that warping utilities are ready for commands. (`to` will return false otherwise.)
- `low`: bool - Warp mode set to low aka physics warp. Note that it can only be changed on zero rate-index (and when `ready`).
- `high`: bool - Warp mode set to high aka on-rails warp. Note that it can only be changed on zero rate-index (and when `ready`).
- `rate`: float - Current rate.
- `index`: int - Current rate index.
- `to()`: bool, time double
  - Warp to specified time. Returns true if engaged, false if not ready.

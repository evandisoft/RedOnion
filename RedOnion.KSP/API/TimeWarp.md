## TimeWarp

Time warping utilities


**Static Properties:**
- `engaged`: bool - Warp-to engaged / in progress. Looks for `TimeWarpTo` lock.
- `ready`: bool - Indicator that `warp.to` can be used.
- `low`: bool - Warp mode set to low aka physics warp. Note that it can only be changed on zero rate-index (and when not `engaged`).
- `high`: bool - Warp mode set to high aka on-rails warp. Note that it can only be changed on zero rate-index (and when not `engaged`).
- `rate`: float - Current rate.
- `index`: int - Current rate index.

**Static Methods:**
- `setIndex()`: bool, value int
  - Set rate index. Returns false if not possible now.
- `setRate()`: bool, value float
  - Set rate. Returns false if not possible now.
- `to()`: bool, time [TimeStamp](TimeStamp.md)
  - Warp to specified time. Returns true if engaged, false if not ready.
- `cancel()`: bool - Cancel any warp-to in progress. Returns true if it was canceled, false if no warp was in progress.

## PID.1

**Derived:** [PID](PID.md)

PID(R) regulator (with extra parameters).


**Instance Properties:**
- `param`: Params - All the parameters.
- `P`: double - Proportional factor (strength of direct control).
- `I`: double - Integral factor (dynamic error-correction, causes oscillation as side-effect).
- `D`: double - Derivative factor (dampening - applied to output, reduces the oscillation).
- `R`: double - Reduction factor for accumulator (dampening - applied to accumulator used by integral factor, works well against both oscillation and windup).
- `scale`: double - Difference scaling factor.
- `targetChangeLimit`: double - Maximal abs(target - previous target) per second. NaN or +Inf means no limit (which is default). This can make the output smoother (more human-like control) and help prevent oscillation after target change (windup).
- `outputChangeLimit`: double - Maximal abs(output-input) and also abs(target-input) for integral and reduction factors). Helps preventing overshooting especially after change of Target (windup). NaN or +Inf means no limit (which is default).
- `errorLimit`: double - Limit of abs(target-input) used by P and I factors. Prevents over-reactions and also reducing windup.
- `accumulatorLimit`: double - Limit of abs(accumulator) used by I and R factors. Another anti-windup measure to prevent overshooting.
- `input`: double - Feedback (true state - e.g. current pitch; error/difference if Target is NaN).
- `maxInput`: double - Highest input allowed.
- `minInput`: double - Lowest input allowed.
- `target`: double - Desired state (set point - e.g. desired/wanted pitch; NaN for pure error/difference mode, which is the default). The computed control signal is added to Input if Target is valid, use error/difference mode if you want to add it to Target.
- `maxTarget`: double - Highest target allowed.
- `minTarget`: double - Lowest target allowed.
- `output`: double - Last computed output value (control signal, call Update() after changing Input/Target).
- `maxOutput`: double - Highest output allowed.
- `minOutput`: double - Lowest output allowed.
- `dt`: double - Time elapsed since last update (in seconds), Time.tick after reset.
- `lastDt`: double - Time elapsed between last and previous update.

**Instance Methods:**
- `reset()`: void - Reset internal state of the regulator (won't change PIDR and limits).
- `resetAccu()`: void - Reset accumulator to zero.
- `update()`: double - Update output according to time elapsed (and Input and Target).
- `update()`: double, dt double, input double
  - Set input and update output according to time elapsed (provided as dt).
- `Update()`: double, dt double, input double, target double
  - Set input and target and update output according to time elapsed (provided as dt).
- `update()`: double, dt double
  - Update output according to time elapsed (provided as dt, using current Input and Target).

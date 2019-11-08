## PidParams

PID(R) regulator parameters.

- `P`: double - Proportional factor (strength of direct control)
- `I`: double - Integral factor (dynamic error-correction, causes oscillation as side-effect)
- `D`: double - Derivative factor (dumpening - applied to output, reduces the oscillation)
- `R`: double - Reduction factor for accumulator (dumpening - applied to accumulator used by integral factor, works well against both oscillation and windup)
- `scale`: double - Difference scaling factor
- `targetChangeLimit`: double - Maximal abs(Target - previous Target) per second. NaN or +Inf means no limit (which is default). This can make the output smoother (more human-like control) and help prevent oscillation after target change (windup).
- `outputChangeLimit`: double - Maximal abs(output-input) and also abs(target-input) for integral and reduction factors). Helps preventing overshooting especially after change of Target (windup). NaN or +Inf means no limit (which is default)
- `errorLimit`: double - Limit of abs(target-input) used by P and I factors. Prevents over-reactions and also reducing windup.
- `accumulatorLimit`: double - Limit of abs(accumulator) used by I and R factors. Another anti-windup measure to prevent overshooting.

## \[`WIP`\] Autopilot.PIDs

Set of PID(R) controllers used by the autopilot. Simple PI-regulator with small `I`
would do (some non-zero `I` is needed to eliminate final offset, especially for roll)
as these are used to modify raw controls (-1..+1). Other parameters were itegrated,
the most important probably being `strength` which determines how aggressive the autopilot is.


**Instance Properties:**
- `pitch`: PidParams - Pitch control PID(R) parameters.
- `yaw`: PidParams - Yaw control PID(R) parameters.
- `roll`: PidParams - Roll control PID(R) parameters.
- `P`: double - Proportional factor (strength of direct control) for all three angles (`NaN` if not same).
- `I`: double - Integral factor (dynamic error-correction, causes oscillation as side-effect) for all three angles (`NaN` if not same).
- `D`: double - Derivative factor (dampening - applied to output, reduces the oscillation) for all three angles (`NaN` if not same).
- `R`: double - Reduction factor for accumulator for all three angles (`NaN` if not same;dampening - applied to accumulator used by integral factor, works well against both oscillation and windup).
- `strength`: double - Common strength/aggressiveness of control (`NaN` if not same`).

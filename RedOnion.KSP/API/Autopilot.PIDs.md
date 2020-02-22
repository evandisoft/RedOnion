## Autopilot.PIDs

Set of PID(R) controllers used by the autopilot.


**Instance Properties:**
- `pitch`: PidParams - Pitch control PID(R) parameters.
- `yaw`: PidParams - Yaw control PID(R) parameters.
- `roll`: PidParams - Roll control PID(R) parameters.
- `P`: double - Proportional factor (strength of direct control) for all three angles (`NaN` if not same).
- `I`: double - Integral factor (dynamic error-correction, causes oscillation as side-effect) for all three angles (`NaN` if not same).
- `D`: double - Derivative factor (dampening - applied to output, reduces the oscillation) for all three angles (`NaN` if not same).
- `R`: double - Reduction factor for accumulator for all three angles (`NaN` if not same;dampening - applied to accumulator used by integral factor, works well against both oscillation and windup).

## \[`WIP`\] PartScience

Available science through one part.


**Instance Properties:**
- `part`: [Part](PartBase.md) - The part this science belongs to.
- `native`: ModuleScienceExperiment - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_module_science_experiment.html)
- `ready`: bool - \[`WIP`\] Ready to perform experiment.
- `state`: [ScienceState](ScienceState.md) - \[`WIP`\] State of science module.

**Instance Methods:**
- `perform()`: bool, dialog bool
  - Perform the experiment (may take some time). Shows the dialog by default. Note that suppressing the dialog is rather experimental and may not work for some parts.

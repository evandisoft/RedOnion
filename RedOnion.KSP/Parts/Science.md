## \[`WIP`\] Science

Available science through one part.


**Instance Properties:**
- `part`: [Part](PartBase.md) - The part this science belongs to.
- `native`: ModuleScienceExperiment - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_module_science_experiment.html)
- `inoperable`: bool - Science module inoperable (cannot deploy).
- `deployed`: bool - Science module is deployed (that usually means it contains data and cannot be deployed again).

**Instance Methods:**
- `deploy()`: bool, dialog bool
  - Deploy the experiment (may take some time). Shows the dialog by default. Note that suppressing the dialog is rather experimental and may not work for some parts.

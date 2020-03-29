## \[`WIP`\] ScienceState

State of science module.


**Static Fields:**
- `Ready`: ScienceState - Can perform experiment now.
- `Inoperable`: ScienceState - Module inoperable.
- `Shielded`: ScienceState - Module is shielded and cannot perform experiments now.
- `NoControl`: ScienceState - Ship is currently not controllable (and module requires it).
- `NoCrew`: ScienceState - No crew in ship or the part (as required by the module).
- `NoScientist`: ScienceState - No scientist in ship or the part (as required by the module).
- `Cooldown`: ScienceState - Module needs sime time before operation.
- `Uknown`: ScienceState - Unknown state, probably cannot perform experiments now.

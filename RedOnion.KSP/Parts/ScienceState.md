## \[`WIP`\] ScienceState

State of science module.


**Static Fields:**
- `ready`: ScienceState - Can perform experiment now.
- `full`: ScienceState - Module is full of data.
- `inoperable`: ScienceState - Module inoperable.
- `shielded`: ScienceState - Module is shielded and cannot perform experiments now.
- `noControl`: ScienceState - Ship is currently not controllable (and module requires it).
- `noCrew`: ScienceState - No crew in ship or the part (as required by the module).
- `noScientist`: ScienceState - No scientist in ship or the part (as required by the module).
- `cooldown`: ScienceState - Module needs some time before operation.
- `uknown`: ScienceState - Unknown state, probably cannot perform experiments now.

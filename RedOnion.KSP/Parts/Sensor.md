## Sensor

Part that is also a sensor. (Has `ModuleEnviroSensor`.)

- `native`: Part - Native `Part` - KSP API.
- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `children`: [PartChildren](PartChildren.md) - Parts attached to this part.
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: int - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: int - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `name`: string - Name of the part (assigned by KSP).
- `title`: string - Title of the part (assigned by KSP).
- `active`: bool - State of the sensor.
- `Type`: SensorType - Sensor type.
- `display`: string - Sensor read-out.
- `consumption`: double - Sensor electric consumption.
- `explode()`: void - Explode the part.
- `istype()`: bool, name string
  - Accepts `sensor`. (Case insensitive)
- `toggle()`: void - Toggle the state.

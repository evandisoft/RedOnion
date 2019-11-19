## Sensor

Part that is also a sensor. (Has `ModuleEnviroSensor`.)

- `ship`: [Ship](../API/Ship.md) - Ship (vehicle/vessel) this part belongs to.
- `native`: Part - Native `Part` - KSP API.
- `parent`: [Part](PartBase.md) - Parent part (this part is attached to).
- `decoupler`: [Decoupler](Decoupler.md) - Decoupler that will decouple this part when staged.
- `stage`: int - Stage number as provided by KSP API. (`Native.inverseStage`)
- `decoupledin`: int - Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)
- `resources`: [ResourceList](ResourceList.md) - Resources contained within this part.
- `state`: PartStates - State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).
- `active`: bool - State of the sensor.
- `Type`: SensorType - Sensor type.
- `display`: string - Sensor read-out.
- `consumption`: double - Sensor electric consumption.
- `istype()`: bool, name string
  - Accepts `sensor`. (Case insensitive)
- `toggle()`: void - Toggle the state.

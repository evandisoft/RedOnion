## Sensor

**Base Class:** [Part](PartBase.md)

Part that is also a sensor. (Has `ModuleEnviroSensor`.)


**Instance Properties:**
- `active`: bool - State of the sensor.
- `Type`: SensorType - Sensor type.
- `display`: string - Sensor read-out.
- `consumption`: double - Sensor electric consumption.
- `value`: double - \[`WIP`\] Sensor read-out as number.

**Instance Methods:**
- `istype()`: bool, name string
  - Accepts `sensor`. (Case insensitive)
- `toggle()`: void - Toggle the state.

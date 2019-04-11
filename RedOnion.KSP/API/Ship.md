## Ship

Active vessel (in flight) or ship construct (in editor).

- `native`: Vessel|ShipConstruct - Native Vessel or ShipConstruct according to scene.
- `vessel`: Vessel - Native Vessel (in flight only, null otherwise).
- `construct`: ShipConstruct - Native ShipConstruct (in editor only, null otherwise).
- `name`: string - Name of the ship/vessel.
- `id`: Guid - Unique identifier of the vessel (Guid.Empty if not in flight).
- `persistentId`: uint - Persistent ID (provided by KSP, should be same for all ships of same design).
- `parts`: List<Part> - All parts (may get changed to our future API class).
- `root`: Part - Root part (usually command module).
- `deltaV`: VesselDeltaV - KSP's native ΔV calculations.
- `resources`: PartSet - KSP's native resource part set (may get changed to our future API class).
- `mass`: float - Total mass of the ship.
- `type`: VesselType - Vessel type (flight only, returns VesselType.Ship otherwise).
- `control`: FlightCtrlState - Ship's controls (KSP native).
- `state`: Vessel.State - Current state of the ship (0 = inactive, 1 = active, 2 = dead).
- `landed`: bool - Wheter the ship is landed or not.
- `splashed`: bool - Wheter the ship is splashed or not.
- `stage`: [Stage](Stage.md) - Redirects to global stage function.
- `latitude`: double - Ship's latitude on current body.
- `longitude`: double - Ship's longitude on current body.
- `altitude`: double - Ship's mean-sea altitude.
- `radarAltitude`: double - Ship's altitude above ground.
- `lat`: double - Alias to latitude.
- `lon`: double - Alias to longitude.
- `alt`: double - Alias to altitude.
- `radar`: double - Alias to radarAltitude.

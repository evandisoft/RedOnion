## SolarPanel

**Base Class:** [Part](PartBase.md)

Solar panel.


**Instance Properties:**
- `retractable`: bool - Indicates retractable (non-broken) solar panel.
- `deployState`: DeployState - Deploy state.
- `extended`: bool - Panel is fully extended.
- `retracted`: bool - Panel is fully retracted (and not broken).
- `broken`: bool - Panel is broken.
- `extending`: bool - Panel is extending.
- `retracting`: bool - Panel is retracting.
- `panelType`: PanelType - Panel type.

**Instance Methods:**
- `extend()`: bool - Extend the panel.
- `retract()`: bool - Retract the panel.

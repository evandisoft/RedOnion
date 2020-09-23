## Propellant

Propellant consumed by engine.


**Instance Properties:**
- `name`: string - Name of the propellant (like `LiquidFuel').
- `native`: Propellant - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_propellant.html). First in the list if aggregate.
- `resourceDef`: PartResourceDefinition - \[`Unsafe`\] [KSP API](https://kerbalspaceprogram.com/api/class_part_resource_definition.html). First in the list if aggregate.
- `flowMode`: ResourceFlowMode - Flow mode of the propellant. First in the list if aggregate, which works for most propellants, but may be random when you combine e.g. Karbonite SRB's with normal engines.
- `solid`: bool - No flow propellant - usually solid fuel, bound to SRB.
- `liquid`: bool - Flowing propellant - liquid fuel and oxidizer, does not include Monopropellant, Xenon Gas or electricity.

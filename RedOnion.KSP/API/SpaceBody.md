## SpaceBody

Celestial body. (`SpaceBody` selected not to conflict with KSP `CelestialBody`.)

- `native`: CelestialBody - \[`Unsafe`\] KSP API. Native `CelestialBody`.
- `name`: string - Name of the body.
- `position`: [Vector](Vector.md) - Position of the body (relative to active ship).
- `body`: SpaceBody - Celestial body this body is orbiting.
- `bodies`: [ReadOnlyList](ReadOnlyList.1.md)\[SpaceBody\] - Orbiting celestial bodies.
- `radius`: double - Radius of the body [m].
- `mass`: double - Mass of the body [kg].
- `gravParameter`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `mu`: double - Standard gravitational parameter (μ = GM) [m³/s²]
- `atmosphere`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body.
- `atm`: [SpaceBody.Atmosphere](SpaceBody.Atmosphere.md) - Atmosphere parameters of the body. (Alias to `atmosphere`)

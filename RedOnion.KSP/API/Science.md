## \[`WIP`\] Science

Science tools.


**Types:**
- `Subject`: [Subject](Science.Subject.md)

**Static Fields:**
- `situationChanged`: [AutoRemoveList](../Utilities/AutoRemoveList.1.md)\[Action\] - Event executed when `situationId` changes.

**Static Properties:**
- `body`: [SpaceBody](SpaceBody.md) - Current space/celestial body.
- `situation`: ExperimentSituations - Current science situation.
- `situationId`: string - Current situation ID (`{body}{situation}{biomeId}`).
- `biomeName`: string - Current biome (display name).
- `biome`: string - Current biome (tag, returns subBiome if landed).
- `biomeId`: string - Current biome ID (spaces removed).
- `mainBiome`: string - Current main biome (tag, ignores landed).
- `mainBiomeId`: string - Current main biome ID (spaces removed).
- `subBiome`: string - Current sub-biome (tag, valid only if landed).
- `subBiomeId`: string - Current sub-biome ID (spaces removed).

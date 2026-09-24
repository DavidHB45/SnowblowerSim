# Scenery shopping list (free / CC0 first)

Everything here is downloaded **on Dave's machine** by `scripts/fetch-thirdparty.sh` from
`unity/Assets/ThirdParty/manifest.json`. The cloud container cannot reach these hosts
(Poly Haven, ambientCG and Sketchfab are all blocked by its egress policy). Imported files land in
`unity/Assets/ThirdParty/<source>/<id>/` and are never edited. Wrappers and material overrides go in
`unity/Assets/Art/`. Record every asset in `LICENSES-AND-ATTRIBUTION.md`.

## What photorealism needs (the bar for every pick)

- **Textures:** 4k minimum, full PBR set: albedo/diffuse, normal (OpenGL or DirectX, noted per
  asset), roughness, AO. Height/displacement if offered. No 1k/2k "preview" sets.
- **No baked lighting:** reject albedo maps with baked shadows/AO and models with baked lightmaps.
  Lighting is real-time (URP) plus an HDRI for sky/ambient.
- **Real scale:** meshes must be in meters at real size (a door is ~2.03 m, a mailbox post ~1.1 m).
  Textures need a known physical tile size (Poly Haven and ambientCG list it); record it in the
  material wrapper.
- **HDRIs:** unclipped, 4k for lighting (8k+ only if used as a visible backdrop). Prefer overcast
  or snowy captures so sky, ambient and snow albedo agree.
- **Licenses:** CC0 preferred; CC-BY only with the attribution recorded. No NC/ND licenses.

## Sources and exact search terms

### Poly Haven (polyhaven.com): all CC0
- **HDRIs.** Search `snowy`, `winter`, `overcast`. Browse *Skies > Overcast*.
- **Textures.** Search `snow`, `asphalt`, `concrete`, `pavement`, `brick`, `wood siding` / `planks`,
  `gravel`.

### ambientCG (ambientcg.com): all CC0
- **Materials** (4K-JPG zip). Search `Snow`, `Asphalt`, `Concrete`, `PavingStones`, `Bricks`,
  `WoodSiding`, `Planks`, `Gravel`, `Ground` (for frozen dirt/verges).

### Sketchfab (sketchfab.com): filter *Downloadable* + license *CC0* (CC-BY acceptable if logged)
- Models: `suburban house`, `garage`, `fence`, `mailbox`, `bare tree`, `pickup truck`,
  `street lamp`, `storefront`.
- Check each for real scale, triangle count, and PBR textures before adding it to the manifest.

## Per environment

### Suburban (driveways, sidewalks)
| Need | Poly Haven | ambientCG | Sketchfab (CC0) |
|---|---|---|---|
| Sky / lighting | `snowy_park_01` (overcast, bare trees), `snow_field_puresky` | — | — |
| Driveway | `asphalt_snow` (snow-dusted asphalt) | `Asphalt006` | — |
| Sidewalk | `pavement_03`, `gravel_concrete_04` | search `Concrete`, `PavingStones` | — |
| House walls | search `brick`, `wood siding` | `Bricks066`, search `WoodSiding` | `suburban house`, `garage` |
| Props | — | — | `fence`, `mailbox`, `bare tree`, `pickup truck` |
| Snow surface | search `snow` | `Snow006`, `Snow005` | — |

### Rural / Mountain (long lanes, gravel, deep snow)
| Need | Poly Haven | ambientCG | Sketchfab (CC0) |
|---|---|---|---|
| Sky / lighting | `snowy_forest` (overcast), `snowy_hillside_02` (overcast), `snowy_field` (sun) | — | — |
| Lane | `gravel_concrete_04`, search `gravel` | search `Gravel` | — |
| Buildings | search `wood siding`, `planks` | search `WoodSiding`, `Planks` | `garage` (barn/shed), `fence` |
| Props | — | — | `bare tree`, `pickup truck`, `mailbox` |
| Snow surface | search `snow` | `Snow010A`, `Snow006` | — |

### Main Street (sidewalks, storefronts, lamps)
| Need | Poly Haven | ambientCG | Sketchfab (CC0) |
|---|---|---|---|
| Sky / lighting | `winter_sky` (clear), `snowy_park_01` (overcast) | — | — |
| Street | `asphalt_snow` | `Asphalt006` | — |
| Sidewalk | `pavement_03` | search `PavingStones`, `Concrete` | — |
| Facades | search `brick` | `Bricks038`, `Bricks066` | `storefront` |
| Props | — | — | `street lamp`, `bare tree`, `pickup truck` |
| Snow surface | search `snow` | `Snow005` | — |

IDs in `code` were checked against the source sites (via web search) on 2026-09-24 and are in
`unity/Assets/ThirdParty/manifest.json`. "search …" entries still need a pick; add the chosen ID to the
manifest.

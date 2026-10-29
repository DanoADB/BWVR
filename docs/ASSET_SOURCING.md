# Sourced models — replacement inventory

Updated 2026-10-29. User direction: find open-source/CC0 models for every visible game item; do not create primitive placeholder artwork. This inventory covers the runtime item families and their remaining procedural visuals. **A candidate is not an imported or approved replacement.** Existing simulation/collision geometry must remain compatible while presentation changes.

## Source evidence

Prefer CC0 packs with a consistent stylized silhouette and simple materials. Keep the original archive license beside selected source files and record the creator, URL, exact model, retrieval date and any conversion. Check the actual download rather than assuming that every item on a marketplace or every tier of a pack has the same license. Scale, palette and pivot adaptation of a sourced mesh are permitted; a cube or sphere assembled in code is not a substitute for sourcing item artwork.

Verified source pages, 2026-10-28:

- [Kenney Survival Kit](https://kenney.nl/assets/survival-kit): CC0. The already-downloaded 2.0 archive was inspected for the exact FBX names below. `Assets/ThirdParty/KenneySurvival/License.txt` retains its license.
- [Kenney Minigolf Kit](https://kenney.nl/assets/minigolf-kit): CC0. Local archive inspected for ball, gate, track and support models. These are scalable meshes; a minigolf ball need not stay golf-ball-sized.
- [Kenney Brick Kit](https://kenney.nl/assets/brick-kit): local bundled CC0 license and exact beveled brick/slope/plate meshes inspected. Existing source record: `Assets/ThirdParty/Kenney/ConstructionBlocks/SOURCE.txt`.
- [Kenney Food Kit](https://kenney.nl/assets/food-kit): creator page and downloaded 2.0 archive explicitly CC0. Apple and bowl-cereal source meshes/palette are imported with license and SHA-256 records in `Assets/ThirdParty/KenneyFood`. The sack fill reuses extracted authored cereal geometry; remaining produce/meal candidates still need selection.
- [Quaternius Ultimate Crops](https://quaternius.com/packs/ultimatecrops.html): creator page explicitly CC0, 102 models and five growth stages. Candidate for crop visuals; inspect the actual archive before import.
- [Quaternius Ultimate RPG](https://quaternius.com/packs/ultimaterpg.html): creator page explicitly CC0, 106 models. Candidate for a physical Book, key and heart icons; inspect filenames and archive license before selecting.
- [Innerscene Xylophone](https://www.innerscene.com/tools/library/3d-parts/xylophone-5da5d32e): original model page explicitly CC0, downloadable GLB, 1.9 MB. Candidate for the chime. Mesh complexity, material count and detachable playable bars are not yet verified for Quest.
- SigilsVault Modular Cathedral Kit v1.0: `ArtSources/Cathedrals/LICENSE.txt` explicitly CC0; original README and selected source GLBs are present. See `docs/RELIGION_AND_CATHEDRALS.md` for provenance and assembly details.
- [Kenney Nature Kit](https://kenney.nl/assets/nature-kit): creator page explicitly CC0. Backup for ground props; prefer the Quaternius nature already present for visual consistency.

Existing imported Quaternius Nature/Village **Standard archive** license files explicitly say CC0: `Assets/ThirdParty/Quaternius/Nature/License_Standard.txt` and `Village/License_Standard.txt`. Retain those files as evidence for the downloaded versions. Other selected models have individual license/source records, including the [Quaternius bag](https://poly.pizza/m/rJuZexcuhU) and [cloud](https://poly.pizza/m/KdFNOVn1Gf).

## Item-by-item coverage

Status: **In use** means an existing sourced mesh is used for that item family, not that every fallback or accessory has been eliminated. **Archive candidate** means the exact filename exists in a locally inspected CC0 archive but has not yet been integrated. **Page candidate** needs download, file/license confirmation and art review.

| Item family / runtime owner | Selected source or candidate | Current state / remaining work |
|---|---|---|
| Villagers, clothing, animation — QuestSlice / HumanoidMotion | Existing Quaternius Universal Base Characters, Fantasy Outfits, Universal Animation Library | In use; retain bundled licenses. Remove primitive fallback villagers once missing-asset behavior is explicit. |
| Creature | Purchased Animal Friends Gorilla | Commercial licensed exception, not CC0. Keep the user's chosen expressive character for now; do not falsely label it public-domain or replace it silently. A fully CC0-only creature remains a separate art/rig decision. |
| Houses, inns, workshops — VillageBuildings | Existing Quaternius Medieval Buildings and Village models | In use. Remove fallback cube dwellings; preserve variety and current scale. |
| Cathedral stages — VillageCathedral | Existing SigilsVault Modular Cathedral Kit assemblies (CC0 archive license verified) | In use. Primitive foundation and custom triangle roof end caps in `tools/prepare-cathedrals.py` remain replacement tasks; use kit modules for those too. |
| Trees, stumps, saplings, bushes, flowers, rocks, path stones — EnvironmentArt / TreePlayWorld | Existing Quaternius nature; `tree-trunk`, `tree-log`, `rock-a`, `rock-b`, `rock-c` from Kenney Survival for remaining props | Nature models in use; archive candidates for generated stumps/cairns. Preserve tree affordances and rooted navigation. |
| Clouds and fog | Existing Quaternius Cloud and Kenney Smoke art | In use; fog/rain particles and shadows are effects, not physical item models. |
| Timber bundles, planks, storm salvage — VillageResources / StormSupplies | Kenney Survival `resource-wood`, `resource-planks` | In use, including storm salvage, the native wood offering and forester routine loads. Grounded base pivots and native pickup/drop are verified in the Editor. |
| Grain/food sacks — VillageResources / VillagerRoutine | Existing Quaternius Bag Open (`food-sack`) | In use, including farmer routine loads. The old primitive grain insert is replaced by combined copies of an authored Kenney cereal piece, with provenance retained. |
| Axes, hoes, hammers, shovels, pickaxes, workbench — ResourceArt / VillageToolmaking | Existing Kenney Survival `tool-*`, `workbench` | In use/available; replace the small primitive resin binding with material tint or sourced accessory. |
| Guidance signs — GodToolPalette / DivineIntervention | Existing Kenney `signpost`, `signpost-single`; existing licensed GodToolIcons | Meshes available. Replace remaining stick/disc assemblies with sourced sign or icon presentation. GodToolIcons include CC BY art: preserve attribution; they are not all CC0. |
| Water offering and villager vessel — WaterVessel / VillagerRoutine | Kenney Survival `bucket`, `barrel-open`; `bottle` remains an archive candidate | Bucket in use for the water offering and water gatherers; open barrel in use for the cistern. Empty/full effects, native delivery and hand attachment remain integrated. |
| Well/cistern | Existing well artwork and Kenney Survival `barrel-open` reservoir | Cistern container body now sourced, fitted to its existing height/radius. Well art and spring effects remain separate; source/delivery anchors and navigation footprint are retained. |
| Food offering, treats, individual gifts — QuestSlice / IslandDiscoveries | Kenney Food `apple` for the native offering; bread/vegetables for other gifts | Apple in use with existing Offering/Discovery IDs. Other treats/discovery/gift props remain a replacement task. |
| Communal tribute — VillageTributes | Kenney Survival `box-open` + sourced Food Kit produce | Crate already imported; produce is a page candidate. Replace generated bowl/spheres without altering tribute ownership or costs. |
| Wheat and other planted crops — ResourceArt / TurnipFarming | Quaternius Ultimate Crops growth-stage meshes | Page candidate. Replace generated stems/heads/turnips, retaining bounded crop state and batched rendering. |
| Resin jars — ResinGathering | Kenney Survival barrel-open plus Kenney Platformer jewel | In use for storage, carried loads and tapping cups; amber chunk fill follows the existing resin amount. |
| Covered stores, roof, posts, upgrades, shelves — VillageResources / ResourceStorage | Kenney Survival `structure`, `structure-floor`, `structure-roof`, `box`, `box-large`, `barrel` | Sourced timber frame, roof/floor, upgrade shelves and construction planks in use. Existing footprint and stock/capacity affordances retained. |
| Meal reserve crate and portions | Existing Kenney `box-open` + Food Kit produce | Crate and filled-sack portions in use, driven by the existing eight-slot reserve display. |
| Beachball / rolling toys — CreaturePlayground | Kenney Minigolf ball-blue | In use at existing large diameter, retaining striped material and spherical physics. |
| Turning-block toy | Shared sourced construction yard | Decorative toy retired; physical block play retained. |
| Chime / rhythm instrument — CreaturePlayground / PlayerRhythm | Innerscene Xylophone | In use: simplified frame/eight authored bars, two input groups, 492 triangles. |
| Bowling targets and lane — BowlingLane | Tomcat94 Bowling Pin; Kenney studs/jewels for cues | In use, fitted to existing physical capsules; scoring/reset/imitative play retained. |
| Movable ball goals / backstop — GoalCourse | Fitted Quaternius trim, Kenney Survival backboard and Brick/Platformer cues | In use; unchanged scoring opening, physical posts, carry roots and stable IDs. |
| Cooperative key, socket, paw pad and gate — CooperativePuzzle | Kenney key/jewel/pads/studs; fitted Quaternius trim and door | In use; native key pickup, matching socket and two-input physics puzzle retained. |
| Construction bricks/beams/cubes/columns/slabs/ramps/cylinders — ConstructionPieces | Kenney Brick beveled bricks, round bricks, plate and slope | In use across all eight shapes; existing dimensions, hidden physics and snap graph retained. |
| Construction arch, tray, foundation, example structures | Fitted Quaternius trim; Kenney Survival floor | Arch/tray/foundation in use. Example structures and other legacy construction visuals remain an audit task. |
| Exploration caches, boxes and tools — IslandDiscoveries | Kenney Survival `chest`, `box`, `bedroll-packed`, existing tool meshes | Archive candidates; preserve discovery attribution and player/creature reservations. |
| Book of Creation | Existing scroll UI; Quaternius RPG book for physical book representation | UI already present; book mesh is a page candidate. Preserve the existing scroll/stone visual language. |
| Relationship / mood / request icons | Existing attributed icon set, or licensed CC0 replacements from RPG/approved icon art | Replace generated heart mesh if used as a world item. Icon graphics need individual provenance; do not relabel CC BY icons CC0. |
| Pickup hand / controller presence — PickupHand | Hand mesh from existing CC0 Quaternius base-character rig, exported as a sourced derivative | Candidate processing task; preserve pointer pose, valid/invalid cues and accessibility. Not implemented. |
| Spirit companion | User-selected glow/particle entity | Effect presentation, not a physical primitive toy; no replacement creature model required. |

## What this changes now

The first contact-art batch replaces the three native offerings, water containers, routine carried loads and sack insert with sourced art. Pickup bounds fit the model meshes, and base-pivot offerings sit on terrain when released. Source geometry, empty/full views, native pickup/drop, water delivery and controlled nonverbal/hybrid behavior are Editor-verified. The table records the wider replacement work; **it does not claim the island is already free of primitive artwork.** Remaining generation sites are inventoried below. Imported-model availability must not be treated as implemented gameplay or verified Quest performance.

Replacement order: (1) high-contact objects: offerings, vessels, toys, chime, puzzle/key and goals; (2) construction shape coverage and physical stores; (3) crops, resin, tribute and discovery props; (4) fallback art, small accessories, hands and attribution cleanup. Each batch needs scale/pivot/material review, original-license retention, interaction/collider checks, a rendered view and relevant Editor tests before commit/push. Quest readability and frame-time remain a headset gate.

Invisible colliders, NavMesh footprints, snap/physics proxies, particle quads, water/terrain surfaces and transient selection rings remain technical/effect geometry. They must not be used as rendered substitutes for physical item models. Original Black & White terrain/local legacy artwork is not CC0 and remains separately identified; this document does not convert it into open-source content.

## Contact-art batch — limits and rebuild

`SettlementPropImporter.BuildSelected` imports just the chosen source models without rebuilding unrelated props. Bucket/open-barrel prefabs use the existing Survival palette. Apple and the source cereal use a separate Food palette. `FoodSackImporter.Build` extracts only the original golden cereal faces by their palette UV column, preserves original topology/normals/UVs, normalizes them and combines nine copies into the sack insert. No bowl or spoon is carried inside the sack. The original FBX and derivation notes remain in the source folder.

The only generated geometry in the new vessels is a sixteen-triangle liquid surface. Water updates remain visual state driven by the existing simulation. Cistern height/radius and villager scale are retained; the existing oversized-well layout is not redesigned in this art batch. Model bounds drive pickup colliders. Imported shared meshes/materials are never destroyed on pickup disposal. Native save data, stable target IDs, controls, resource accounting, water/food work preferences and unlimited test energy are unchanged.

Verification includes a rendered model sheet with empty/half/full buckets, cistern, apple, timber and filled sack; live native pickup/drop and an actual player water delivery; controlled villager signaling/autonomous fill-delivery and creature hybrid checks. Navigation must finish before fixtures run. Frozen traffic is excluded or moved aside in focused movement checks. Headset readability, hand-pose polish and device performance remain unverified. No APK is built.

## Procedural-art audit snapshot

The following runtime files contain primitive construction or generated meshes. Some sites are effects or invisible proxies; inspect usage before removing them. This list is a gap inventory, not a promise that each site is visible in the current scene.

- `BeachBall.cs`: lines 22.
- `BowlingLane.cs`: lines 42, 45.
- `ConstructionPieces.cs`: lines 29, 31, 39, 40.
- `ConstructionSite.cs`: lines 40, 47.
- `CooperativePuzzle.cs`: lines 63, 69.
- `CreatureBridge.cs`: lines 43, 49, 50.
- `CreatureConstructionLearning.cs`: lines 18.
- `CreaturePlayground.cs`: lines 18, 22, 25.
- `DivineIntervention.cs`: lines 37.
- `FarmExpansion.cs`: lines 38.
- `GoalCourse.cs`: lines 48.
- `GodToolEmblems.cs`: lines 21, 22, 28.
- `IslandDiscoveries.cs`: lines 48.
- `IslandEnvironment.cs`: lines 178.
- `IslandNavigation.cs`: lines 19.
- `IslandOcean.cs`: lines 22.
- `PickupHand.cs`: lines 14.
- `PlayerRhythm.cs`: lines 22.
- `QuestSlice.cs`: lines 37.
- `ResourceArt.cs`: lines 7, 8, 12.
- `SpiritCompanion.cs`: lines 54.
- `TreePlayWorld.cs`: lines 24.
- `TurnipFarming.cs`: lines 10, 11, 12.
- `VillageCathedral.cs`: lines 20.
- `VillageFamilies.cs`: lines 13.
- `VillageResources.cs`: lines 30.
- `VillageSettlement.cs`: lines 98, 121, 126.
- `VillageToolmaking.cs`: lines 29.
- `VillageTributes.cs`: lines 18.
- `WaterVessel.cs`: lines 17.

## Toy, goal, puzzle and yard batch

Delivered source files, derivations, rebuilding and validation are recorded in [PLAY_ART.md](PLAY_ART.md). The procedural-art snapshot above predates this replacement: several listed sites now create invisible physics proxies only. Other item families remain open; this batch does not claim full island art replacement.

## Village storage art batch

Sourced stores, upgrades, exposed grain, resin and reserve portions are delivered; see [STORAGE_ART.md](STORAGE_ART.md) for source records, fitting and live validation. Crop art, tribute produce and exploration gifts remain open.

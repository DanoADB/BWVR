# Sourced village storage art

Integrated 2026-10-29. Storage presentation now uses CC0 Kenney Survival timber frame, plank floor and roof models. The frame fits the existing 10-by-8 resource-store footprint; the shallow sloped roof replaces the old flat slab. Three upgrade shelves use fitted plank-floor models, and the work-in-progress beam uses the existing sourced plank bundle.

Uncovered grain and reserved meal portions use the existing Quaternius filled sack. Stored resin, tree tapping cups and carried resin use the existing Kenney open barrel with a tinted Kenney Platformer jewel representing amber resin. The existing source fill amount drives the amber chunk height/size. No new visible primitive container or resource marker is created by these storage paths.

## Gameplay boundaries

Resource counts, capacity, overflow/spoilage rates, upgrade costs/timing, navigation footprint, delivery approach, carry routines, source IDs and save schema are unchanged. Rendering still shows twelve bounded timber/sack stock slots, fifteen resin slots, eight reserve portions and three earned shelves. Empty stock disappears; exposed sacks stay outside cover; construction progress grows as work accumulates. The cosmetic roof height does not alter navigation or resource protection rules.

StorageArt.StorePart fits collider-free sourced prefabs to existing marker bounds. Renderer bounds affect presentation only. Original resource accounting and scheduler remain authoritative. No new food/water priority or economy changes are introduced.

## Sources and rebuild

- Kenney Survival Kit 2.0: https://kenney.nl/assets/survival-kit. Original bundled CC0 license: Assets/ThirdParty/KenneySurvival/License.txt. Selected original frame/roof FBX retained beside the existing floor, barrel and plank sources.
- Existing sack and cereal licenses/derivations: docs/ASSET_SOURCING.md and docs/SETTLEMENT_PROP_ART.md.
- Existing Platformer jewel source/license: ArtSources/Play/SOURCES.json.
- Exact selected source hashes: ArtSources/Storage/SOURCES.json.

Rebuild just the new architecture prefabs in Unity Edit mode with SettlementPropImporter.BuildSelected("Assets/ThirdParty/KenneySurvival/", "Assets/BWVR/Resources/SettlementProps/", "structure", "structure-roof", "structure-floor"). Originals retain palette UVs and topology; runtime fitting changes scale/pivot only. Other prefab importers need not be rerun.

## Validation

Unity 6000.6.7f1 desktop Play mode passed HouseholdStorageChecks model/runtime (villager construction, pause, cost, capacity, spoilage, save), ResinChecks model/runtime (tap, carry, deposit, interruption, renewal/save), FoodReserveChecks (visible fullness, priorities, native delivery, save), and ResourceChecks model/runtime. Empty/half/full renders verified 0/6/12 timber slots, three upgrade stages, and the unchanged 10-by-6-by-8 navigation obstacle. New storage frame artwork has no colliders. Live console reported zero errors.

Quest headset readability and device performance remain unverified. No APK built. User season save and original XR startup settings are restored after checks. Crops, tributes, discovery props and other procedural-art gaps remain separate batches.

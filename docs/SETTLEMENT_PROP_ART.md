# Kenney settlement props

Source: https://kenney.nl/assets/survival-kit
Author: Kenney. Pack: Survival Kit 2.0 (3 April 2024).
License: CC0 1.0; original License.txt included with the source FBX files.

Only the selected tool, sign, workbench, crate and timber meshes are imported.
Generated runtime prefabs share the original colormap through BWVR/GameSurface,
are normalized to one unit tall with their base at zero, and have no colliders.
Rebuild with BWVR > Art > Rebuild Kenney settlement props in the Editor.

The axe and hoe replace procedural held tools. The workbench displays the tool
silhouettes. Wooden resource signs use physical log, wheat and jar symbols.
Timber piles, grain sacks and resin jars represent fractions of current capacity;
additional storage shelves still appear when villagers build upgrades. Meal
reserves use eight visible portions in an open crate, relative to the village's
food target. These are visual summaries, not extra inventory or pickups.

The storage navigation footprint, save format, resource costs, work scheduling
and learned-tool benefits are unchanged. Other tools are available as art only;
importing a hammer, shovel or pickaxe does not unlock new gameplay capabilities.

## Sourced offerings and water containers (2026-10-28)

Selected Survival Kit `bucket` and `barrel-open` models now replace procedural containers. The three native offerings use bucket, Kenney Food Kit apple and the existing timber model. Farmers carry the Quaternius sack and foresters carry timber. The sack insert contains original authored cereal faces extracted from Food Kit `bowl-cereal`, combined into one small sourced derivative; it no longer uses a Unity sphere. Food Kit CC0 license, source FBX/palette, archive/file hashes and derivation notes are retained in `Assets/ThirdParty/KenneyFood`.

Rebuild the selected Survival models with `SettlementPropImporter.BuildSelected("Assets/ThirdParty/KenneySurvival/", "Assets/BWVR/Resources/SettlementProps/", "bucket", "barrel-open")`, the selected Food models with the same method using `Assets/ThirdParty/KenneyFood/`, `Assets/BWVR/Resources/FoodProps/`, `apple`, `bowl-cereal`, then rebuild the sack through BWVR > Art > Rebuild food sack. See [ASSET_SOURCING.md](ASSET_SOURCING.md) for scope, remaining replacements and verification limits.

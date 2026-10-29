# CC0 tribute and discovery artwork

Integrated 2026-10-29. Six discovery clearings retain their six treats and six toys, fixed IDs, ownership, creature affordances, audio, rewards and restock timers. Source geometry replaces their rendered primitive assemblies.

| Item | Authored source | Integration |
| --- | --- | --- |
| Strawberry treat | Kenney Food Kit strawberry | Replaces berry/leaf primitive assembly inside the same food pickup root and collider. |
| Play hammer | Kenney Survival tool-hammer | Replaces the mallet primitive assembly; retains turning/play affordance and native carrying. |
| Seed shaker | Kenney Survival bottle | A sealed bottle presented as a seed shaker, keeping the old rattle/chime affordance and existing discovery-note sound. This is an adaptation of a bottle, not a separately authored rattle model. |
| Clearing stones | Kenney Survival rock-a/b/c | Five collider-free stones per clearing, fitted to the former stone dimensions and projected positions. No new navigation obstacles. |
| Communal tribute | Kenney Survival box-open with Food Kit apples, pears and oranges | Same large food pickup, seven visible fruits and native courier route. Crate tint distinguishes fear from affection; produce retains original palette colors. |

Sources, exact filenames, license records and SHA256 hashes are in ArtSources/Discoveries/SOURCES.json. Original bundled CC0 licenses remain under Assets/ThirdParty/KenneyFood and KenneySurvival. The already-selected hammer/crate/apple models are reused. Imported originals retain their topology, UVs and palette; runtime fitting adjusts size and pivot only. No visible primitive models are added.

## Rebuild and ownership

Use SettlementPropImporter.BuildSelected in Unity Edit mode with source Assets/ThirdParty/KenneyFood/, destination Assets/BWVR/Resources/FoodProps/, and strawberry/pear/orange. For source Assets/ThirdParty/KenneySurvival/, destination Assets/BWVR/Resources/SettlementProps/, select bottle/rock-a/rock-b/rock-c. Existing apple, hammer and crate prefabs do not need rebuilding.

SourcedItemArt.Fit centers a collider-free model under an existing gameplay root and fits its bounds without adding simulation components. DiscoveryObject caches child renderers after artwork creation, so depleted treats hide and restock correctly. The communal crate owns a cloned palette material; imported shared materials are retained. Roots, colliders, reservations, audio and save schema remain authoritative and unchanged. Food/water priorities and test energy settings are unchanged.

## Verification

Unity 6000.6.7f1 desktop Play mode passed DiscoveryChecks (six clearings, reachable items, independent exploration, exclusive ownership, creature delight/hunger, depletion/restock/pause, no repeat reward farming and save validation) and TributeChecks (strict majority, fear/affection, native courier delivery, crop cost/cooldown, autonomous eating and save validation). Native player grab/release was also exercised for all three discovery types, including collider disabling/restoration and pickup-volume fitting. A rendered model sheet was visually reviewed.

The legacy test fixtures were corrected for the twelve-adult village: tribute consensus now selects a real population majority, and discovery verifies the actual explorer identity. During the creature-only feast test, frozen couriers are staged clear and the creature starts at a sampled approach; routes, movement and eating still execute normally. No production consensus or navigation rule was weakened.

User season save and XR initialization settings are restored after QA. Quest headset readability and frame time remain unverified. No APK built. Crop growth models, spontaneous individual gift artwork and other remaining source-inventory gaps are separate work.

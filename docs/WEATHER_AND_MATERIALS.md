# Weather and material interactions

The island has a saved, seeded weather cycle: clear intervals, wind, fog, rain, thunderstorms, and a travelling storm wave after a thunderstorm. The first clear interval lasts 90 simulation seconds. Later ordinary conditions last 60–120 seconds; the large wave crosses the island in 24 seconds. Conditions blend rather than switching the whole view instantly. Water jobs remain secondary in development focus.

`IslandWeather` owns the environmental simulation. It advances through `QuestSlice.Simulate`, including free play, and respects pause and application suspension. Climate, random seed, partial thermal tick, changed materials and wave receipts live in optional `SliceState.Weather`; earlier saves remain valid. Starting again resets weather and the existing structural damage state. No new packages or controller bindings.

## Interactions

- Rain wets fuel, removes heat, extinguishes fire and slowly replenishes village water/crops. The existing paid rain miracle also wets and cools objects within 24 game units of the cistern. Its existing larger resource reward and energy cost remain owned by `DivineIntervention`.
- Lightning draws one brief local bolt, without a screen flash or flashing light. A dry tree or timber building can ignite; sufficiently wet fuel resists ignition. A struck villager can be injured.
- Burning consumes finite fuel and damages the existing `PlayImpactTarget`. Tree foliage and building timber have different ignition heat, heat capacity and burn rate. Nearby fuel receives heat, with a stronger downwind contribution. Spread uses the previous tick's burning sources so an entire forest cannot ignite in one update.
- Nearby villagers recoil from fire and may be injured by prolonged exposure. Weather does not reduce their trust in the creature or create an eligible creature-training completion.
- Wind changes cloud drift, slants rain, increases ocean waves and applies small forces to existing beachball/tree physics bodies. It does not create a Rigidbody for every tree.
- The large storm wave is wide enough for the ocean grid and has one shared analytical height field for rendering and coastal contact. It wets/extinguishes and damages low-lying trees/buildings, can injure exposed villagers, and pushes a loose beachball. Each target receives one damage event per wave. High terrain remains protected by elevation. Storm-wave ball motion cancels the previous play attribution so it cannot earn creature rewards.

## Quest budgets and limits

Thermal/material interactions run at 1 Hz against a cached target registry and a reusable spatial grid for nearby fuel, not scene searches each frame. There are at most 192 sparse changed-material records and 16 active fires. Cold unchanged records are reclaimed. Rain is concentrated around the creature with a 192-particle cap; each of the 16 pooled fires has 10 particles. Existing six cloud meshes, three fog pockets, one ocean grid and directional shadow map are reused. Fire and lightning add no shadow lights or reflection passes.

This is a gameplay material model, not fluid dynamics, fracture, or structural engineering. Waves do not erode terrain, simulate indoor flooding, or topple whole buildings with joints. Destroyed structures use the existing damaged/rubble representation and conservative navigation obstacles. Wind does not uproot planted trees. Villagers react to hazards; autonomous firefighting, shelters, rebuilding houses, sound design and seasonal ecology remain follow-up work. Quest frame rate still needs a headset measurement.

## Development and verification

In Play mode, use `IslandWeather.SetWeather(IslandWeatherKind.Thunderstorm)` (or `Rain`, `Fog`, `Wind`, `TidalWave`) through the Unity CLI. Set `automatic=false` when isolating a condition. `Strike(stableTargetId)` exercises a targeted strike. These are development hooks, not a new player command panel.

Editor checks: `WeatherChecks.VerifyModel()` and `WeatherChecks.VerifyRuntime()`. The runtime check isolates season state and restores original damage/visuals afterward. Keep a backup of the player's save when doing longer staged tests; the normal autosave can run between commands.

Verified in Unity 6000.6.7f1 on 2026-10-25:

- Model and live interaction checks passed, including ignition of trees and buildings, actual spread to neighboring fuel, rain extinguishing, pause, bounded fires, coastal damage and save validity.
- An isolated 1,800-second climate run visited all six conditions and retained valid state. A staged wave pushed a beachball without recording a successful or harmful creature throw.
- Twelve existing checks passed: hybrid model/runtime, navigation, bridge, creature skills, creature carry, feedback, shared activities, shared experiences, ball rallies, development focus and tree throwing.
- Live frames advanced from 1,745 to 36,007. Temporary camera captures were inspected for rain, roof flames, local lightning and a travelling ocean crest. The water shader reported no compiler messages. No gameplay errors appeared in the final console window; existing compiler warnings and the XR audio fallback warning remained.
- The original local save was restored byte-for-byte after stopping Play. No APK was rebuilt and standalone Quest performance was not measured.

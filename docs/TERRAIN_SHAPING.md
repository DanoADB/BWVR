# Bounded terrain shaping

God tools now includes **Raise ground**, **Lower ground** and **Smooth ground**. This is the first S8 terrain-editing increment, confined to clear, dry, gentle ground. It does not excavate under buildings, move forests, create islands or simulate flowing water.

## Play

Select Terraform in the low God Tools dock. T, the underside-wrist gesture and Sanctuary remain alternate entry points. Select a sculpt disk and aim at open island ground. A cyan proposed surface and ring indicate an eligible patch; red indicates a rejected patch. The proposed surface is an effect derived from the island grid, not replacement terrain artwork.

Hold the right front/index trigger, or desktop F, still for half a second. Release to request the stroke. Moving more than five original game units cancels the charge. Navigation prepares in the background while the existing island remains playable. The stroke appears only after both navigation profiles are ready and the patch is checked again. Closing/changing tools, Escape, pause or application suspension cancels a pending stroke. Nothing is spent for a preview, canceled charge or rejected request.

Each stroke covers a 30-original-unit radius. Raise/lower applies a one-unit centre displacement with squared radial falloff. Smooth moves toward the current centre height by a weighted quarter-step. Each grid point remains within four original units of its imported height. At most 2,048 edited grid points are saved. The settling cooldown is 1.5 simulation seconds. Configured cost is eight spiritual energy per successful stroke; unlimited development energy remains enabled.

## Safety and integration

- Protect original tree/building/field/well/rock/feature positions, current actors, resource plots, moved trees, construction pieces, physical toys and other nearby island colliders. Construction bounds must be converted from their private physics space before testing.
- Ground, render mesh and height queries use a runtime copy. Imported `FirstIsland` and its mesh remain unchanged.
- A requested stroke prepares two replacement navigation datasets using Unity's [asynchronous NavMesh builder](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AI.NavMeshBuilder.UpdateNavMeshDataAsync.html). Source revisions, target occupancy, pause, energy and season identity are checked again before publication. Preparation times out after 20 real seconds.
- Publication updates the rendered mesh, main collision, private beachball/tree-flight collision and both navigation profiles together, then invalidates routes and grounds free actors. Construction yards retain their bounded foundations and are excluded from editing.
- Pending operations have no saved effects. Confirmed sparse height deltas are source-hash/grid matched, bounded, unique and finite. Old saves without terrain edits remain valid. New season restores the original surface.
- Successful strokes are player-miracle evidence and invalidate creature teaching attribution. They do not count as creature work.

## Artwork

The sculpt disks reuse the established scroll and raised-stone presentation. Their mountain glyph is **Peaks by Lorc**, [Game-icons.net](https://game-icons.net/1x1/lorc/peaks.html), **CC BY 3.0**. Lower rotates the glyph; Smooth vertically compresses it. The original transparent PNG is `Resources/GodToolIcons/terrain.png`; its source URL, license and SHA-256 are retained in the shipped icon credits. This is attributed open artwork, not CC0.

## Validation boundary

`TerrainSculptChecks` exercises save validation, source isolation, previews, charged release/cancel, pause, protected targets, energy, raise/lower/smooth, collision/navigation and native Save/Load. `TerrainAsyncChecks` adds a multi-frame preparation/publication test, private physics mesh equality, and rejection when a villager enters a pending patch. The synchronous Editor-only core helper is not the player stroke path.

Verified in the running Unity 6000.6.7f1 Editor on 2026-10-31: model/save validation; charged release/cancel and native save/resume including the settling cooldown; raise/lower/smooth; separate navigation profiles; main and private-physics ground consistency; multi-frame publication and occupied-patch rejection. Existing navigation, god-tool selection/healing/input ownership, rain and wind checks pass. Rendered palette and circular ground-preview views were inspected. Desktop timings are diagnostic, not Quest performance measurements. Standalone/headset comfort, menu reach at supported zoom and final collider-publication frame time remain hardware gates; no APK is part of this increment.

One desktop asynchronous publication sample measured 79.6 ms, including mesh/collider updates and receipts. Moving navigation preparation off the player path avoids the roughly 0.83-second synchronous test bake, but final publication still needs optimization and hardware profiling before a Quest release. Neither number establishes headset frame rate or comfort.

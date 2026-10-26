# Useful construction shelters — S3 first increment

A shelter is detected from the existing construction pieces. It does not require an Arch/Bench/Tower recipe flag. No new block inventory, resource grants or actor movement controller is introduced.

## Build and use

In the main construction yard, pick up and snap two tall Column pieces four grid units apart, then bridge them with a flat Slab. A useful example is columns at local (-2, 2.4, -12) and (2, 2.4, -12), with the slab at (0, 5.4, -12). Leave the center and at least one straight corridor to the yard edge clear. Another supported arrangement uses two stacks of two Cube pieces and a quarter-turned slab at the same height. These are examples, not mandatory recipes. Other valid Beam/Slab arrangements can work.

Rest-seeking villagers may choose usable cover through their existing leisure decision loop. They retain the choice of other activities. The visitor approaches the clear entrance, walks under the roof, rests for six simulation seconds and visibly reacts on completion. One visitor uses the shelter at a time. There are no new controls or permanent signs.

A completed sheltered rest reduces rest need by 0.45 plus up to 0.20 extra relief during observed rain. The benefit is applied once on confirmed completion, never from a roof merely existing nearby. Shelter visits and covered duration persist in villager memory; cover itself is recalculated from the saved construction geometry. Existing saves start with zero shelter visits. This increment provides comfortable rest; it does not add personal wetness, rain-particle collision, storage protection or an injury immunity system.

## Geometry and ownership

- Only flat, attached Slab/Beam roofs with intact, grounded support chains qualify. Two braced sides must touch the underside; a single cantilever does not qualify.
- Headroom and the clear corridor use the existing villager navigation profile, including the current enlarged villager size. Roofs that are too low, tilted or moving are excluded.
- A conservative swept upright clearance prism rejects loose pieces, walls and supports in the rest space or entry corridor. Existing island navigation must also find a complete route to the entrance and resting point.
- Held, reserved or detached pieces cannot support a roof. The visitor lease shares existing construction/repair ownership, and held villagers abandon visits through the existing carry logic.
- Active visits recheck geometry at 4 Hz and again before awarding relief. Removing the roof or a needed support cancels the activity and releases the visitor lease; no benefit is granted for an incomplete visit.
- Simulation pause suspends timers. Reload restores experience and pieces, without replaying an active rest or its reward.
- No creature praise, recipe mastery or player miracle credit is fabricated for a villager resting. Shelter use does not pretend that an incomplete named recipe was completed or repaired.

## Validation

`ShelterChecks.VerifyModel` checks measured dry/rain rest relief, bounded saved experience and legacy social memory. `VerifyRuntime` stages two arrangements, checks held/support/headroom/obstruction failures, native navigation, visitor ownership, pause, completion/removal and restored geometry. On 2026-10-26 the memory checks passed; the corrected runtime fixture body passed through the live Editor evaluator at frame 8661 after releasing its stale resource-job lease. The test used native villager navigation and existing construction physics. Construction-use, miracle, rain-brush, wind-fan, god-tools and farming-model regression checks all passed at frame 10823. A temporary camera rendered the column/slab canopy and confirmed useful cover at frame 12538. Final scripts compiled with zero reported errors. Play mode was stopped, XR automatic startup restored for Standalone and Android, and the original on-disk season restored with matching SHA256 hashes. Quest comfort and device performance require a headset playtest.

## Remaining S3 work

Physical planting from a real bounded seed/plant supply at eligible existing plots, with safe preview, exactly-once resource payment and saved growth. Autonomous food gathering and water remain deprioritized while these interactions are developed.

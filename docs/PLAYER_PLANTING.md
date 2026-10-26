> Current input: hold the tool trigger (desktop F) to plant at the pointer. Sweeping across eligible plots plants at most twice per simulation second; staying on a growing plot does not spend again. Release stops planting.

# Player planting — S3 second increment

Open **God tools**, select **Plant seeds**, aim at an empty field outline, then press the normal tool trigger (desktop **F**). Release before planting another plot. The selected tool shows the existing CC0 food-sack model as a seed pouch. Temporary outlines appear only while this tool is selected: green squares are eligible; crossed amber outlines need supplies or have a current restriction. The pointer also uses a crossed invalid shape, plus a concise reason.

Planting sows wheat in the existing depleted fields or four bounded expansion sites. A depleted prepared field spends **1 wheat from stores as seed**. A previously unprepared expansion field also spends **2 timber** to prepare soil. Costs are shown in the tool hint. This deliberately remains a finite material interaction while spiritual energy is unlimited for testing. It cannot replace standing crops, claim arbitrary terrain or spawn free forests. Tree planting and crop-selection controls are later extensions.

## Integration and limits

- Reuses `ResourceState.PrepareFarm`, `Sow`, growth, soil/crop art, source IDs and saved resource ledgers. No new save schema or plant physics.
- Preparation and sowing commit together; failed attempts cannot consume only timber or only seed. A growing plot rejects duplicate payment after another click or reload.
- The preview is read-only. Application rechecks the plot, supplies and reservations. Held input cannot plant repeatedly or sweep several fields in a single press.
- At most three fields are growing or reserved for planting/preparation. At most twelve cached field outlines are rendered; refresh is limited to 4 Hz. These are private mesh/line views with no colliders or actor affordances.
- Existing villager plot, seed, timber and tool reservations take precedence. Pause, suspend, tracking loss, menu/carry/zoom input ownership and cancellation use the current palette rules.
- New-field safety uses the existing reachable, dry farm footprint. The pointer must aim near a registered field on dry ground; nothing relocates a plot or alters navigation geometry.
- Planted fields show soil and seedlings immediately and mature through the existing crop/weather clock. Rain helps existing growth. Pause suspends it; saved fields resume without replanting or replaying payment.
- Player planting is not a creature action or miracle. It creates no creature training receipt, global crop/water bonus or miracle credit. The existing finite seed/timber counters record its actual cost.
- Food work and water remain deprioritized. Villagers can keep their other existing jobs, including collecting the timber needed for new fields.

## Validation

`PlayerPlantingChecks.VerifyModel` verifies atomic cost, preview purity, duplicate/reload protection, seed/timber shortage rejection, existing-field reuse and saved growth/maturity. `VerifyRuntime` covers the palette press/release flow, safety and pause/suspend gates, reservations, plot budget, visible growth, save data and private preview cleanup. On 2026-10-26 both new checks passed. The compiled live seed-tool check completed at frame 1051. Nine regression checks passed at frame 5587: shelter memory/runtime, farming model/runtime, villager construction use, rain brush, wind fan, god tools and miracles. The eight-tool palette was rendered at frame 6939; ready plot outlines and a planted half-grown field were staged and rendered at frames 13019 and 17146. The invalid pointer cross was reduced so it does not cover the whole field, then the matching live shape was rendered again. Final scripts compiled with zero reported errors. The original season file was restored after confirmed Play stop and matched its backup SHA256; Standalone and Android XR automatic startup were restored. Unlimited spiritual energy remained enabled. These are Editor/staged checks, not an unattended balance or headset playtest. Quest input comfort/readability and device performance remain hardware checks.

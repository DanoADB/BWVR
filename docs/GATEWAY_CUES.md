# Gateway project cues and bounded participation

2026-10-30 continuation of the optional Village gateway project. This adds presentation and invitation tuning, without new items, input bindings, resources, actor motors or saved progress fields.

## In the world

Only a player-started active project shows four low-key rings along the front of the main construction yard. Their order follows the Book: timber stored, player placement, creature placement, villager use. Achieved steps turn teal; unfinished steps remain subdued gold. A gold wire outline suggests a feasible next arch placement, and a teal ring identifies a suitable existing loose piece. When the arch and prior milestones are ready, the teal ring moves to the entrance.

These are light effects, not physical objects or signs. Six LineRenderers share one existing ConstructionPreview material and a fixed 144 positions. There are no colliders, physics bodies, UI canvases or new primitive artwork. Updates use the existing quarter-second project cadence and bounded main-yard block list; there is no global scene scan or per-frame path ranking. No flashing, forced camera movement, new sound loop or permanent HUD is added. This budget is an implementation bound, not a headset performance measurement.

Held pieces, a leased villager placement, or creature-owned construction hide the suggested piece/slot so they do not compete with the existing placement preview. Pause, locked/wrong-form yards and ended/inactive projects hide the cues. The completed display expires after eight simulation seconds. Cues are rebuilt from actual saved project/geometry state and award no receipts, resources, learning or completion. Removing a credited piece still uses the existing evidence validation.

## Participation changes

A witnessed player snap in an active main-yard arch project gets a 75-second shared-building invitation while the creature's contribution is missing, with an initial attention bias of .95. Ordinary construction retains its 45-second / .85 invitation. This remains a fading utility contribution, not a guaranteed command. Held/detached anchors, pause/suspend, wrong sites, infeasible pieces, unknown forms, urgent needs and existing action ownership still gate it. The project-specific invitation stops attracting further building once the missing creature contribution is recorded. Polling cues does not renew it, and save/load does not resurrect transient invitations.

A finished, supplied, shared project is considered earlier in the existing idle leisure chooser, after current social/comfort/place opportunities. A healthy, unfrightened, free villager must face a physically visible yard within the existing 40-unit observation range; the normal structure motor and single visitor lease remain authoritative. Shelter retains priority. Existing gathering, family, social, held-item and cathedral ownership are not interrupted. An approaching project visitor briefly gestures toward the arch, then uses the normal walkthrough and reaction. No villager is teleported, forcibly assigned from another task, or credited merely for noticing.

## Verification

GatewayCueChecks verifies the four milestone projection, fixed renderer/vertex budget, lack of colliders, real feasible piece/slot selection, read-only rendering, player/motor preview exclusion, pause/lock, ready entrance, and completion/ended visibility cleanup. Staged hint and ready-yard views were rendered and inspected.

GatewayProjectChecks now verifies the bounded 75-second invitation and withdrawal of its bias for a still-feasible loose piece after the creature contributes. Its existing end-to-end test now exercises the idle leisure chooser for a familiar visitor, followed by the real walkthrough, partial/completed native Save/Load and one project completion. SharedBuildingChecks confirms ordinary invitation behavior; VillageBuildingChecks confirms independent construction remains intact.

These Editor fixtures isolate positions and opportunities for reproducibility. They do not establish natural population-wide participation frequency, subjective fun, or Quest readability/comfort/frame time. Physical headset testing remains deferred. The original user save and XR startup flags are restored after validation; no APK is built. Food/water remain deprioritized and development energy remains unlimited.

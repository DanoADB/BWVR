# Look here: renew a shared-building invitation

2026-10-30. Adds a deliberate attention gesture to the existing shared-build invitation and physical input path. Headset playtesting remains deferred.

## Controls and response

Point at an attached block whose latest retained placement receipt belongs to the player. On Quest, tap and release the right front trigger to invite the creature again; hold it for at least 0.32 seconds to perform the existing native pickup. Desktop F invites on the press edge; E keeps its separate pickup behavior. Left-hand carrying, loose objects, non-player-built blocks, creature pickup and right side-grip zoom keep their established bindings. A canceled tap never becomes an invitation when tracking is lost or the ray leaves the block.

This tap/hold arbitration is needed because the right Quest front trigger previously performed pickup before the interaction path could run. It applies only to recorded player-built construction anchors, not to every grabbable object. The Book's shared-building and gateway instructions explain the controls.

A successful gesture uses the existing noticed/curious sound, gaze and affect response. It renews the bounded same-yard invitation: normally 45 seconds, or the existing 75-second gateway window while the creature contribution is missing. There is an eight-simulation-second receipt cooldown. The creature may continue its current action or choose another opportunity; the gesture neither interrupts its motor nor reserves a piece. Existing urgent-needs, navigation, utility, failure and ownership rules remain authoritative.

Unknown forms, invisible anchors, completed/locked/paused/bridge yards, held items, active site visitors/repairers/requesters and unavailable pieces cannot create a usable invitation. Provenance uses the site's exact saved contribution slot and latest retained placement record; anonymous or creature/villager-authored placements do not become player work. The existing history is bounded to 48 placements, so anchors without retained provenance use ordinary pickup rather than inferred ownership.

## Evidence and persistence

Pointing records a GuidanceOffered event and its acknowledgment as PlayerGuidance. It does not append a placement receipt, stock resources, complete a project, teach a form or mint a reinforcement window. Existing completed-action receipts remain unchanged. A later utility-selected construction action records the invitation's own evidence ID as its cause. A new gesture during an already started action does not relabel that action or change its original cause. Ordinary guidance still uses its own evidence ID.

This invitation remains transient: save/load keeps physical geometry, placement provenance and historical evidence, but does not replay attention. The player can tap again after reload. No new persistent state schema, input package, AI framework, world marker, primitive art or UI hierarchy is introduced. Menus, God Tools, zoom, explicit pause, suspension, tracking loss, carrying and a changed save state cancel pending taps before they can fire.

## Verification

BuildAttentionChecks passed in Unity 6000.6.7f1. The native fixture verifies anonymous rejection, an expired player invitation, right-trigger tap versus hold with native TryGrab/Release, pointer drag cancellation, tracking/menu blocking, pause, visibility, unknown-form rejection, cooldown, desktop press-edge behavior, no fabricated work/teaching, native Save/Load without invitation replay, a renewed utility-selected native carry/place and one-shot praise. It also verifies that a later invitation preserves an active action and its causal record.

Hybrid model/runtime, shared-building runtime, gateway project runtime and Book model/runtime regressions pass. Updated Book instructions were rendered and inspected inside their existing layout. Actor positions and competing opportunities are staged for repeatability; natural acceptance rate and subjective timing are not established by this check. Right-trigger timing and hand comfort still need tomorrow's physical Quest playtest.

Original user save and XR startup settings are restored after validation. Food/water remain deprioritized, development energy remains unlimited, and no APK is built.

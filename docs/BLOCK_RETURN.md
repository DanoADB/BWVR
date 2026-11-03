# Return the latest player block

In **Book of Creation → Places → Build with me**, use **Return last block** to send your latest confirmed, safe placement back to its loose supply position. The existing rounded rectangle button has a recall icon and an availability explanation. This is a single-piece recall, not a full assembly undo or a restoration of an earlier pose.

Only an actual confirmed player snap grants the one-use ticket. Holding, previewing or rotating does not. A fresh confirmed placement replaces the ticket. The ticket stores the site, block, receipt and local pose; it survives saving. Missing/empty tickets from older saves remain valid but unavailable. Consumed tickets cannot be used again after loading. An aged-out placement receipt disables recall without invalidating the save.

The click rechecks the current state. Pause, suspension, held objects, bridges, locked or busy yards, villager ownership/occupancy, creature construction, Build & Bowl reservations and active construction projects prevent recall. Changed poses, missing attachments, newer ownership receipts, creature-built pieces and supporting pieces also prevent it. Return uses the existing native loose-supply reset and withdraws the block's building invitation. Historical confirmed placements remain recorded. No new placement, reward, lesson or resource refund is generated.

## Validation — 2026-11-03

Editor compilation passed. `BlockReturnChecks.Verify` exercised native pickup, raycast placement, pure Book navigation, layout bounds, the actual tracked Book pointer, pause/lock/occupancy/project/moved/creature/support guards, one-use recall, unchanged contribution history and snap totals, native Save/Load, new-placement renewal, changed attribution and malformed-ticket rejection. An empty season JSON roundtrip caught and verified Unity's inline optional-object representation. A staged Book render was inspected, including single-line caption fit, uniform button dimensions and an unobstructed physical UI ray.

Existing placement-contribution model/runtime, Book model/runtime (all page layouts), shared-yard and shared-building-attention checks passed. Live Editor frames advanced. Android player scripts compiled successfully (43 assemblies); no APK was built. The original season save was restored byte-for-byte before the final user Link session. Desktop/Android XR initialization was restored and verified enabled. BWVR was reopened in a visible Unity Editor on FirstQuestSlice. A subsequent Link launch reported an active OpenXR display, head tracking and two Quest Pro controllers; headset visual confirmation remains a user check.

Food/water remain deprioritized. Quest comfort, physical button readability and prolonged construction play remain playtest gates.

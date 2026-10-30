# Useful buildings and villager thanks

After a villager first uses a construction form comfortably, credit now reflects the creature's fraction of the pieces satisfying that recipe. Previously, any player-built or decorative piece prevented the all-creature test from passing. Joint construction can now earn trust, affection and weighted construction preference without attributing the player's work to the creature. Loose pieces, unrelated decorative extras and incomplete recipes do not contribute.

A nearby villager can then walk toward the creature, face it and make a short friendly gesture. Completing that gesture produces the existing positive affect icon, a quiet content sound and a bounded journal entry. The creature can acknowledge if idle; its active movement/carry action is never interrupted. There is no new command, menu, asset or animation controller.

## Rules

- Credit is applied only after native structure use completes, not when construction snaps, an invitation is shown or a villager starts approaching.
- The existing first-form guard remains: one credit opportunity per villager/form in that season. Previously used forms do not gain retroactive credit. Repeated visits still give their normal benefits without farming trust or teaching.
- Player-only construction still provides its normal rest/place benefit, with no creature credit. Exposed benches in rain do not earn positive credit.
- Full creature contribution retains the prior +0.06 trust / +0.03 affection / +0.12 form-preference maximum. Partial contributions scale those deltas linearly.
- The thank-you is optional: within 60 game units, reachable approach, healthy/unfrightened villager and unheld creature. It times out after 12 active seconds. Pickup, injury, fright, target loss and ordinary task reset clean it up. Paused/suspended simulation does not advance it.
- Structure visitor reservations are released before approaching the creature. The existing villager leisure controller retains locomotion ownership; no carried items are claimed.

`SocialMemory.helpfulBuildUses` and `buildThanks` are bounded additions to the current save. Helpful uses cannot exceed actual construction uses; completed thank-yous cannot exceed helpful uses. Old saves default to zero without invented history. The Book's People profiles show the thank-you count beside construction uses. Pending gestures are transient and not saved/replayed; completed use/learning and completed thanks retain their normal season persistence.

## Verification

Unity 6000.6.7f1 compiles. `BuildThanksChecks.Verify()` isolates a reachable bench and a nearby creature, moves unrelated villagers out of the route and exercises actual native structure use. It verifies a 2/3 creature build yields +0.04 trust and +0.08 preference, decorative exclusions, visitor cleanup, approach and gesture fields, positive affect timer, pause, no repeat credit, serialized/corrupt-state checks, player-only rest without creature credit, exposed rainy benches, preserved full contribution, pickup/injury cancellation and incomplete-form rejection.

Existing construction-use, shared-building invitation, shared-shelter and Book model/runtime checks pass. The People page was rendered and visually inspected. These staged tests do not prove natural frequency or headset readability/comfort. No new physical Quest test or APK build is claimed. User save and XR startup flags are restored after QA.

Next: observe this cycle with the full population and tune invitations, approach frequency and gestures before adding more rewards or economy layers.

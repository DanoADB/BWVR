# Timber relay: villagers share useful physical work

S5 continuation, 2026-10-30. This extends the existing gathering motor, finite cargo, equipped axe work, peer relationships and Book of Creation. The existing turn-taking bench builder remains intact.

## In play

After actually felling a tree with an owned bound axe, a cutter can beckon while holding the normal timber bundle if a free, willing neighbor has a clear physical sightline within 18 island units. An idle neighbor may choose to approach, reach for the bundle for 1.2 seconds, take it, and carry it through the existing storage/deposit route. The cutter is then free to choose another activity. No player command, new marker, controller binding or artwork is added.

Offers last six simulation seconds without a helper, or up to 20 seconds with one approaching. An unaccepted or timed-out offer returns the cutter to ordinary solo carrying. Receiving an offer can bypass the recipient's ordinary gathering cooldown; it does not interrupt social play, exploration, held actors, family visits, cathedral activity, injury, fright, urgent storage or food emergencies. Recently harmed neighbors decline through existing mutual PeerWilling checks. Water/food remain deprioritized; this adds no food task.

The ordinary gathering chooser checks this bounded roster opportunity at 4 Hz while idle. No global scene scan is added, and physical visibility queries are skipped when no unclaimed offer exists. Participation still depends on the existing scheduler: other activities can win. Natural population-wide frequency is not yet tuned.

## Ownership, outcomes and saving

A single helper can claim each offer. Until the actual close-range handoff, the cutter retains the finite cargo and source reservation. Transfer moves the same load between cargo slots, without changing stock or source yield. The helper uses the existing carry animation and navigation. Canceling approach releases the helper without stealing the donor's wood. Canceling after transfer recovers unstocked cargo through the existing finite-source recovery rule. Weather safety, player-held actors, timeout and pause checks use the existing gathering lifecycle.

Only a positive native Deposit creates shared success. The latest saved relay records the cutter, carrier, source, accepted amount (at most three wood), time and a bounded cumulative completion count. Both villagers receive the existing cooldown-limited positive peer experience. The carrier does not gain axe-use practice; the cutter keeps the felling credit. Neither villager's work creates creature training, creature trust or faith. Actual relayed storage can complete the cutter's latest matching delivered-tool proof within its existing time window, without another resource award.

Book -> Provisions -> Next shows the latest completed relay and separate cutter/carrier attribution. Reads do not initialize history, and legacy/default empty records remain valid. Older relay details are replaced, not retained in an unbounded log.

Completed outcomes persist through native Save/Load. In-flight relay plans are not serialized: on world reconstruction their saved cargo returns to its finite source, following the existing gathering policy. This does not claim exact mid-gesture plan resumption. Application pause stops simulation; ordinary pause neither transfers cargo nor creates completion.

## Validation and limits

TimberRelayChecks.VerifyModel/VerifyRuntime pass in Unity 6000.6.7f1. The native staged fixture uses actual standing-tree approach/harvest, an idle helper's selector, native approach/take/carry/deposit, and exact stock checks. It verifies player-held/hostile/occluded helper rejection, pause, exclusive ownership, canceled approach, interrupted-load native Save/Load recovery, solo fallback, no duplicate completion, correct work/haul attribution, matching tool evidence, mutual peer memory and completed native Save/Load.

Resource model/runtime, Book model/runtime, tool delivery runtime and tool-use runtime regressions pass. The older tool-use test now accepts the new optional OfferWood phase immediately after a real harvest; it still checks actual learning, canceled cargo and later real storage. The populated Book page was rendered with a staged verified-value display fixture and visually inspected (208px preferred height inside the existing panel).

Tests stage actors and tick native motors for repeatability; they do not establish natural recruitment frequency, animation hand-contact polish or Quest performance/readability. Headset testing remains deferred. The original user save and XR startup flags are restored after testing. No APK is built.

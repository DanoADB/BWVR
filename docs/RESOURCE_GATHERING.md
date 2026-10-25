# Wood, wheat and shared stores

This is the first autonomous resource milestone: finite gathering, reserved carrying, shared stocks and food consumption. It replaces the live farmer/forester timer rewards with actual resource trips, while retaining the water routine and the existing leisure/social scheduler. Water remains secondary in development focus.

## Current loop

- A bounded registry selects up to 16 nearby suitable trees and eight existing field locations, using stable source identities and reachable native navigation approaches.
- Each source contains 12 units. Workers reserve one source and up to three units of store capacity. No stock is credited until delivery.
- Cutting a standing tree uses the existing work animation and a small hand axe. The tree becomes a stump, its original trunk navigation obstruction is removed through the existing asynchronous tree pipeline, and its remaining timber stays available for later trips. The creature cannot uproot a harvested or reserved tree. Workers prefer collecting timber already cut before felling more trees.
- Existing fields gain small batched wheat stalks. Harvesting reduces their visible height to stubble as the finite yield is depleted. This does not yet sow a new crop or create a farm.
- A covered village store shows timber stacks and grain sacks filling up. It has a native navigation footprint and a separate approach point. Workers carry a visible load, reach the store, unload, then return to the shared scheduler.
- Wood and wheat have capacities of 36 and 48 units. Low stocks encourage gathering toward a soft target of 18; simultaneous deliveries can exceed that target within capacity. Existing farmer/forester roles supply a small preference, rather than an exclusive resource assignment. The water gatherer retains its current work in normal play.
- Each resident consumes one stored wheat unit per 90 simulation seconds. Wheat is an abstract food ration in this slice; milling, baking, individual hunger/starvation consequences and dietary variety are later work. Food comes from actual stock rather than the old crop-health meter.

Work respects pause, application suspension, carried villagers, severe injury, frightening encounters, dangerous/destroyed sources and blocked or timed-out routes. Existing play, shared rests, building and repair behavior remains available between trips. A weather fright yields the worker's resource ownership before the recoil behavior takes over. Natural gathering grants no creature praise receipt or creature-blame penalty.

## Accounting and persistence

`SliceState.Resources` is optional for old saves. It holds stocks, source depletion/felling, delivery/consumption totals and three bounded cargo records. Harvesting debits the source into cargo; delivery transfers that amount to stock exactly once. Unaccepted overflow returns to its source. Reservations for source and free store capacity are transient and exclusive.

Canceling an action returns undelivered cargo to its source. Loading recovers saved cargo safely to the source and releases the old trip; the already-cut tree stays cut. There is no offline growth, duplicated delivery or invented replacement resource. New seasons reset source depletion and restore tree visuals/navigation. Cold source records remain finite; no scene-wide searches or plant Rigidbodies run every frame.

The visible store and worker loads represent counted inventory. They are not yet freely grabbable individual stock objects. Wood is not yet spent by toy construction. The store roof is a visual foundation for weather-aware storage; spoilage, fire/flood damage to inventories and weather forecasting are not implemented in this milestone.

## Next resource milestones

Replanting/sapling growth and deliberate forest creation; renewable resin; crop growth, sowing and new farm plots; additional foods; storage losses and weather preparation; then seasons. These remain queued in [gameplay direction](GAMEPLAY_DIRECTION.md). Existing tree play and resin-free toy systems are preserved rather than rewritten as an economy.

## Checks

`ResourceChecks.VerifyModel()` validates finite accounting, cargo limits, exactly-once delivery, capacity overflow recovery, consumption and legacy/corrupt saves. `ResourceChecks.VerifyRuntime()` exercises native cutting, harvesting, carrying and deposit, source exclusivity, creature exclusion, pause, interrupted workers and pending-cargo recovery.

Verified in Unity 6000.6.7f1 on 2026-10-25: both resource checks passed, together with 14 selected regressions covering hybrid model/runtime, development focus, social gatherings, shared play, the construction yard, village building/repair, bridges, navigation, tree play, weather, creature carry and feedback. The registry contained 16 tree sources and eight wheat patches.

An isolated 240-second run through the normal village scheduler, starting without stored food and with clear weather, completed four deliveries without resource commands: nine wood and three harvested wheat, of which about 2.12 food units were consumed. Two trees were felled. This demonstrates unattended progress, not a guarantee of self-sufficiency under every weather/social state. A staged render of the store and its stocks was inspected. Live frames advanced from 4,156 to 37,742; the final console window had no gameplay errors and one existing XR audio fallback warning. After stopping Play, the original save was restored byte-for-byte. No APK or standalone Quest performance test was part of this pass.

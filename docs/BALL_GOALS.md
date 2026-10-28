# Movable beachball goals

Implemented 2026-10-28. Editor-verified physical target practice; standalone Quest feel/performance still needs playtesting.

## Play

Three coloured wooden arches appear beside the beachball play area. Aim at a coloured crossbar and use the existing carry control to move an arch. Release over dry, level ground; its opening faces along your horizontal pointer direction. Unsafe or crowded placements return to the previous position. Arrange a single target or a longer course of up to three arches.

Use the existing beachball toss: carry and move the hand before release in Quest; desktop supports its existing Space + release toss. The entire ball must pass through an opening. Posts and crossbars physically deflect the ball. Each arch counts at most once per launch, in either direction. The badge flashes and a discovery cue plays after the throw settles safely (or reaches its bounded outcome time).

A creature that actually witnesses a successful player crossing can discover target practice. This adds a temporary utility bias, while bounded learned preference affects its seeded choice between goal practice and other ball activities. The creature remembers the chosen arch while collecting the ball, checks the route and nearby villagers again before throwing, and improves aim through successful practice. Praise/discouragement on a completed creature goal uses the existing 12-second, once-only feedback window. A player goal cannot become a creature training receipt.

No new controller buttons, command list, persistent HUD, packages or licensed assets are added. The wooden frames use original low-poly runtime geometry and shared materials. Water and food work remain deprioritized.

## Rules and integration

- Reuses BeachBall's private game-unit PhysX scene, existing time step, curiosity executor, navigation and action ownership. Nine kinematic frame colliders; three bounded swept opening tests per physics substep. Villager references are cached, not searched per frame.
- Swept centre-plane tests include sphere radius and fit within the physical opening. This handles fast crossings without relying on trigger callback timing.
- Held/moved arches must stabilize for half a simulation second. Moving an arch clears its pending crossing. Holding the ball, environmental impulses, returns, action cancellation and resets cancel unfinished credit.
- The pending result reserves the ball from autonomous grabbing. Harm before outcome completion discards all pending goals. Successful goal completion releases the ordinary fetch reservation. Each arch can contribute only once per launch.
- Goal counters, witnessed discoveries, preference, practice, and three game-coordinate positions/yaws persist. Old saves default to no goal experience and initial placements. Save while carrying stores the last placed position. Paused or swapped layouts restore together, independently of old positions; unsafe terrain placements fall back to defaults. Transient launches and flashes do not resume after reload.
- Frames constrain the ball. This increment does not add dynamic navigation cuts for actors around the new posts, ramp/block collisions with the ball, or a sequence/combo score. Broad safe-route checks reduce risky creature throws; they do not promise all physical outcomes are safe.

## Verification

Unity 6000.6.7f1: compilation with zero errors/warnings. `BallGoalChecks.VerifyModel()` covers swept/reverse crossings, clipping and near misses, legacy saves, bounded memories and typed receipt exclusion. `Verify()` uses a staged flat lane with bowling pins disabled to isolate real goal physics, moves villagers away, and temporarily limits competing opportunities for reproducible creature testing.

The runtime check exercises native goal grab/drop and native ball grab/toss/release, real ball crossing and physical post rebound, pause, held/moved arch and ball cancellation, harmful/unwitnessed outcomes, a utility-selected creature goal, one-shot discouragement, JSON state roundtrip, and paused/swapped layout restoration. This proves the integration under controlled conditions, not natural attendance frequency or final gameplay balance.

Targeted bowling, player rhythm/audience, shared activities, chase, sanctuary, pickup/drop and fetch checks also pass. A rendered Editor view was inspected. An existing scene-stop audio exception was also fixed by rejecting cues after the listener/rig is destroyed; destroyed-reference and existing audio checks pass, and the checked stop path has no new console errors. The user's existing save and XR startup settings are restored after testing. No APK or physical headset test was made for this increment.

Next candidates: player/creature turn-taking through a course and further nonverbal creature tricks, followed by headset tuning of the growing play set. Benches/shelters remain deferred.

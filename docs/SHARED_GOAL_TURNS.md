# Shared beachball goal turns

Implemented 2026-10-28. Editor-verified; Quest controller feel, readability, audio and performance remain untested on hardware.

## Play

Point at the small centre badge above a goal and press the right front trigger (desktop F) to invite a three-round game. Pointing at this badge consumes the select press before ordinary carry, so it does not pick up the arch. The crossbar remains the carry handle. Press the badge again to cancel.

The blue turn light means your turn; amber means the creature's turn or retrieval. Three small beads on each side record successful shots in the current game. There is no new text panel or permanent HUD.

Make a witnessed throw toward the selected goal from roughly 16–55 game units away. The first valid throw establishes a launch circle. A successful witnessed goal teaches the activity if it is unfamiliar. After an observed demonstration, later misses can still be part of the game; a miss is not a goal. Subsequent player throws must begin near the marked circle.

The creature can choose to retrieve the ball, physically carry it back to that circle, and attempt the same remembered goal. It then retrieves and returns the ball for your next turn. The circle stays visible between turns. A three-round finish keeps the final beads visible until a new invitation. Each completed round requires both attempts and the final physical return.

This is an invitation, not guaranteed obedience. Current needs, feasibility, learned goal preference, seeded willingness and utility selection still apply. Choosing another activity, an unreachable return, a timeout, harmful play, moving the goal, wind moving the ball, picking up the creature, taking the ball out of turn, reset or reload ends the unfinished game. Picking up the ball during your own waiting turn is allowed. A first unfamiliar missed demonstration does not magically teach target practice.

## Physical play and ownership

A visible rope backstop appears behind the selected arch, opposite the established launch point, during shared play. It uses one additional rectangular collider in the existing ball physics scene. It keeps many on-target shots near the flat play area through real collision; off-target shots can still escape. No ball is teleported back from a shot. The existing curiosity/fetch executor owns both retrievals, including hand carry, navigation, placement, reservation tickets and cleanup. The final small physics settle is allowed a positional tolerance.

Ordinary goals remain open without a backstop. Active turn games exclude competing villager ball reservations, player-ball chase and autonomous theft of the player's turn. Physical carrying by the creature is distinguished from player pickup. No competing motor, physics scene, network service or package is introduced. Nine original post colliders plus three optional net colliders are allocated; only the chosen net is enabled. Rope strands and turn beads use shared existing materials and original low-poly geometry. Dynamic actor navigation cuts around goal frames remain outside this increment.

## Learning and saving

Actual safe goal crossings retain their existing goal counters, practice and once-only contextual feedback. A course retrieval does not increment ordinary fetch returns or become a fetch lesson. Completed shared rounds increment a bounded shared-round counter and shared enjoyment, which biases subsequent invitations. Player goals and incomplete/interrupted rounds never become creature goal receipts or completed shared rounds.

Goal layout, goal learning, completed round count and shared enjoyment persist in the existing save. Live invitations, turn phase, temporary scores, launch circle and net are transient and clear on reload/new season. Old saves default to no shared rounds. The original user save and XR startup settings are restored after verification.

## Verification

Unity 6000.6.7f1 compiled with zero errors/warnings. `GoalTurnChecks.VerifyModel()` checks historical saves and bounded shared memories. `Verify()` exercises a physical badge ray, press edges, tracking/side/menu/pause guards, three native player pickup/toss/release cycles, utility-selected creature retrievals and attempts, the final carries/placements, visible launch circle, player-turn exclusivity, distinct fetch attribution, once-only round memory, badge cancellation, moved-goal cancellation, expiry and reload.

The integration fixture uses an existing flat lane, disables bowling pins and competing construction ideas, moves villagers aside, and controls the seed for willing creature attempts. It does not force locomotion or teleport shot returns. This verifies the complete flow under controlled conditions; it does not measure natural game acceptance frequency or final balance. The standalone goal fixture now similarly isolates competing ideas and willingness so running it after social tests remains reproducible.

Existing goal physics, bowling, fetch, pickup/drop, chase, villager shared activities, chime echoes/audiences and sanctuary checks pass. A staged Editor render of the net, turn beads and returned ball was inspected; normal Editor frames were also confirmed advancing. No APK or physical headset test was made.

Next: further nonverbal creature tricks and hands-on playful interactions, followed by headset tuning of the play set. Water/food work remains deprioritized and benches/shelters deferred.

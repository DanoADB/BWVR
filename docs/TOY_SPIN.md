# Toy-led turn-around trick

Implemented 2026-10-28. Editor-verified; Quest gesture comfort and recognition tolerance still need headset testing.

## Play

Hold the beachball in front of an attentive creature, roughly at its head height. Draw a slow closed circle in a vertical plane, about one or two ball widths across, over roughly 2–6 seconds. Either direction works. The creature can notice the gesture and choose to turn around with stepping feet. Keep holding the ball nearby until the turn finishes.

A completed turn takes about three seconds and keeps the creature on its standing footprint. Release the ball, then use the existing A/B praise/discourage controls (desktop 1/2) within the 12-second teaching window. Existing feedback protection blocks teaching while carrying. Learning changes future spin utility rather than guaranteeing compliance. No new buttons or command menu are added.

Near-vertical curved toy motion can interrupt an existing follow-the-ball action so following does not consume the circle. Straight-line chase still works. Only a complete recognized circle offers the trick or records a cue; lifting the ball, jitter, a line, depth wandering or tracking jumps do not earn a spin. The invitation expires after eight simulation seconds. After an attempt there is a 12-second cooldown, preserved when putting down and picking up the ball.

## Integration and limits

`CircleGesture` keeps at most 64 samples in a six-second window. Sampling is bounded at 10 Hz in a stable creature-facing plane. Width/height, closure, depth, angular travel, reversals, sample gaps and discontinuities are checked. A visible nearby held player toy is required. Actor movement/turning during perception resets the frame. In-progress vertical motion briefly gives the gesture attention ahead of ordinary chasing; it does not teach anything by itself.

`BallSpin` supplies perception and a bounded action adapter. Utility selection, typed intention/receipt, exclusive action ownership and interruption remain in CreatureHybrid/CreatureCuriosity. The creature can decline while occupied or favor another need. There is no second locomotion controller and it never takes ownership of the player's ball. The existing authored in-place walk blend provides stepping feet while the actor rotates; this is a lightweight turn adapter, not a new authored spin clip.

Standing room, nearby actor clearance and local terrain height are checked before starting and during the turn. Rotation stays on the existing footprint; no jump, forced camera movement or player movement is introduced. Pause freezes the action. Tracking/input loss, toy release, another actor obstructing the footprint, moving the creature, a harmful toy or losing sight at the end fails without a completed lesson. Active shared-goal games exclude the gesture. A failed/canceled action clears the stepping override and leaves the player-held toy alone.

Recognized cue count, completed spins, last direction, bounded practice history and learned preference save with WorldPlayMemory. Old saves start with no spin knowledge. Current gesture samples, cue expiry, cooldown, partial turn and pose are transient; reload/new season clears them. Existing contextual receipt rules distinguish spin from chase, fetch, bowling, goal shots and musical actions. Teaching remains once-only and expires normally. Water/food work stays deprioritized.

## Verification

Unity 6000.6.7f1 compiled with zero errors/warnings. `BallSpinChecks.VerifyModel()` covers clockwise/counter circles, once-only recognition, line/jitter/depth/fast-motion/teleport rejection, legacy saves and typed receipt bounds.

The Play-mode fixture uses native pickup, scripted player toy motion and normal utility selection on a flat lane. Competing construction ideas are temporarily disabled and other actors moved aside for reproducibility. It verifies both directions, a full grounded turn with stepping animation parameters, pause, exclusive player carry, native release and feedback with carrying/cooldown guards, positive/negative learning, save validity, input/tracking cancellation and shared-goal exclusion. It does not force the trick action or creature rotation from the test.

Targeted chase, fetch, pickup/drop, shared goals, goal physics, bowling, player chime echoes/audiences, villager shared play and sanctuary checks pass. Normal Editor frames were confirmed advancing. A staged render was inspected; manual offscreen captures require refreshed skinning matrices after changing the rig in the same frame, and are not headset screenshots. No licensed art was added or changed, no APK was built, and physical Quest performance/gesture feel remains unverified. User save and XR settings are restored after verification.

Next candidates: villagers noticing and reacting to tricks, and simple cooperative toy puzzles. Benches/shelters remain deferred.

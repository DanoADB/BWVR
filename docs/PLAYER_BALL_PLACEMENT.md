# Native beachball placement

A slow player drop now hands the final ground position directly to the beachball's private physics scene. The rendered sphere and Rigidbody agree immediately, including when pickup/release happens between simulation ticks. Ground placement uses the actual ball radius, rather than the generic small-object offset. Invalid targets return the visual object and physics body to the existing home together. Release cancels pending fetch/rally/flight state without crediting a creature throw. Measured player tosses retain their existing branch and fetch behavior.

## Reproduced defect

Before the fix, a native TryGrab/Release pair moved the visible ball approximately 18 game units while its Rigidbody remained at the previous position. The next 0.02-second ball advance snapped it back. The regular held-state polling happened to mask this for longer carries, but was not a reliable release handoff. The previous generic drop offset also placed the ball center only 2 units above the ground despite its 3.5-unit radius.

`BeachBall.PlaceOnGround` reuses the existing ReturnHome cleanup, then applies the accepted new center and current rotation, wakes the body, and resets the contact sample. QuestSlice dispatches to it after validating a non-toss target. No control, save schema, priority, resource cost or automatic play behavior changes.

## Verification — 2026-10-27

`PlayerBallPlacementChecks.Verify` uses the native pickup/release handlers. It checks a quick drop without an intervening ball tick, an ordinary held drop, immediate visual/body agreement, radius-correct ground contact, the next physics step, paused placement, canceling a fetch by replacing the ball, and invalid-target return. The test does not reposition the Rigidbody to manufacture the asserted release result.

Native player toss/fetch and the full five-minute village-scheduler regression also pass. Compilation reports no errors or warnings. Existing Book model purity/save checks passed during the diagnostic session. The user's original save is backed up and restored separately after stopping the temporary Editor; unrelated project settings are excluded from this change.

## Combined S4 scenario remains open

A local 900-second probe used native player pointer placement for a brick/beam bench, a supported roof and a beachball, with ordinary QuestSlice scheduling between interventions. One run recorded six uses before rain, three rainy shelter rests, and two shared-rest encounters (four participants) by the end. Ball-game completions remained zero, so placement alone did not prove a third useful contribution. No activity-completion counters or memories were injected.

A later attempt to make this a committed combined regression failed the roof-usefulness check after native placement. Its arrangement failure remains unresolved; that diagnostic was not retained as a passing regression test. The three-useful-contribution gate, declined-suggestion evidence, reliable natural play/music pacing and physical headset checks remain pending. This increment fixes the confirmed player-placement defect without forcing cooperation to satisfy the larger milestone.
